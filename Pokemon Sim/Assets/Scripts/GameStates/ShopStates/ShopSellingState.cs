using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class ShopSellingState : State<GameController>
{
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelector;
    public static ShopSellingState i { get; private set; }

    [SerializeField] InventoryUI inventoryUI;
    private void Awake()
    {
        i = this;
    }

    GameController gC;
    Inventory inventory;
    public override void EnterState(GameController owner)
    {
        gC = owner;
        inventory = Inventory.GetInventory();
        StartCoroutine(StartSellingState());
        walletUI.ShowBalance();
    }

    IEnumerator StartSellingState()
    {
        yield return gC.StateMachine.PushAndWait(InventoryState.i);

        var selectedItem = InventoryState.i.SelectedItem;
        if (selectedItem != null)
        {
            // Item was selected
            yield return SellItem(selectedItem);
            StartCoroutine(StartSellingState());
        }
        else
        {
            gC.StateMachine.Pop();
        }
    }

    IEnumerator SellItem(ItemBase item)
    {

        if (!item.IsSellable)
        {
            yield return DialogueManager.Instance.ShowDialogue($"{item.Name}?\nSorry, we don't buy those.");
            yield break;
        }

        int sellPrice = Mathf.FloorToInt(item.Price * 0.8f);
        int amount = inventory.GetItemQuantity(item);
        int countToSell = 1;
        if (amount > 1)
        {
            yield return DialogueManager.Instance.ShowDialogue($"{item.Name}?\nHow many would you like to sell?", false, false);
            yield return countSelector.ShowSelector(amount, sellPrice, (selCount) => countToSell = selCount);
            DialogueManager.Instance.CloseDialogue();
            if (countToSell == -1)
            {
                // this means user selected back option
                yield break;
            }
        }

        int selectedChoice = 0;
        sellPrice *= countToSell;

        List<string> choices = new List<string>() { "Yes", "No" };
        System.Action<int> onChoiceSelection = (i) => { selectedChoice = i; };
        Dialogue d = new Dialogue() { Lines = { $"I can offer you ${sellPrice} for that.", } };

        yield return DialogueManager.Instance.ShowDialogueChoices(d, choices, onChoiceSelection, false);
        if (selectedChoice == 0)
        {
            // Add money, remove item
            inventory.RemoveMany(item, countToSell);
            Wallet.i.AddMoney(sellPrice);
            yield return DialogueManager.Instance.ShowDialogue($"Handed over {item.Name} and received ${sellPrice}.");
        }
    }
}
