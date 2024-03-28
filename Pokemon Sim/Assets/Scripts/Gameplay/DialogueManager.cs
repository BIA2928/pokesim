using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] GameObject dialogueBox;
    [SerializeField] Text dialogueText;
    [Range(5, 50)] [SerializeField] int textSpeed;

    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;

    private int currentLine = 0;
    Dialogue currDialogue;
    bool isTyping;

    private Action onDialogueFinished;

    public bool IsShowing { get; private set; }
    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShowDialogue(Dialogue dialogue, Action onFinished=null)
    {
        yield return new WaitForEndOfFrame();
        IsShowing = true;
        OnShowDialogue?.Invoke();
        currDialogue = dialogue;

        onDialogueFinished = onFinished;

        dialogueBox.SetActive(true);
        StartCoroutine(TypeDialogue(dialogue.Lines[0]));
    }

    public IEnumerator ShowDialogue(string text, bool waitForInput=true)
    {
        //yield return new WaitForEndOfFrame();
        IsShowing = true;

        dialogueBox.SetActive(true);
        yield return TypeDialogue(text);

        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
        }

        dialogueBox.SetActive(false);
        IsShowing = false;
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

        isTyping = false;
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if (currentLine < currDialogue.Lines.Count)
            {
                StartCoroutine(TypeDialogue(currDialogue.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialogueBox.SetActive(false);
                onDialogueFinished?.Invoke();
                OnCloseDialogue?.Invoke();
            }
        }
    }
}
