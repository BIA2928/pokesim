using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BattleDialogue : MonoBehaviour
{
    [SerializeField] Text dialogueText;
    [SerializeField] [Range(20, 70)] int textSpeed;


    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject switchBox;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;
    [SerializeField] Text switchText;
    [SerializeField] Text continueText;

    Color highlightedColor;
    bool isTyping = false;
    bool isBusy;

    public static int charLimit = 75;

    public bool IsTyping => isTyping;
    public bool IsBusy { get => isBusy; set => isBusy = value; }
    private void Start()
    {
        highlightedColor = GlobalSettings.i.HighlightedColorBlue;
    }
    public void SetDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    public IEnumerator TypeDialogue(string dialogue)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (var character in dialogue.ToCharArray())
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(1f / textSpeed);
        }
        // add buffer
        yield return new WaitForSeconds(1f/ (textSpeed * 0.04f));
        isTyping = false;
    }
    
    public IEnumerator ShowDialogue(string dialogue, bool waitForInput = true, bool hideActions = false)
    {
        isBusy = true;
        yield return new WaitForEndOfFrame();
        actionSelector.SetActive(!hideActions);
       

        yield return TypeDialogue(dialogue);
        if (waitForInput)
        {
            Debug.Log("waiting for input");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }
        Debug.Log("Input received");
        isBusy = false;

    }

    public IEnumerator ShowDialogue(Dialogue dialogue, bool waitForInput = true, bool hideActions = false)
    {
        yield return new WaitForEndOfFrame();
        actionSelector.SetActive(!hideActions);
        isBusy = true;

        foreach (var line in dialogue.Lines)
        {
            yield return TypeDialogue(line);
            if (waitForInput)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
                AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
            }
        }
        isBusy = false;


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

    public void EnableChoiceSelector(bool enabled)
    {
        switchBox.SetActive(enabled);
        UpdateChoiceSelection(true);

    }
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = highlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }


        ppText.text = $"PP {move.PP}/{move.Base.Pp}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0){
            ppText.color = Color.red;
        }
        else
        {
            ppText.color = Color.black;
        }
    }

    public void UpdateChoiceSelection(bool switchIsSelected)
    {
        if (switchIsSelected)
        {
            switchText.color = highlightedColor;
            continueText.color = Color.black;
        }
        else
        {
            continueText.color = highlightedColor;
            switchText.color = Color.black;
        }
    }
    

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }

    public void HideActions(bool hide)
    {
        actionSelector.SetActive(!hide);
    }

    public void ClearDialogue()
    {
        dialogueText.text = "";
    }

    public bool IsChoiceBoxEnabled => switchBox.activeSelf;

}
