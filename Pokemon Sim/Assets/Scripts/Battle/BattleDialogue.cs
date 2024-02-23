using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleDialogue : MonoBehaviour
{
    [SerializeField] Text dialogueText;
    [SerializeField] int textSpeed;

    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;


    public void SetDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    public IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach (var character in dialogue.ToCharArray())
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(1f / textSpeed);
        }
    }

    public void EnableDialogueText(bool enabled)
    {
        dialogueText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }


}
