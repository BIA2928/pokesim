using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] ActionSelectionUI selectionUI;
    public static ActionSelectionState i;

    void Awake()
    {
        i = this;
    }

    BattleSystem bS;
    public override void EnterState(BattleSystem owner)
    {
        bS = owner;
        bS.DialogueBox.SetDialogue($"What will {bS.CurrentAllyPokemon.Base.Name} do?");
        selectionUI.gameObject.SetActive(true);
        selectionUI.OnSelected += OnActionSelected;
        selectionUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        selectionUI.HandleUpdate();
    }

    public override void ExitState()
    {
        selectionUI.gameObject.SetActive(false);
        selectionUI.OnSelected -= OnActionSelected;
        selectionUI.OnBack -= OnBack;
    }

    void OnActionSelected(int selection)
    {
        if (selection == 0)
        {
            //Fight 
            bS.SelectedAction = BattleAction.Move;
            MoveSelectionState.i.Moves = bS.CurrentAllyPokemon.Moves;
            bS.StateMachine.ChangeState(MoveSelectionState.i);
        }
        else if (selection == 1) 
        {
            //Bag
            bS.SelectedAction = BattleAction.UseItem;
        }
        else if (selection == 2)
        {
            //Party 
            StartCoroutine(GoToPartyState());
        }
        else
        {
            //Run
            bS.SelectedAction = BattleAction.Run;
            bS.StateMachine.ChangeState(RunTurnState.i);
        }

    }

    IEnumerator GoToPartyState()
    {
        
        yield return GameController.i.StateMachine.PushAndWait(PartyScreenState.i);
        var selected = PartyScreenState.i.SelectedPokemon;
        if (selected != null)
        {
            bS.SelectedAction = BattleAction.SwitchPoke;
            bS.SelectedSwitchPokemon = selected;
            bS.StateMachine.ChangeState(RunTurnState.i);
        }
    }
    
    void OnBack()
    {

    }
}
