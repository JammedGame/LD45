using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Invoker
{
	public static Action Update;
	public static Action LateUpdate;
	public static Action EndOfFrame;

	static readonly List<Action> invokeListBuffer = new List<Action>();
	static readonly List<Action> invokeList = new List<Action>();

	internal static void NotifyUpdate()
	{
		ExecuteInvokedActions();
		Update?.Invoke();
	}

	internal static void NotifyLateUpdate()
	{
		LateUpdate?.Invoke();
		CollectInvokedActions();
	}

	internal static void NotifyEndOfFrame()
	{
		EndOfFrame?.Invoke();
	}

	public static void Invoke(Action action, bool skipTouchingProxy = false)
	{
		if (action == null) { return; }
		invokeListBuffer.Add(action);
		if (skipTouchingProxy) { return; }
		InvokerProxy.Touch();
	}

	public static Coroutine StartCoroutine(this IEnumerator routine)
	{
		return InvokerProxy.Instance.StartCoroutine(routine);
	}

	static void CollectInvokedActions()
	{
		if (invokeListBuffer.Count <= 0) { return; }
		invokeList.AddRange(invokeListBuffer);
		invokeListBuffer.Clear();
	}

	static void ExecuteInvokedActions()
	{
		var count = invokeList.Count;
		if (count <= 0) { return; }

		for (int i = 0; i < count; i++)
		{
			try { invokeList[i](); }
			catch (Exception e) { Debug.LogException(e); }
		}

		invokeList.Clear();
	}
}