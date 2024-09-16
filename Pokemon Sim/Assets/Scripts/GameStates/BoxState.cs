using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class BoxState : State<GameController>
{
    [SerializeField] BoxUI boxUI;
    [SerializeField] PointerSelector pointer;
    GameController gC;
    public static BoxState i { get; private set; }

    PokemonParty party;
    PokemonStorage boxes;

    private void Awake()
    {
        i = this;
        party = PokemonParty.GetPlayerParty();
        boxes = PokemonStorage.GetPlayerStorageBoxes();

    }

    public override void EnterState(GameController owner)
    {
        boxUI.OnBack += OnBack;
        boxUI.OnPokemonSelected += OnPokemonSelected;
        gC = owner;
        StartCoroutine(FadeIntoBox());
    }

    public override void Execute()
    {
        boxUI.HandleUpdate();
    }

    public override void ExitState()
    {
        boxUI.OnBack -= OnBack;
        boxUI.OnPokemonSelected -= OnPokemonSelected;
        StartCoroutine(FadeOutOfBox());
    }

    void OnPokemonSelected()
    {
        StartCoroutine(SelectionRoutine());
    }

    public IEnumerator SelectionRoutine()
    {
        if (boxUI.HoldingPokemon)
        {
            int selectedChoice = 0;
            Dialogue d = new Dialogue() { Lines = { $"{boxUI.HeldPokemon.Base.Name} is selected." } };
            List<string> choices = new List<string>() { "RELEASE", "CANCEL" };
            if (boxUI.HoverSpotFull())
            {
                choices.Insert(0, "SWITCH");
            }
            else
            {
                choices.Insert(0, "PLACE");
            }
            yield return DialogueManager.Instance.ShowPCDialogueChoices(d, choices, (i) => selectedChoice = i, false);

            if (selectedChoice == 1)
            {
                // Release, not gonnna bother with this logic rn
            }
            else if (selectedChoice == 0)
            {
                PokemonStorage boxes = PokemonStorage.GetPlayerStorageBoxes();
                // If empty, place, otherwise place and grab
                if (boxUI.HoverSpotFull()) 
                { 
                    if (boxUI.SelectionIndex > 36)
                    {
                        // In party
                        Pokemon temp = party.PokemonList[boxUI.SelectionIndex - 38];
                        yield return pointer.DropPokemon();
                        party.PokemonList[boxUI.SelectionIndex - 38] = boxUI.HeldPokemon;
                        boxUI.HeldPokemon = temp;
                        yield return pointer.HoldPokemon(temp);
                        //boxUI.SetPartyData();
                    }
                    else
                    {
                        
                        Pokemon temp = boxes.GetPokemon(boxUI.BoxSelection, boxUI.SelectionIndex);
                        boxes.AddPokemon(boxUI.HeldPokemon, boxUI.BoxSelection, boxUI.SelectionIndex);
                        yield return pointer.DropPokemon();
                        //boxUI.FillCurrentSpot();
                        boxUI.HeldPokemon = temp;
                        yield return pointer.HoldPokemon(temp);
 
                    }
                }
                else
                {
                    if (boxUI.SelectionIndex > 36)
                    {
                        // In party, so add to party 
                        PokemonParty.GetPlayerParty().AddPokemon(boxUI.HeldPokemon);
                        yield return pointer.DropPokemon();
                        boxUI.HeldPokemon = null;
                        //boxUI.SetPartyData();
                    }
                    else
                    {
                        // In box, so add to box spot at boxIndex, spotIndex
                        boxes.AddPokemon(boxUI.HeldPokemon, boxUI.BoxSelection, boxUI.SelectionIndex);
                        yield return pointer.DropPokemon();
                        //boxUI.FillCurrentSpot();
                        boxUI.HeldPokemon = null;
                        
                    }
                }
            }
        }
        else
        {
            Pokemon pokemon = boxUI.GetHoverPokemon();
            bool inBox = boxUI.InBox();
            int selectedChoice = 0;
            Dialogue d = new Dialogue() { Lines = { $"{pokemon.Base.Name} is selected" } };
            List<string> choices = new List<string>() { "MOVE", "RELEASE", "CANCEL" };
            yield return DialogueManager.Instance.ShowPCDialogueChoices(d, choices, (i) => selectedChoice = i, false);
            if (selectedChoice == 0)
            {
                if (party.PokemonList.Count == 1)
                {
                    yield return DialogueManager.Instance.ShowPCDialogue(new Dialogue() { Lines = { "That's your last pokemon!" } });
                    yield break;
                }
                //Move
                if (inBox)
                {
                    //remove from box spot, update boxUI, add to held pokemon, update pointer
                    boxUI.HeldPokemon = boxes.TakePokemon(boxUI.BoxSelection, boxUI.SelectionIndex);
                    yield return pointer.HoldPokemon(boxUI.HeldPokemon);
                    //boxUI.EmptyCurrentSpot();
                }
                else
                {
                    // remove from party spot (unless only pokemon in party), shuffle party, update party, update pointer, update heldpokemon
                    boxUI.HeldPokemon = party.TakePokemon(boxUI.SelectionIndex - 38);
                    yield return pointer.HoldPokemon(boxUI.HeldPokemon);
                    //boxUI.ShuffleParty();
                }
            }
            else if (selectedChoice == 1)
            {
                // release
            }
            //else cancel, do nothing
        }
    }

    void OnBack() 
    {
        if (boxUI.HoldingPokemon)
            StartCoroutine(CantLeave());
        else
            StartCoroutine(AskLeavePC());

    }

    public IEnumerator CantLeave()
    {
        yield return DialogueManager.Instance.ShowCantLeaveBoxDialogue();
    }

    public IEnumerator AskLeavePC()
    {
        int selectedChoice = 0;
        Dialogue dialogue = new Dialogue() { Lines = { "Continue box operations?" } };
        yield return DialogueManager.Instance.ShowPCDialogueChoices(dialogue, new List<string>() { "Yes", "No" }, (i) => selectedChoice = i, false);
        if (selectedChoice == 1)
        {
            gC.StateMachine.Pop();
        }
    }

    public IEnumerator FadeIntoBox()
    {
        yield return Fader.instance.FadeIn(0.4f);
        boxUI.gameObject.SetActive(true);
        yield return Fader.instance.FadeOut(0.4f);
        boxUI.UpdateHoverSelection(0,0);
    }

    public IEnumerator FadeOutOfBox()
    {
        yield return Fader.instance.FadeIn(0.4f);
        boxUI.gameObject.SetActive(false);
        yield return Fader.instance.FadeOut(0.4f);
    }

}
