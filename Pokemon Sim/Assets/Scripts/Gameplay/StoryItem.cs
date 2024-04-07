using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryItem : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] Dialogue dialogue;
    [SerializeField] Vector2 movementDirection;
    public void OnPlayerTrigger(PlayerController player)
    {
        player.StopPlayerMovement();
        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue));

        if (movementDirection != Vector2.zero)
        {
            StartCoroutine(WaitForDialogueFinishThenMove(player));
        }
    }

    public IEnumerator WaitForDialogueFinishThenMove(PlayerController player)
    {
        yield return new WaitUntil(() => DialogueManager.Instance.IsShowing == false);
        yield return player.ForceMove(movementDirection);
    }
    public bool TriggerRepeatedly => false;
}
