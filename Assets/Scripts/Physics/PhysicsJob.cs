using System;
using GameConsole;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

[BurstCompile(FloatPrecision.High, FloatMode.Strict)]
public struct PhysicsJob : IJob
{
	// Collision settings
	public int PhysicsPassesPerFrame;
	public float VelocityDragConstant;
	public float VelocityDragRelative;
	public float BounceAbsorb;
	public float LeftEdge, RightEdge, TopEdge, BottomEdge;

	// SoA state of the battle
	public int objectCount;
	[ReadOnly] public NativeArray<ColliderProperties> properties;
	public NativeArray<float2> origPositions;
	public NativeArray<float2> positions;
	public NativeArray<float2> velocities;
	public float dT;

	// output events
	public NativeList<int2> collisionEvents;

	/// <summary>
	/// Simulate physics of objects in room for dT.
	/// </summary>
	public static void Run(PhysicsSettings settings, Room room, float dT)
	{
		// fetch buffers for the physics job
		var battleObjectCount = room.AllObjects.Count;
		var job = new PhysicsJob()
		{
			dT = dT,

			// objects buffer
			objectCount = battleObjectCount,
			positions = new NativeArray<float2>(battleObjectCount, Allocator.TempJob),
			origPositions = new NativeArray<float2>(battleObjectCount, Allocator.TempJob),
			velocities = new NativeArray<float2>(battleObjectCount, Allocator.TempJob),
			properties = new NativeArray<ColliderProperties>(battleObjectCount, Allocator.TempJob),

			// output collision events buffer
			collisionEvents = new NativeList<int2>(Allocator.TempJob),

			// load settings into the job
			PhysicsPassesPerFrame = settings.Passes,
			BounceAbsorb = settings.BounceAbsorb,
			VelocityDragConstant = settings.VelocityDragConstant,
			VelocityDragRelative = pow(settings.VelocityDragRelative, 1f / settings.Passes),

			// get world boundaries
			LeftEdge = -room.Width / 2,
			RightEdge = room.Width / 2,
			TopEdge = -room.Height / 2,
			BottomEdge = room.Height / 2,
		};

		// load objects into the buffer
		var battleObjects = room.AllObjects;
		for(int i = 0; i < battleObjectCount; i++)
		{
			var obj = battleObjects[i];
			job.positions[i] = obj.Position;
			job.velocities[i] = obj.Velocity;
			job.properties[i] = new ColliderProperties(obj);
		}

		// run it!
		job.Run();

		// apply results to the game. Skip NaNs if any.
		for(int i = 0; i < battleObjectCount; i++)
		{
			var newPosition = job.positions[i];
			if (any(isnan(newPosition))) continue;

			var newVelocity = job.velocities[i];
			if (any(isnan(newVelocity))) continue;

			var obj = battleObjects[i];
			if (!obj.Settings.IsStatic)
			{
				obj.Position = newPosition;
				obj.Velocity = newVelocity;
			}
		}

		// dispatch collected collision events
		foreach(var collisionEvent in job.collisionEvents)
		{
			if (collisionEvent.y < 0)
			{
				battleObjects[collisionEvent.x].OnCollisionWithWall();
			}
			else
			{
				battleObjects[collisionEvent.x].OnCollisionWith(battleObjects[collisionEvent.y]);
				battleObjects[collisionEvent.y].OnCollisionWith(battleObjects[collisionEvent.x]);
			}
		}

		// return buffer memory
		job.collisionEvents.Dispose();
		job.positions.Dispose();
		job.origPositions.Dispose();
		job.velocities.Dispose();
		job.properties.Dispose();
	}

	/// <summary>
	/// Main physics loop.
	/// </summary>
	public void Execute()
	{
		for(int i = 0; i < PhysicsPassesPerFrame; i++)
		{
			UpdateVelocities(dT / PhysicsPassesPerFrame);
			CheckCollisionN2();
		}
	}

	/// <summary>
	/// Update position per velocity, and decrease velocities by drag forces.
	/// </summary>
	private void UpdateVelocities(float dT)
	{
		for(int i = 0; i < objectCount; i++)
		{
			// update position by velocity
			positions[i] += velocities[i] * dT;

			// slow down all velocities by global drag values
			var velocityMagnitude = length(velocities[i]);
			if (velocityMagnitude > 0)
			{
				var newVelocityMagnitude = (velocityMagnitude - VelocityDragConstant * dT) * VelocityDragRelative;
				if (newVelocityMagnitude <= 0)
				{
					velocities[i] = 0;
				}
				else
				{
					velocities[i] *= newVelocityMagnitude / velocityMagnitude;
				}
			}
		}
	}

	/// <summary>
	/// Check all pairs of objects for possible collisions.
	/// </summary>
	private void CheckCollisionN2(int attemptsLeft = 10)
	{
		// snapshot positions at start of the loop. Collision checks will be done agains this snapshot
		positions.CopyTo(origPositions);

		// go n^2
		bool objectsWereMovedFLag = false;
		for(int i = 0; i < objectCount; i++)
		{
			var leftCollider = properties[i];
			for(int j = i + 1; j < objectCount; j++)
			{
				var rightCollider = properties[j];

				// early AABB discard
				var size = leftCollider.Size + rightCollider.Size;
				var delta = origPositions[i] - origPositions[j];
				if (any(abs(delta) > size))
				{
					continue;
				}

				// collision normal and displacement to be calculated
				float displacement = 0f;
				float2 normal = 0f;

				// select proper interaction
				if (leftCollider.Shape == ColliderShape.Circle && rightCollider.Shape == ColliderShape.Circle)
				{
					Circle2CircleCollision(delta, size, ref normal, ref displacement);
				}
				if (leftCollider.Shape == ColliderShape.Circle && rightCollider.Shape == ColliderShape.Rectangle)
				{
					Rect2CircleCollision(delta, size, rightCollider.Size, ref normal, ref displacement);
				}
				if (leftCollider.Shape == ColliderShape.Rectangle && rightCollider.Shape == ColliderShape.Circle)
				{
					Rect2CircleCollision(-delta, size, leftCollider.Size, ref normal, ref displacement);
					normal = -normal;
				}
				if (leftCollider.Shape == ColliderShape.Rectangle && rightCollider.Shape == ColliderShape.Rectangle)
				{
					Rect2RectCollision(delta, size, ref normal, ref displacement);
				}

				// Discard if objects are not colliding
				if (displacement <= 0.001)
				{
					continue;
				}

				// write down that collision between i and j happened.
				RecordObjectCollision(i, j);

				// Displace objects, if objects allow to be moved by physics.
				if (!leftCollider.TriggerOnly && !rightCollider.TriggerOnly && !(leftCollider.IsStatic && rightCollider.IsStatic))
				{
					// special case for equal position (or if system is in impossible situation), randomize push direction
					if (delta.x == 0 && delta.y == 0 || attemptsLeft == 0)
					{
						normal = new Unity.Mathematics.Random(14968749 + (uint)i * 17 + (uint)j * 19).NextFloat2Direction();
					}

					// update position based on the displacement needed
					if (leftCollider.IsStatic)
					{
						positions[j] -= normal * displacement;
					}
					else if (rightCollider.IsStatic)
					{
						positions[i] += normal * displacement;
					}
					else
					{
						// distribute displacement based on the mass ratio.
						var totalForce = normal * displacement;
						var totalMass = leftCollider.Mass + rightCollider.Mass;
						positions[i] += totalForce * (totalMass > 0 ? rightCollider.Mass / totalMass : 0.5f);
						positions[j] -= totalForce * (totalMass > 0 ? leftCollider.Mass / totalMass : 0.5f);
					}

					// redistribute velocities.
					var v1 = max(0, dot(velocities[i], -normal) - BounceAbsorb * leftCollider.Size.x);
					var v2 = max(0, dot(velocities[j], normal) - BounceAbsorb * rightCollider.Size.x);
					var f1 = min(v1 * rightCollider.Mass, 2 * v1 * leftCollider.Mass);
					var f2 = min(v2 * leftCollider.Mass, 2 * v2 * rightCollider.Mass);
					var redistribution = normal * (f1 + f2);
					velocities[i] += redistribution / leftCollider.Mass;
					velocities[j] -= redistribution / rightCollider.Mass;
					objectsWereMovedFLag = true;
				}
			}
		}

		// make sure nothing is pushed outside the walls.
		if (EnforceLevelBoundaries())
		{
			objectsWereMovedFLag = true;
		}

		// if necessary, call loop until everything is resolved
		if (objectsWereMovedFLag && attemptsLeft > 0)
		{
			CheckCollisionN2(attemptsLeft - 1);
		}
	}

	private void Circle2CircleCollision(float2 delta, float2 size, ref float2 normal, ref float displacementNeeded)
	{
		// distance is normalized per axis - 1 means objects are touching
		var distancesq = lengthsq(delta / size);
		if (distancesq < 1)
		{
			normal = normalizesafe(delta);
			displacementNeeded = (1 - sqrt(distancesq));
		}
	}

	private void Rect2CircleCollision(float2 delta, float2 size, float2 rectSize, ref float2 normal, ref float displacementNeeded)
	{
		// uh-oh, corner case!
		if (all(abs(delta) > rectSize))
		{
			var corner = sign(delta) * rectSize;
			var circleSize = size - rectSize;
			var offset = delta - corner;
			normal = normalizesafe(offset);
			displacementNeeded = 1 - length(offset / circleSize);
			return;
		}

		// flat normals. early discard steps makes sure both overlaps axii are below 1f
		var overlap = abs(delta) / size;
		if (overlap.x < overlap.y)
		{
			normal = new float2(0, sign(delta.y));
			displacementNeeded = (1 - overlap.y) * size.y;
		}
		else
		{
			normal = new float2(sign(delta.x), 0);
			displacementNeeded = (1 - overlap.x) * size.x;
		}
	}

	private void Rect2RectCollision(float2 delta, float2 size, ref float2 normal, ref float displacementNeeded)
	{
		var overlap = abs(delta) / size;
		if (overlap.x < overlap.y)
		{
			normal = new float2(0, sign(delta.y));
			displacementNeeded = (1 - overlap.y) * size.y;
		}
		else if (overlap.y < overlap.x)
		{
			normal = new float2(sign(delta.x), 0);
			displacementNeeded = (1 - overlap.x) * size.x;
		}
	}

	/// <summary>
	/// Make sure objects do not leave level boundaries, emit wall collision events.
	/// </summary>
	private bool EnforceLevelBoundaries()
	{
		bool collisionDetected = false;

		for(int i = 0; i < objectCount; i++)
		{
			// ignore static objects.
			if (properties[i].IsStatic)
			{
				continue;
			}

			var size = properties[i].Size;
			var position = positions[i];
			var velocity = velocities[i];
			var collidedWithWall = false;

			// left wall
			var minX = LeftEdge + size.x;
			if (position.x < minX)
			{
				position.x = minX;
				if (velocity.x < 0) { velocity.x = max(0, -velocity.x - size.x * BounceAbsorb); }
				collidedWithWall = true;
			}

			// right wall
			var maxX = RightEdge - size.x;
			if (position.x > maxX)
			{
				position.x = maxX;
				if (velocity.x > 0) { velocity.x = min(0, -velocity.x + size.x * BounceAbsorb); }
				collidedWithWall = true;
			}

			// top wall
			var minY = TopEdge + size.y;
			if (position.y < minY)
			{
				position.y = minY;
				if (velocity.y < 0) { velocity.y = max(0, -velocity.y - size.x * BounceAbsorb); }
				collidedWithWall = true;
			}

			// bottom wall
			var maxY = BottomEdge - size.y;
			if (position.y > maxY)
			{
				position.y = maxY;
				if (velocity.y > 0) { velocity.y = min(0, -velocity.y + size.x * BounceAbsorb); }
				collidedWithWall = true;
			}

			if (collidedWithWall)
			{
				collisionDetected = true;
				velocities[i] = velocity;
				positions[i] = position;
				RecordWallCollision(i);
			}
		}

		return collisionDetected;
	}

	/// <summary>
	/// Adds collision event to the dispatch queue.
	/// </summary>
	private void RecordObjectCollision(int i, int j)
	{
		if (!collisionEvents.Contains(new int2(i, j)))
		{
			collisionEvents.Add(new int2(i, j));
		}
	}

	/// <summary>
	/// Adds collision event to the dispatch queue.
	/// </summary>
	private void RecordWallCollision(int i)
	{
		if (!collisionEvents.Contains(new int2(i, -1)))
		{
			collisionEvents.Add(new int2(i, -1));
		}
	}
}

/// <summary>
/// Immutable collider properties, snapshoted at start of the physics job.
/// </summary>
public readonly struct ColliderProperties
{
	public readonly ColliderShape Shape;
	public readonly float2 Size;
	public readonly float Mass;
	public readonly bool IsStatic;
	public readonly bool TriggerOnly;

	public ColliderProperties(BattleObject obj)
	{
		TriggerOnly = obj is Projectile;
		Shape = obj.Settings.ColliderType;
		Mass = obj.Settings.Mass;
		IsStatic = obj.Settings.IsStatic;
		Size = obj.Size;
	}
}