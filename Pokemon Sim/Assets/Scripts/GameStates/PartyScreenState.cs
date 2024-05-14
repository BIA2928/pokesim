using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class PartyScreenState : State<GameController>
{
    [SerializeField] PartyScreen partyScreen;

    public Pokemon SelectedPokemon { get; private set; }
    public static PartyScreenState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gC;
    public override void EnterState(GameController owner)
    {
        SelectedPokemon = null;
        gC = owner;
        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;
        InventoryState prev = gC.StateMachine.GetPreviousState() as InventoryState;
        if (prev != null && prev.InTMPocket())
            partyScreen.ShowIfTMUsable(prev.GetSelectedItem() as TmItem);
         // use set message 
    }

    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }

    public override void ExitState()
    {
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
        partyScreen.ClearMemberSlotMessages();
    }

    void OnPokemonSelected(int selection)
    {
        StartCoroutine(EnterPokemonSelectedState(selection));
    }


    IEnumerator EnterPokemonSelectedState(int selection)
    {
        
        var prev = gC.StateMachine.GetPreviousState();
        if (prev is InventoryState)
        {
            StartCoroutine(EnterUsingItemState());
        }
        else if (prev is BattleState)
        {
            BattleSystem bS = (prev as BattleState).BattleSystem;


            DynamicMenuState.i.MenuItems = new List<string>() { "Shift", "Summary", "Cancel" };
            yield return gC.StateMachine.PushAndWait(DynamicMenuState.i);
            if (DynamicMenuState.i.SelectedItem == 0)
            {
                // Send out
                var selectedMember = partyScreen.SelectedMember;
                if (selectedMember.HP <= 0)
                {
                    partyScreen.SetMessageText($"{selectedMember.Base.Name} has fainted and cannot battle.");
                    yield break;
                }
                if (selectedMember == bS.AllyUnit.Pokemon)
                {
                    partyScreen.SetMessageText($"{selectedMember.Base.Name} is already in battle!");
                    yield break;
                }

                SelectedPokemon = selectedMember;
                gC.StateMachine.Pop();
            }
            else if (DynamicMenuState.i.SelectedItem == 1)
            {
                // summary 
            }
            else
            {
                yield break;
            }
        }
        else if (prev is MenuOpenState)
        {
            // Pokemon summary screen
            DynamicMenuState.i.MenuItems = new List<string>() { "Summary", "Switch", "Exit" };
            yield return gC.StateMachine.PushAndWait(DynamicMenuState.i);
            if (DynamicMenuState.i.SelectedItem == 0)
            {
                //Summary
                Debug.Log($"Selected index = {selection} for summary");
            }
            else if (DynamicMenuState.i.SelectedItem == 1)
            {
                //Switch
                Debug.Log($"Selected index = {selection} to switch");
            }
            else
            {
                yield break;
            }
        }
    }




    IEnumerator EnterUsingItemState()
    {
        yield return gC.StateMachine.PushAndWait(UsingItemState.i);
        gC.StateMachine.Pop();
    }

    void OnBack()
    {
        SelectedPokemon = null;
        AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        var prev = gC.StateMachine.GetPreviousState() as BattleState;
        if (prev != null)
        {
            if (prev.BattleSystem.AllyUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You must select a Pokemon to continue!");
                return;
            }
        }
        gC.StateMachine.Pop();
    }
}
