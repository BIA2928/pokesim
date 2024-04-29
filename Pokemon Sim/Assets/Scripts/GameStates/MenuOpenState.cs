using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class MenuOpenState : State<GameController>
{

    [SerializeField] MenuController1 menuController;
    public static MenuOpenState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gC;
    public override void EnterState(GameController owner)
    {
        gC = owner;
        menuController.gameObject.SetActive(true);
        menuController.OnSelected += OnMenuItemSelected;
        menuController.OnBack += OnMenuBack;
    }
    public override void Execute()
    {
        menuController.HandleUpdate();
        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Return))
        {
            
        }
    }

    public override void ExitState()
    {
        menuController.gameObject.SetActive(false);
        menuController.OnSelected -= OnMenuItemSelected;
        menuController.OnBack -= OnMenuBack;

    }

    void OnMenuItemSelected(int selection)
    {
        AudioManager.i.PlaySFX(AudioID.UISelect);
        Debug.Log("Selected menu item " + selection);
    }

    void OnMenuBack()
    {
        AudioManager.i.PlaySFX(AudioID.MenuClose);
        gC.StateMachine.Pop();
    }

}
