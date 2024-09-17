using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class ForgettingMoveState : State<GameController>
{
    [SerializeField] MoveForgetScreen forgetScreen;
    public static ForgettingMoveState i;
    public MoveBase MoveToLearn { get; set; }
    public List<Move> Moves { get; set; }

    public int Selection { get; set; }
    GameController gC;

    private void Awake()
    {
        i = this;
    }
    public override void EnterState(GameController owner)
    {
        gC = owner;

        Selection = 0;

        forgetScreen.Init();
        forgetScreen.SetMoveData(Moves, MoveToLearn);
        forgetScreen.gameObject.SetActive(true);
        forgetScreen.OnSelected += OnMoveSelected;
        forgetScreen.OnBack += OnBack;
    }

    public override void Execute()
    {
        forgetScreen.HandleUpdate();
    }

    public override void ExitState()
    {
        forgetScreen.gameObject.SetActive(false);
        forgetScreen.OnSelected -= OnMoveSelected;
        forgetScreen.OnBack -= OnBack;
    }

    void OnMoveSelected(int selection)
    {
        Selection = selection;
        gC.StateMachine.Pop();
    }

    void OnBack()
    {
        Selection = -1;
        gC.StateMachine.Pop();
    }
}
