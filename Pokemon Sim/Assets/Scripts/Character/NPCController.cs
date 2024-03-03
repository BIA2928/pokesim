using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactive
{
    [SerializeField] Dialogue dialogue;
    
    public void Interact()
    {
        Debug.Log("Interacted with NPC");
        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
    }
}
