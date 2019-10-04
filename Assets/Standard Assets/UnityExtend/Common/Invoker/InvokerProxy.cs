using System.Collections;
using System.Threading;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// MonoBehaviour/EditorApplication.update background for updating Invoker each frame.
/// </summary>
internal class InvokerProxy : MonoBehaviour
{
	void Update() { Invoker.NotifyUpdate(); }
	void LateUpdate() { Invoker.NotifyLateUpdate(); }

	[RuntimeInitializeOnLoadMethod]
	static void Initialize()
	{
		Touch();
	}

	static InvokerProxy instance;
	public static InvokerProxy Instance
	{
		get
		{
			Touch();
			return instance;
		}
	}

	/// <summary>
	/// Assures there exists an instance of this type.
	/// </summary>
	public static void Touch()
	{
		if (instance != null) { return; }
		if (Thread.CurrentThread.ManagedThreadId != 1) { return; }
		if (!Application.isPlaying) { return; }
		instance = FindObjectOfType<InvokerProxy>();
		if (instance != null) { return; }
		instance = new GameObject("Invoker").AddComponent<InvokerProxy>();
	}

	void Awake()
	{
		if (Application.isPlaying)
		{
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			DontDestroyOnLoad(gameObject);
		}

		// There can be only one!
		if (instance != null && instance != this)
		{
			GameObject.Destroy(instance);
			instance = this;
		}
	}

	void OnDestroy()
	{
#if UNITY_EDITOR
		if (PlaymodeListener.IsQuitingPlaymode) { return; }
#endif

		// make sure we have somebody to continue our job if we get destroyed
		if (instance != this) return;
		instance = FindObjectOfType<InvokerProxy>();
		if (instance != null) { return; }
		instance = new GameObject("Invoker").AddComponent<InvokerProxy>();
	}
}

#if UNITY_EDITOR
[InitializeOnLoad]
public static class PlaymodeListener
{
	public static bool IsQuitingPlaymode { get; private set; }
	public static event System.Action OnQuitPlaymode;

	static PlaymodeListener()
	{
		EditorApplication.playModeStateChanged += (x) =>
		{
			if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
			{
				OnQuitPlaymode?.Invoke();
				IsQuitingPlaymode = true;
			}
		};
	}
}
#endif