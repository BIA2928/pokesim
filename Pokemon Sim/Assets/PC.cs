using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PC : MonoBehaviour, Interactive
{
    [SerializeField] Sprite pcOnSprite;

    SpriteRenderer spriteRenderer;
    GameController gC;


    public IEnumerator Interact(Transform initiator)
    {
        yield return DialogueManager.Instance.ShowDialogue("Booting up the PC!");
        // Add little animation here
        yield return new WaitForSeconds(0.5f);

        List<string> choices = new List<string>() { "YES", "NO" };
        Dialogue d = new Dialogue() { Lines = { "Would you like the access the PC?" } };
        int selection = 0;
        yield return DialogueManager.Instance.ShowDialogueChoices(d, choices, (i) => selection = i, false);

        if (selection == 0)
        {
            gC.StateMachine.Push(BoxState.i);
        }
        
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gC = FindObjectOfType<GameController>();
        
    }


}
