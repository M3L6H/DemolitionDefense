﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MarketManager))]
public class MarketManagerEditor : Editor
{

    private static readonly GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
    private static readonly GUIContent
        moveUpButtonContent = new GUIContent("\u2191", "move up"),
        moveDownButtonContent = new GUIContent("\u2193", "move down"),
        duplicateButtonContent = new GUIContent("+", "duplicate"),
        deleteButtonContent = new GUIContent("-", "delete");

    private List<bool> showItem;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Menu
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Menu"));

        // Menu entry
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MenuEntryPrefab"));

        // Horizontal line
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // Buttons to add/remove items
        ShowAddRemoveButtons(serializedObject.FindProperty("Items"));

        // List of bools indicating the open/close status of the Waves
        if (showItem == null)
            showItem = new List<bool>();

        // Show each item
        for (int i = 0; i < serializedObject.FindProperty("Items").arraySize; i++)
        {
            EditorGUI.indentLevel += 1;

            // Make sure our list of bools is kept up to size
            if (showItem.Count <= i)
                showItem.Insert(i, false);

            EditorGUILayout.BeginHorizontal();

            // Foldout for waves
            showItem[i] = EditorGUILayout.Foldout(showItem[i], $"Item {i + 1}", true);

            // Buttons to re-order waves
            ShowMoveUpDownButtons(serializedObject.FindProperty("Items"), i);
            EditorGUILayout.EndHorizontal();

            // Rendering each wave
            if (showItem[i] && ((MarketManager)target).Items.Length > 0)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.BeginHorizontal();

                // Labels
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Price", GUILayout.Width(80));
                EditorGUILayout.LabelField("Icon", GUILayout.Width(80));
                EditorGUILayout.LabelField("Rotates", GUILayout.Width(80));
                EditorGUILayout.LabelField("Multi", GUILayout.Width(80));
                EditorGUILayout.EndVertical();

                // Fields
                EditorGUILayout.BeginVertical();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("Price"),
                    GUIContent.none, GUILayout.Width(150f));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("Icon"),
                    GUIContent.none, GUILayout.Width(150f));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("Rotates"),
                    GUIContent.none, GUILayout.Width(150f));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("MultiTile"),
                    GUIContent.none, GUILayout.Width(150f));
                EditorGUILayout.EndVertical();

                // Labels
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Enabled", GUILayout.Width(80));
                EditorGUILayout.LabelField("Tile", GUILayout.Width(80));
                EditorGUILayout.LabelField("Second", GUILayout.Width(80));
                EditorGUILayout.EndVertical();

                // Fields
                EditorGUILayout.BeginVertical();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("Enabled"),
                    GUIContent.none, GUILayout.Width(150f));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("Tile"),
                    GUIContent.none, GUILayout.Width(150f));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("HasSecond"),
                    GUIContent.none, GUILayout.Width(150f));
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

                if (((MarketManager)target).Items[i].Rotates)
                    ShowRotation(serializedObject, i);

                if (((MarketManager)target).Items[i].HasSecond)
                    ShowHasSecond(serializedObject, i);

                if (((MarketManager)target).Items[i].MultiTile)
                    ShowMultiTile(serializedObject, i);

                EditorGUI.indentLevel -= 1;
            }
            EditorGUI.indentLevel -= 1;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static void ShowMultiTile(SerializedObject serializedObject, int i)
    {
        EditorGUILayout.BeginHorizontal();

        // Labels
        EditorGUILayout.LabelField("N", GUILayout.Width(50f));

        // Fields
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("N"),
            GUIContent.none, GUILayout.Width(50f));

        // Labels
        EditorGUILayout.LabelField("S", GUILayout.Width(50f));

        // Fields
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("S"),
            GUIContent.none, GUILayout.Width(50f));

        // Labels
        EditorGUILayout.LabelField("E", GUILayout.Width(50f));

        // Fields
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("E"),
            GUIContent.none, GUILayout.Width(50f));

        // Labels
        EditorGUILayout.LabelField("W", GUILayout.Width(50f));

        // Fields
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("W"),
            GUIContent.none, GUILayout.Width(50f));

        EditorGUILayout.EndHorizontal();
    }

    private static void ShowHasSecond(SerializedObject serializedObject, int i)
    {
        EditorGUILayout.BeginHorizontal();

        // Labels
        EditorGUILayout.LabelField("Icon", GUILayout.Width(80));

        // Fields
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("SecondIcon"),
            GUIContent.none, GUILayout.Width(150f));

        // Labels
        EditorGUILayout.LabelField("Tile", GUILayout.Width(80));

        // Fields
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("SecondTile"),
            GUIContent.none, GUILayout.Width(150f));

        EditorGUILayout.EndHorizontal();
    }

    private static void ShowRotation(SerializedObject serializedObject, int i)
    {
        EditorGUILayout.BeginHorizontal();

        // Labels
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("North", GUILayout.Width(80));
        EditorGUILayout.LabelField("East", GUILayout.Width(80));
        EditorGUILayout.EndVertical();

        // Fields
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("NorthTile"),
            GUIContent.none, GUILayout.Width(150f));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("EastTile"),
            GUIContent.none, GUILayout.Width(150f));
        EditorGUILayout.EndVertical();

        // Labels
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("South", GUILayout.Width(80));
        EditorGUILayout.LabelField("West", GUILayout.Width(80));
        EditorGUILayout.EndVertical();

        // Fields
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("SouthTile"),
            GUIContent.none, GUILayout.Width(150f));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Items").GetArrayElementAtIndex(i).FindPropertyRelative("WestTile"),
            GUIContent.none, GUILayout.Width(150f));
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private static void ShowAddRemoveButtons(SerializedProperty list)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Add/Remove Market Items");
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
