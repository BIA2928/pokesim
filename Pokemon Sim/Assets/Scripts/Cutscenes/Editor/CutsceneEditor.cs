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

        using (var scope = new GUILayout.HorizontalScope()) {
            
            if (GUILayout.Button("MoveActor Action"))
                cS.AddAction(new MoveActorAction());
            else if (GUILayout.Button("TurnActor Action"))
                cS.AddAction(new TurnActorAction());
            else if (GUILayout.Button("TeleportActor Action"))
                cS.AddAction(new TeleportActorAction());
        }
        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Dialogue Action"))
                cS.AddAction(new DialogueAction());
            else if (GUILayout.Button("Fade Action"))
                cS.AddAction(new FadeScreenAction());
        }
        using (var scope = new GUILayout.HorizontalScope())
        {
            if (GUILayout.Button("NPC Interact Action"))
                cS.AddAction(new NPCInteractAction());
            else if (GUILayout.Button("Enable/Disable Object Action"))
                cS.AddAction(new EnableObjectAction());
        }


        base.OnInspectorGUI();
    }
}
