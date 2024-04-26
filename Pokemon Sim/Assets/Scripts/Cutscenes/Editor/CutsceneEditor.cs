using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Cutscene cS = target as Cutscene;
        if (GUILayout.Button("Add Dialogue Action"))
            cS.AddAction(new DialogueAction());
        else if (GUILayout.Button("Add Move Actor Action"))
            cS.AddAction(new MoveActorAction());

        base.OnInspectorGUI();
    }
}
