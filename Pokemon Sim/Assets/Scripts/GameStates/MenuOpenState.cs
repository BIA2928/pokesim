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
        if (selection == 0)
            gC.StateMachine.Push(PartyScreenState.i);
        else if (selection == 1)
            //Pokedex
            Debug.Log("Pokedex selected");
        else if (selection == 2)
            gC.StateMachine.Push(InventoryState.i);
        else if (selection == 3)
        {
            //gC.StateMachine.Push(BadgeState.i);
            // for testing, use as load funciton
            SavingSystem.i.Load("SaveSlot1");
            gC.StateMachine.Pop();
        }
        else if (selection == 4)
        {
            StartCoroutine(SaveSelected());
            gC.StateMachine.Pop();
        }
        else if (selection == 5)
        {
            // options
        }

            
    }

    IEnumerator SaveSelected()
    {
        yield return Fader.instance.FadeIn(0.3f);
        SavingSystem.i.Save("SaveSlot1");
        yield return Fader.instance.FadeOut(0.3f);
    }

    void OnMenuBack()
    {
        AudioManager.i.PlaySFX(AudioID.MenuClose);
        gC.StateMachine.Pop();
    }

}
