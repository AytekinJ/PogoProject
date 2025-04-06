using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Platform))]
public class PlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Platform platform = (Platform)target;

        serializedObject.Update();

        platform.movingPlatform = EditorGUILayout.Toggle("Moving Platform", platform.movingPlatform);

        if (platform.movingPlatform)
        {
            SerializedProperty usePositions = serializedObject.FindProperty("usePositions");
            SerializedProperty useLength = serializedObject.FindProperty("useLength");

            EditorGUILayout.PropertyField(usePositions);
            EditorGUILayout.PropertyField(useLength);

            if (usePositions.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("startPosition"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("endPosition"));
            }

            if (useLength.boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("moveLength"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("vertical"));
            }
        }

            EditorGUILayout.Space(5);

        // breakablePlatform değişkeni
        platform.breakablePlatform = EditorGUILayout.Toggle("Breakable Platform", platform.breakablePlatform);
        if (platform.breakablePlatform)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("duration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("delay"));
        }

        EditorGUILayout.Space(5);

        // jumpSwitch değişkeni (tek başına)
        platform.jumpSwitch = EditorGUILayout.Toggle("Jump Switch", platform.jumpSwitch);
        if (platform.jumpSwitch)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("matchedPlatform"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("matched"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}


