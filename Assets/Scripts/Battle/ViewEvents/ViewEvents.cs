using System.Collections.Generic;

/// <summary>
/// View event types.
/// </summary>
public enum ViewEventType
{
	Init = 0,
	End = 1
}

/// <summary>
/// Single view event, emitted by Source object, to be handled later by its 3d object.
/// </summary>
public readonly struct ViewEvent
{
	public readonly BattleObject Source;
	public readonly ViewEventType EventType;
	public readonly object Data;

	public ViewEvent(BattleObject source, ViewEventType eventType, object data = null)
	{
		Source = source;
		EventType = eventType;
		Data = data;
	}
}

/// <summary>
/// Message queue for various view events that need to be handled by the unity layer.
/// </summary>
public class ViewEventsPipe
{
	public readonly List<ViewEvent> EventsInQueue = new List<ViewEvent>();

	public void SendEvent(BattleObject source, ViewEventType eventType, object data = null)
	{
		EventsInQueue.Add(new ViewEvent(source, eventType, data));
	}
}