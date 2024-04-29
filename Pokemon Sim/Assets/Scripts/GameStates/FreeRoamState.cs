using Utils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRoamState: State<GameController>
{
    public static FreeRoamState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gC;
    public override void EnterState(GameController owner)
    {
        gC = owner;
    }

    public override void Execute()
    {
        PlayerController.i.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayerController.i.StopPlayerMovement();
            AudioManager.i.PlaySFX(AudioID.MenuOpen);
            gC.StateMachine.Push(MenuOpenState.i);
        }
        // Only for testing purposes, to be remvoed later
        else if (Input.GetKeyDown(KeyCode.L))
        {
            SavingSystem.i.Load("testSave1");
        }
    }
}
