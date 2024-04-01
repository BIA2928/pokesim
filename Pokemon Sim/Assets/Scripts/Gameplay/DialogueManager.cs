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


    bool isTyping;


    public bool IsShowing { get; private set; }
    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ShowDialogue(Dialogue dialogue)
    {
        yield return new WaitForEndOfFrame();
        IsShowing = true;
        OnShowDialogue?.Invoke();
        dialogueBox.SetActive(true);

        foreach (var line in dialogue.Lines)
        {
            yield return TypeDialogue(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
        }
        dialogueBox.SetActive(false);
        IsShowing = false;
        OnCloseDialogue?.Invoke();
    }

    public IEnumerator ShowDialogue(string text, bool waitForInput = true)
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
        OnCloseDialogue?.Invoke();
    }

    /// <summary>
    /// Shows multi-line dialogue, but will wait until final line before waiting for input
    /// </summary>
    /// <param name="dialogue"></param>
    /// <param name="onFinished"></param>
    /// <returns></returns>
    public IEnumerator ShowDialogueContinuous(Dialogue dialogue, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame();
        IsShowing = true;
        OnShowDialogue?.Invoke();


        dialogueBox.SetActive(true);

        for (int i = 0;  i < dialogue.Lines.Count; i++)
        {
            yield return TypeDialogue(dialogue.Lines[i]);
            if (i == dialogue.Lines.Count - 1)
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
            else
                yield return new WaitUntil(() => isTyping == false);
        }
        dialogueBox.SetActive(false);
        IsShowing = false;
        OnCloseDialogue?.Invoke();
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
        
    }
}
