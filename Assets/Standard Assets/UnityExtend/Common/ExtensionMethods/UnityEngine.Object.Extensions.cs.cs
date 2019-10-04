using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class UnityEngineObjectExtensions
{
    public static void Destroy(this UnityEngine.Object obj)
    {
        if (obj == null) { return; }

#if UNITY_EDITOR
        // editor destroy
        if (!Application.isPlaying)
        {
            UnityEngine.Object.DestroyImmediate(obj);
            return;
        }
#endif

        UnityEngine.Object.Destroy(obj);
    }

    public static void SetDirtyForUnityEditor(this Object obj)
    {
#if UNITY_EDITOR
        if (obj == null) return;
        EditorUtility.SetDirty(obj);
#endif
    }

    /// <summary>
    /// Use this to safely check null for UnityEngine.Object references stored as plain c# classes.
    /// </summary>
    public static bool IsNullOrDestroyed<T>(this T obj) where T : class
    {
        if (obj is UnityEngine.Object unityObj)
        {
            return unityObj == null;
        }
        else
        {
            return obj == null;
        }
    }
}
