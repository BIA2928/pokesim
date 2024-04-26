using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NPCController))]
public class NPCControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        NPCController nPCController = target as NPCController;
        if (GUILayout.Button("Add walk"))
            nPCController.AddMovement(new MoveVector());
        else if (GUILayout.Button("Add Turn"))
            nPCController.AddMovement(new Turn());

        base.OnInspectorGUI();
    }
}
