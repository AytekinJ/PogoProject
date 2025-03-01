using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Platform))]
public class PlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Platform platform = (Platform)target;

        serializedObject.Update();

        // movingPlatform değişkeni
        platform.movingPlatform = EditorGUILayout.Toggle("Moving Platform", platform.movingPlatform);
        if (platform.movingPlatform)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("startPosition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("endPosition"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveLength"));
        }

        EditorGUILayout.Space(5);

        // breakablePlatform değişkeni
        platform.breakablePlatform = EditorGUILayout.Toggle("Breakable Platform", platform.breakablePlatform);
        if (platform.breakablePlatform)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("delay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isInteracted"));
        }

        EditorGUILayout.Space(5);

        // jumpSwitch değişkeni (tek başına)
        platform.jumpSwitch = EditorGUILayout.Toggle("Jump Switch", platform.jumpSwitch);
        if (platform.jumpSwitch)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("matchedPlatform"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}


