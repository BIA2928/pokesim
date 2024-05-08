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
        Debug.Log($"Selected index = {selection}");
        var prev = gC.StateMachine.GetPreviousState();
        if (prev is InventoryState)
        {
            StartCoroutine(EnterUsingItemState());
        }
        else if (prev is BattleState) 
        {
            BattleSystem bS = (prev as BattleState).BattleSystem;
            var selectedMember = partyScreen.SelectedMember;
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText($"{selectedMember.Base.Name} has fainted and cannot battle.");
                return;
            }
            if (selectedMember == bS.AllyUnit.Pokemon)
            {
                partyScreen.SetMessageText($"{selectedMember.Base.Name} is already in battle!");
                return;
            }

            SelectedPokemon = selectedMember;
            gC.StateMachine.Pop();
        }
        else if (gC.StateMachine.GetPreviousState() is MenuOpenState)
        {
            // Pokemon summary screen
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
