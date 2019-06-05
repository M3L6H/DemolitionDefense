using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{

    private static readonly GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
    private static readonly GUIContent
        moveUpButtonContent = new GUIContent("\u2191", "move up"),
        moveDownButtonContent = new GUIContent("\u2193", "move down"),
        duplicateButtonContent = new GUIContent("+", "duplicate"),
        deleteButtonContent = new GUIContent("-", "delete");

    private List<bool> showWave;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Level map
        EditorGUILayout.PropertyField(serializedObject.FindProperty("LevelMap"));

        // Spawn speed
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Spawn Speed (in seconds)");
        ((WaveManager)target).SpawnSpeed = EditorGUILayout.Slider(((WaveManager)target).SpawnSpeed, 0.1f, 1f);
        EditorGUILayout.EndHorizontal();

        // Wave cooldown
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Wave Cooldown (in seconds)");
        ((WaveManager)target).WaveCooldown = EditorGUILayout.Slider(((WaveManager)target).WaveCooldown, 1f, 10f);
        EditorGUILayout.EndHorizontal();

        // List of bools indicating the open/close status of the Waves
        if (showWave == null)
            showWave = new List<bool>();

        // Horizontal line
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Buttons to add/remove waves
        ShowAddRemoveButtons(serializedObject.FindProperty("Waves"));
        // Show each wave
        for (int i = 0; i < serializedObject.FindProperty("Waves").arraySize; i++)
        {
            EditorGUI.indentLevel += 1;
            EditorGUILayout.BeginHorizontal();
            // Make sure our list of bools is kept up to size
            if (showWave.Count <= i)
                showWave.Insert(i, false);
            // Foldout for waves
            showWave[i] = EditorGUILayout.Foldout(showWave[i], $"Wave {i + 1}", true);
            // Buttons to re-order waves
            ShowMoveUpDownButtons(serializedObject.FindProperty("Waves"), i);
            EditorGUILayout.EndHorizontal();
            // Rendering each wave
            if (showWave[i] && ((WaveManager)target).Waves.Length > 0)
            {
                EditorGUI.indentLevel += 1;
                ShowWaveAddRemoveButtons(serializedObject.FindProperty("Waves").GetArrayElementAtIndex(i));
                for (int j = 0; j < serializedObject.FindProperty("Waves").GetArrayElementAtIndex(i).FindPropertyRelative("Enemies").arraySize; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Enemy {j + 1}", GUILayout.Width(75));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Waves").GetArrayElementAtIndex(i).FindPropertyRelative("Enemies").GetArrayElementAtIndex(j),
                                                  GUIContent.none);
                    EditorGUILayout.LabelField($"Source {j + 1}", GUILayout.Width(75));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Waves").GetArrayElementAtIndex(i).FindPropertyRelative("Sources").GetArrayElementAtIndex(j),
                                                  GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel -= 1;
            }
            EditorGUI.indentLevel -= 1;
        }
        serializedObject.ApplyModifiedProperties();
    }

    private static void ShowWaveAddRemoveButtons(SerializedProperty wave)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Add/Remove Enemies in Wave");
        if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
        {
            wave.FindPropertyRelative("Enemies").InsertArrayElementAtIndex(wave.FindPropertyRelative("Enemies").arraySize);
            wave.FindPropertyRelative("Sources").InsertArrayElementAtIndex(wave.FindPropertyRelative("Sources").arraySize);
        }
        if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth) && wave.FindPropertyRelative("Enemies").arraySize > 0)
        {
            // Unity empties the field before removing it, but we want full removal, so we remove a second time when necessary
            int oldSize = wave.FindPropertyRelative("Enemies").arraySize;
            wave.FindPropertyRelative("Enemies").DeleteArrayElementAtIndex(wave.FindPropertyRelative("Enemies").arraySize - 1);
            if (wave.FindPropertyRelative("Enemies").arraySize == oldSize)
                wave.FindPropertyRelative("Enemies").DeleteArrayElementAtIndex(wave.FindPropertyRelative("Enemies").arraySize - 1);

            oldSize = wave.FindPropertyRelative("Sources").arraySize;
            wave.FindPropertyRelative("Sources").DeleteArrayElementAtIndex(wave.FindPropertyRelative("Sources").arraySize - 1);
            if (wave.FindPropertyRelative("Sources").arraySize == oldSize)
                wave.FindPropertyRelative("Sources").DeleteArrayElementAtIndex(wave.FindPropertyRelative("Sources").arraySize - 1);
        }
        EditorGUILayout.EndHorizontal();
    }

    private static void ShowAddRemoveButtons(SerializedProperty list)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Add/Remove Waves");
        if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
        {
            list.InsertArrayElementAtIndex(list.arraySize);
        }
        if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth) && list.arraySize > 0)
            list.DeleteArrayElementAtIndex(list.arraySize - 1);
        EditorGUILayout.EndHorizontal();
    }

    private static void ShowMoveUpDownButtons(SerializedProperty list, int index)
    {
        if (GUILayout.Button(moveDownButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
            list.MoveArrayElement(index, index + 1);
        if (GUILayout.Button(moveUpButtonContent, EditorStyles.miniButtonRight, miniButtonWidth) && index > 0)
            list.MoveArrayElement(index, index - 1);
    }

}
