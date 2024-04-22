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
        int totalWaterChance = serializedObject.FindProperty("totalChanceWater").intValue;

        var style = new GUIStyle
        {
            fontStyle = FontStyle.Bold
        };
        GUILayout.Label($"Total Chance (Grassy Areas) = {totalChance}%", style);
        

        if (totalChance != 100 && totalChance != -1)
            EditorGUILayout.HelpBox("GrassError. Total encounter chance percentage should be equal to 100%", MessageType.Warning);
        GUILayout.Label($"Total Chance (Water Areas) = {totalWaterChance}%", style);
        if (totalWaterChance != 100 && totalWaterChance != -1)
            EditorGUILayout.HelpBox("WaterError. Total encounter chance percentage should be equal to 100% in all areas.", MessageType.Warning);
    }
}
