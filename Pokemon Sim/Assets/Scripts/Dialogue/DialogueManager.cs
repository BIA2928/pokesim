using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] GameObject dialogueBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogueText;
    [Range(5, 50)] [SerializeField] int textSpeed;

    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;

    public static int CHARACTER_LIMIT = 78;
    bool isTyping;


    public bool IsShowing { get; private set; }
    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public void CloseDialogue()
    {
        IsShowing = false;
        dialogueBox.SetActive(false);
    }

    public IEnumerator ShowDialogue(Dialogue dialogue, bool waitForInput = true)
    {
        yield return new WaitForEndOfFrame();
        AudioManager.i.PlaySFX(AudioID.UISelect);
        IsShowing = true;
        OnShowDialogue?.Invoke();
        dialogueBox.SetActive(true);

        foreach (var line in dialogue.Lines)
        {
            yield return TypeDialogue(line);
            if (waitForInput)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
                AudioManager.i.PlaySFX(AudioID.UISelect);
            }
                

        }
        dialogueBox.SetActive(false);
        IsShowing = false;
        OnCloseDialogue?.Invoke();
    }

    public IEnumerator ShowDialogue(string text, bool waitForInput = true, bool autoClose = true)
    {
        yield return new WaitForEndOfFrame();
        AudioManager.i.PlaySFX(AudioID.UISelect);
        IsShowing = true;

        dialogueBox.SetActive(true);
        yield return TypeDialogue(text);

        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
            AudioManager.i.PlaySFX(AudioID.UISelect);
        }

        if (autoClose)
            CloseDialogue();
        OnCloseDialogue?.Invoke();
    }

    public IEnumerator ShowDialogueChoices(Dialogue d, List<string> choices, Action<int> onChoicesSelected, bool waitForInput = true)
    {
        yield return new WaitForEndOfFrame();
        AudioManager.i.PlaySFX(AudioID.UISelect);
        IsShowing = true;
        OnShowDialogue?.Invoke();
        dialogueBox.SetActive(true);

        for (int i = 0; i < d.Lines.Count; i++)
        {
            yield return TypeDialogue(d.Lines[i]);
            if (waitForInput)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
                AudioManager.i.PlaySFX(AudioID.UISelect);
            }
            else
            {
                if (i != d.Lines.Count - 1)
                {
                    // If not last, wait for input
                    yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
                    AudioManager.i.PlaySFX(AudioID.UISelect);
                }
            }
                
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoicesSelected);
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
        AudioManager.i.PlaySFX(AudioID.UISelect);
        IsShowing = true;
        OnShowDialogue?.Invoke();


        dialogueBox.SetActive(true);

        for (int i = 0;  i < dialogue.Lines.Count; i++)
        {
            yield return TypeDialogue(dialogue.Lines[i]);
            if (i == dialogue.Lines.Count - 1)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
                AudioManager.i.PlaySFX(AudioID.UISelect);
            }
                
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

    /// <summary>
    /// Only to be used with item received template dialogues. Expects 2 line dialogue
    /// </summary>
    /// <param name="dialogue"></param>
    /// <param name="audioID"></param>
    /// <returns></returns>
    private IEnumerator ShowDialogueWithSFX(Dialogue dialogue, AudioID audioID)
    {
        if (dialogue.Lines.Count > 2)
        {
            yield return ShowDialogue(dialogue);
        }
        yield return new WaitForEndOfFrame();
        AudioManager.i.PlaySFX(AudioID.UISelect);
        IsShowing = true;
        OnShowDialogue?.Invoke();
        dialogueBox.SetActive(true);

        for (int i = 0; i < dialogue.Lines.Count; i++)
        {
            if (i != 1)
            {
                AudioManager.i.PlaySFX(audioID);
            }
            yield return TypeDialogue(dialogue.Lines[i]);

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X));
            AudioManager.i.PlaySFX(AudioID.UISelect);
        }
        dialogueBox.SetActive(false);
        IsShowing = false;
        OnCloseDialogue?.Invoke();
    }
    public IEnumerator ShowItemReceivedDialogue(ItemBase itemBase, int count = 1)
    {
        Dialogue d = new Dialogue();
        if (count > 1)
        {
            d.Lines.Add($"You obtained {count} {itemBase.Name}s!");
            d.Lines.Add($"You put away the {itemBase.Name}s in the {Inventory.GetPocketForItem(itemBase)} pocket.");
        }
        else
        {
            //if itemBase is keyItem
            if(Vowels.Contains(itemBase.Name.ToCharArray()[0]))
                d.Lines.Add($"You obtained an {itemBase.Name}!");
            else if (itemBase is TmItem tmHm)
            {
                if (tmHm.IsHM)
                    d.Lines.Add($"You obtained an {itemBase.Name}: {tmHm.Move.Name}!");
                else
                    d.Lines.Add($"You obtained a {itemBase.Name}: {tmHm.Move.Name}");
            }
            else
                d.Lines.Add($"You obtained a {itemBase.Name}!");
            d.Lines.Add($"You put away the {itemBase.Name} in the {Inventory.GetPocketForItem(itemBase)} pocket.");
        }
        AudioID audio = AudioID.ItemReceived;
        if (itemBase is TmItem)
            audio = AudioID.TMReceived;
        else if (itemBase is KeyItem)
            audio = AudioID.KeyItemReceived;
        yield return ShowDialogueWithSFX(d, audio);

    }

    public IEnumerator ShowPokemonReceivedDialogue(PokemonBase pokemon, bool partyFull=false)
    {
        Dialogue dialogue = new Dialogue();
        dialogue.AddLine($"You obtained {pokemon.Name}!");
        if (partyFull)
            dialogue.AddLine($"{pokemon.Name} was stored in Box 1.");
        else
            dialogue.AddLine($"{pokemon.Name} was added to the party.");
        yield return ShowDialogueWithSFX(dialogue, AudioID.ObtainedPoke);
    }

    public IEnumerator ShowItemPickupDialogue(ItemBase itemBase, int count = 1)
    {
        Dialogue d = new Dialogue();
        if (count > 1)
        {
            d.Lines.Add($"You found {count} {itemBase.Name}s!");
            d.Lines.Add($"The {itemBase.Name}s were added to the inventory.");
        }
        else
        {
            //if itemBase is keyItem
            // if word needs to be proceed by an not a (an apple, an object, etc)
            if (Vowels.Contains(itemBase.Name.ToCharArray()[0]))
                d.Lines.Add($"You found an {itemBase.Name}!");
            else if (itemBase is TmItem tmHm)
            {
                if (tmHm.IsHM)
                    d.Lines.Add($"You found an {itemBase.Name}: {tmHm.Move.Name}!");
                else
                    d.Lines.Add($"You found a {itemBase.Name}: {tmHm.Move.Name}");
            }
            else
                d.Lines.Add($"You found a {itemBase.Name}!");
            d.Lines.Add($"The {itemBase.Name} was added to the inventory.");
        }

        yield return ShowDialogueWithSFX(d, AudioID.ItemReceived);
    }

    public IEnumerator ShowItemGivenDialogue(ItemBase itemBase, int count = 1)
    {
        Dialogue d = new Dialogue();
        if (count == 1)
        {
            if (Vowels.Contains(itemBase.Name.ToCharArray()[0]))
                d.Lines.Add($"You handed over an {itemBase.Name}!");
            else
                d.Lines.Add($"You handed over a {itemBase.Name}!");
        }
        else
        {
            d.Lines.Add($"You handed over {count} {itemBase.Name}s");
        }

        yield return ShowDialogue(d);
    }

    public IEnumerator ShowItemUsedDialogue(ItemBase item)
    {
        Dialogue d = new Dialogue();
        d.Lines.Add($"You used the {item.Name}");
        yield return ShowDialogue(d);
    }

    public IEnumerator ShowCantUseDialogue()
    {
        Dialogue dialogue = new Dialogue();
        dialogue.Lines.Add($"Professor Rowan's words echo in your ears...");
        dialogue.Lines.Add($"There's a time and place for everything!\nBut not now.");
        yield return ShowDialogue(dialogue);
    }

    public IEnumerator ShowPreEvolutionDialogue(string pokemonName)
    {
        Dialogue d = new Dialogue();
        d.Lines.Add("...");
        d.Lines.Add("What's this?");
        d.Lines.Add($"{pokemonName} is evolving!");
        yield return ShowDialogue(d);
        yield return new WaitForSeconds(1f);
    }

    public IEnumerator ShowPostEvolutionDialogue(string oldName, string newName)
    {
        yield return ShowDialogue($"Your {oldName} evolved into {newName}!");
    }

    public void HandleUpdate()
    {
        
    }

    public static List<char> Vowels = new List<char>(){ 'A', 'E', 'I', 'O', 'U'};
}


