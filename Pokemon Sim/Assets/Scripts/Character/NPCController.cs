using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactive
{
    [SerializeField] Dialogue dialogue;
    public void Interact()
    {
        Debug.Log("Interacted with NPC");
        DialogueManager.Instance.ShowDialogue(dialogue);
    }
}
