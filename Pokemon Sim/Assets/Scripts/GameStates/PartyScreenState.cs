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

    private void Start()
    {
        pokemonParty = PlayerController.i.GetComponent<PokemonParty>();
    }

    PokemonParty pokemonParty;
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


    bool isSwitching = false;
    int selectedIndexForSwitching = 0;
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
                SummaryState.i.PokemonIndex = selection;
                yield return gC.StateMachine.PushAndWait(SummaryState.i);
            }
            else
            {
                yield break;
            }
        }
        else if (prev is MenuOpenState)
        {
            if (isSwitching)
            {
                if (selectedIndexForSwitching == selection)
                {
                    partyScreen.SetMessageText("You can't switch the same pokemon!");
                    yield break;
                }
                var temp = pokemonParty.PokemonList[selectedIndexForSwitching];
                pokemonParty.PokemonList[selectedIndexForSwitching] = pokemonParty.PokemonList[selection];
                pokemonParty.PokemonList[selection] = temp;
                pokemonParty.PartyUpdated();
                isSwitching = false;
                selectedIndexForSwitching = 0;
            }
            else
            {
                // Pokemon summary screen
                DynamicMenuState.i.MenuItems = new List<string>() { "Summary", "Switch", "Exit" };
                yield return gC.StateMachine.PushAndWait(DynamicMenuState.i);
                if (DynamicMenuState.i.SelectedItem == 0)
                {
                    //Summary
                    SummaryState.i.PokemonIndex = selection;
                    yield return gC.StateMachine.PushAndWait(SummaryState.i);
                }
                else if (DynamicMenuState.i.SelectedItem == 1)
                {
                    //Switch
                    Debug.Log($"Selected index = {selection} to switch");
                    isSwitching = true;
                    selectedIndexForSwitching = selection;
                    partyScreen.SetMessageText("Choose a pokemon to switch.");

                }
                else
                {
                    yield break;
                }
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
        var prev = gC.StateMachine.GetPreviousState();
        if (prev is MenuOpenState)
        {
            if (isSwitching)
            {
                isSwitching = false;
                selectedIndexForSwitching = 0;
                partyScreen.SetMessageText("Select a pokemon.");
                return;
            }
        }
        else if (prev is BattleState state)
        {
            if (state.BattleSystem.AllyUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You must select a Pokemon to continue!");
                return;
            }

        }
        gC.StateMachine.Pop();
    }

    void OnBack2()
    {
        SelectedPokemon = null;
        AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        var prev = gC.StateMachine.GetPreviousState();
        if (prev is MenuOpenState)
        {
            if (isSwitching)
            {
                isSwitching = false;
                selectedIndexForSwitching = 0;
                partyScreen.SetMessageText("Select a pokemon.");
                return;
            }
        }
        else if (prev is BattleState state)
        {
            if (state.BattleSystem.AllyUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You must select a Pokemon to continue!");
                return;
            }

        }
        gC.StateMachine.Pop();
    }
}
