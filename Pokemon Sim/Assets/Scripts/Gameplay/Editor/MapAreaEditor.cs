using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        int totalChance = serializedObject.FindProperty("totalChance").intValue;

        var style = new GUIStyle
        {
            fontStyle = FontStyle.Bold
        };
        GUILayout.Label($"Total Chance = {totalChance}%", style);

        if (totalChance != 100)
            EditorGUILayout.HelpBox("Total chance percentage should be equal to 100%", MessageType.Warning);
    }
}
