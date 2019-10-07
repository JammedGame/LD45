using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 3D representation of the BattleObject.
/// </summary>
public class BattleObject3D : MonoBehaviour
{
	/// <summary>
	/// Seconds needed to display death animation after gameplay destruction.
	/// If 0, visuals will instantly disappear.
	/// </summary>
	public float DeathAnimationDuration;

	public GameObject OnDeathEffect;
	public float OnDeathEffectDuration;

	/// <summary>
	/// Object being rendered by this 3d object.
	/// </summary>
    public BattleObject Data { get; private set; }

	/// <summary>
	/// Controller in charge of syncing battle simulation layer with the view layer.
	/// </summary>
	public BattleViewController ViewController { get; private set; }

	/// <summary>
	/// Animator for this object;
	/// </summary>
	public Animator Animator { get; private set; }

	/// <summary>
	/// Create a new 3D representation of the 3D object.
	/// </summary>
	public static BattleObject3D CreateNewVisual(BattleObject data, BattleViewController viewController)
	{
		var prefab = data.Settings.LoadVisuals();
		if (prefab == null)
		{
			Debug.LogWarning($"Failed to find prefab for {data.Settings}", data.Settings);
			return null;
		}

		var newObject = BattleObject3D.Instantiate<BattleObject3D>(prefab);
		newObject.Data = data;
		newObject.Animator = newObject.gameObject.GetComponentInChildren<Animator>();
		newObject.ViewController = viewController;
		newObject.Init(data);
		newObject.Sync(0);

		if (newObject.Animator) newObject.Animator.logWarnings = false;

		return newObject;
	}

	public virtual void Init(BattleObject data) {}

	/// <summary>
	/// Sync method to be executed each frame.
	/// </summary>
	public virtual void Sync(float dT)
	{
		transform.position = Data.Position3D;
		SyncFlip();
	}

	public void SyncFlip()
	{
		var sign = math.sign(Data.Velocity.x);
		if (sign == 0 || Data.Settings.IsStatic) { return; }

		var scale = transform.localScale;
		scale.x = math.abs(scale.x) * sign;
		transform.localScale = scale;
	}

	/// <summary>
	/// Handles one view event raised by my object.
	/// </summary>
	public void OnViewEvent(ViewEvent evt)
	{
	}

	public void Deactivate()
	{
		if (Animator != null)
		{
			Animator.SetTrigger("End");
		}

		if (OnDeathEffect != null)
		{
			var instance = GameObject.Instantiate(OnDeathEffect, transform.position, Quaternion.identity);
			Destroy(instance, OnDeathEffectDuration);
		}

		Invoke("Dispose", DeathAnimationDuration);
	}

	public void Dispose()
	{
		try
		{
			if (ViewController != null)
				ViewController.Remove(this);
		}
		finally
		{
			Destroy(gameObject);
		}
	}
}
