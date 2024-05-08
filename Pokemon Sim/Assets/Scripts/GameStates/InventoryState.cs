using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class InventoryState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;

    public static InventoryState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gC;
    public override void EnterState(GameController owner)
    {
        gC = owner;
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OnSelected += OnItemSelected;
        inventoryUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        inventoryUI.HandleUpdate();
    }

    public override void ExitState()
    {
        inventoryUI.gameObject.SetActive(false);
        inventoryUI.OnSelected -= OnItemSelected;
        inventoryUI.OnBack -= OnBack;
        
    }

    void OnItemSelected(int selection) 
    {
        AudioManager.i.PlaySFX(AudioID.UISelect);
        gC.StateMachine.Push(PartyScreenState.i);
    }

    void OnBack() 
    {

        AudioManager.i.PlaySFX(AudioID.MenuClose);
        gC.StateMachine.Pop();
    }

    public bool InTMPocket()
    {
        if (inventoryUI.SelectedPocket == (int)ItemType.Tms)
            return true;
        return false;
    }

    public ItemBase GetSelectedItem()
    {
        return inventoryUI.SelectedItem;
    }

}
