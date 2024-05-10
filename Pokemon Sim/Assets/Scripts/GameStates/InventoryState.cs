using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class InventoryState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;
    Inventory inventory;

    public static InventoryState i { get; private set; }

    public ItemBase SelectedItem { get; private set; }

    private void Awake()
    {
        i = this;
    }

    void Start()
    {

        inventory = Inventory.GetInventory();
    }

    GameController gC;
    public override void EnterState(GameController owner)
    {
        SelectedItem = null;
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
        SelectedItem = inventoryUI.SelectedItem;
        StartCoroutine(SelectPokemonAndUseItem());
    }

    void OnBack() 
    {
        SelectedItem = null;
        AudioManager.i.PlaySFX(AudioID.MenuClose);
        gC.StateMachine.Pop();
    }

    IEnumerator SelectPokemonAndUseItem()
    {
        var prevState = gC.StateMachine.GetPreviousState() as BattleState;
        if (prevState != null)
        {
            // In battle 
            if (!SelectedItem.CanUseInBattle)
            {
                yield return DialogueManager.Instance.ShowCantUseDialogue();
                yield break;
            }
        } 
        else
        {
            if (!SelectedItem.CanUseOutsideBattle)
            {
                yield return DialogueManager.Instance.ShowCantUseDialogue();
                yield break;
            }
        }
        if (SelectedItem is PokeballItem)
        {
            inventory.UseItem(SelectedItem, null);
            gC.StateMachine.Pop();
            yield break;
        }
        yield return gC.StateMachine.PushAndWait(PartyScreenState.i);

        if (prevState != null)
        {
            if (UsingItemState.i.ItemUsed)
                gC.StateMachine.Pop();
        }
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
