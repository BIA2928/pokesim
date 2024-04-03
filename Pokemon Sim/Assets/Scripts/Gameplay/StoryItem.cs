using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryItem : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Dialogue dialogue;
    public void OnPlayerTrigger(PlayerController player)
    {
        player.StopPlayerMovement();
        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));
    }

    public bool TriggerRepeatedly => false;
}
