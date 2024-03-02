using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] GameObject dialogueBox;
    [SerializeField] Text dialogueText;
    [Range(5, 50)] [SerializeField] int textSpeed;

    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        dialogueBox.SetActive(true);
        StartCoroutine(TypeDialogue(dialogue.Lines[0]));

    }

    public IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach (var character in dialogue.ToCharArray())
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(1f / textSpeed);
        }
        // add buffer
        
    }
}
