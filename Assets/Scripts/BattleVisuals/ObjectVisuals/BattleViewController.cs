using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleViewController
{
	readonly List<BattleObject3D> allVisuals = new List<BattleObject3D>();
	readonly Dictionary<BattleObject, BattleObject3D> object2VisualMap = new Dictionary<BattleObject, BattleObject3D>();

	public BattleObject3D GetView(BattleObject obj)
		=> obj != null && object2VisualMap.TryGetValue(obj, out BattleObject3D obj3D) ? obj3D : null;

	public void SyncEverything(float dT)
	{
		for(int i = 0; i < allVisuals.Count; i++)
		{
			try
			{
				allVisuals[i].Sync(dT);
			}
			catch(Exception e)
			{
				if (allVisuals[i] == null)
				{
					allVisuals.RemoveAt(i--);
					continue;
				}

				Debug.LogException(e);
			}
		}
	}

	public void HandleViewEvents(ViewEventsPipe eventPipe)
	{
		foreach(var evt in eventPipe.EventsInQueue)
		{
			try
			{
				OnEvent(evt);
			}
			catch(Exception e)
			{
				Debug.LogException(e);
			}
		}

		// clear
		eventPipe.EventsInQueue.Clear();
	}

	public void OnEvent(ViewEvent evt)
	{
		// special event for object creating.
		if (evt.EventType == ViewEventType.Init)
		{
			InitNewObject(evt.Source);
		}

		// handle events
		var obj3D = GetView(evt.Source);
		if (obj3D != null)
		{
			obj3D.OnViewEvent(evt);

			if (evt.EventType == ViewEventType.End)
			{
				obj3D.Deactivate();
			}
		}
	}

	/// <summary>
	/// Create new 3D representation of the given object.
	/// </summary>
	void InitNewObject(BattleObject obj)
	{
		if (!object2VisualMap.ContainsKey(obj)
			&& BattleObject3D.CreateNewVisual(obj, this) is BattleObject3D newVisual)
		{
			object2VisualMap.Add(obj, newVisual);
			allVisuals.Add(newVisual);
		}
	}

	/// <summary>
	/// Removes 3d object from all collections on dispose.
	/// </summary>
	public void Remove(BattleObject3D obj)
	{
		allVisuals.Remove(obj);
		if (obj != null && obj.Data != null) { object2VisualMap.Remove(obj.Data); }
	}
}