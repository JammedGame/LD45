using UnityEditor;

public static class ShowMonoScript
{
	public static bool Show(SerializedObject serializedObject)
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
		return serializedObject.ApplyModifiedProperties();
	}

	public static bool DefaultInspector(SerializedObject serializedObject)
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);

		var property = serializedObject.GetIterator();
		property.NextVisible(true);
		while(property.NextVisible(property.isExpanded))
		{
			if (property.displayName == "Script") continue;
			EditorGUILayout.PropertyField(property);
		}
		return serializedObject.ApplyModifiedProperties();
	}
}