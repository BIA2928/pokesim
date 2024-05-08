using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class MoveSelectionState : State<BattleSystem>
{
    [SerializeField] MoveSelectionUI moveSelectionUI;
    [SerializeField] GameObject moveDetails;


    //Required input before state
    public List<Move> Moves { get; set; }


    public static MoveSelectionState i { get; private set; }

    private void Awake()
    {
        i = this;
    }


    BattleSystem bS;
    public override void EnterState(BattleSystem owner)
    {
        bS = owner;

        moveSelectionUI.SetMoves(Moves);

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.OnSelected += OnMoveSelected;
        moveSelectionUI.OnBack += OnBack;

        moveDetails.SetActive(true);
        bS.DialogueBox.EnableDialogueText(false);
    }

    public override void Execute()
    {
        moveSelectionUI.HandleUpdate();
        
    }

    public override void ExitState()
    {
        moveSelectionUI.ClearItems();
        moveSelectionUI.gameObject.SetActive(false);
        moveSelectionUI.OnSelected -= OnMoveSelected;
        moveSelectionUI.OnBack -= OnBack;

        moveDetails.SetActive(false);
        bS.DialogueBox.EnableDialogueText(true);
    }

    void OnMoveSelected(int selection)
    {
        bS.SelectedMove = selection;
        bS.StateMachine.ChangeState(RunTurnState.i);
    }

    void OnBack()
    {
        bS.StateMachine.ChangeState(ActionSelectionState.i);
    }
}

