using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShopState  
{ 
    Menu, Buying, Selling, Busy
}

public class ShopController : MonoBehaviour
{

    public static ShopController instance { get; private set; }
    public ShopState state;
    [SerializeField] Vector2 shopCamOffset;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelector;
    [SerializeField] ShopUI shopOverlay;
    Merchant currMerchant;
    Inventory inventory;

    public event Action OnStartShopping;
    public event Action OnCloseShop;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        inventory = Inventory.GetInventory();
    }
    public IEnumerator StartTrading(Merchant merchant)
    {
        currMerchant = merchant;
        OnStartShopping?.Invoke();
        yield return StartMenuState();
    }

    IEnumerator StartMenuState()
    {
        state = ShopState.Menu;
        int selectedChoice = 0;
        List<string> choices = new List<string>() { "Buy", "Sell", "Exit" };
        Action<int> onChoiceSelection = (i) =>
        {
            selectedChoice = i;
        };
        yield return DialogueManager.Instance.ShowDialogueChoices(currMerchant.OpeningDialogue, choices, onChoiceSelection, false);

        if (selectedChoice == 0)
        {
            // Buy
            
            yield return GameController.i.MoveCamera(shopCamOffset);
            shopOverlay.Show(currMerchant.SellingItems, (selItem)=>StartCoroutine(BuyItem(selItem)), () => StartCoroutine(OnBackFromBuying()));
            state = ShopState.Buying;
        }
        else if (selectedChoice == 1)
        {
            // sell
            inventoryUI.gameObject.SetActive(true);
            walletUI.ShowBalance();
            state = ShopState.Selling;
        }
        else
        {
            //Exit
            OnCloseShop?.Invoke();
            yield return DialogueManager.Instance.ShowDialogue(currMerchant.ClosingDialogue);
            yield break;
        }
    }


    public void HandleUpdate()
    {
        if (state == ShopState.Selling)
        {
            Action onBack = () =>
            {
                StartCoroutine(StartMenuState());
                inventoryUI.gameObject.SetActive(false);
                walletUI.CloseWallet();
            };
            Action<ItemBase> onSelected = (ItemBase sellItem) =>
            {
                StartCoroutine(SellItem(sellItem));
            };
            inventoryUI.HandleUpdate(onBack, onSelected);
        }
        else if (state ==  ShopState.Buying)
        {
            shopOverlay.HandleUpdate();
        }
    }

    IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.Busy;
        int count = 1;
        int maxToBuy = Mathf.Min(999, Wallet.i.Balance / item.Price);
        yield return DialogueManager.Instance.ShowDialogue($"{item.Name}? Certainly.\nHow many would you like?",false, false);
        yield return countSelector.ShowSelector(maxToBuy, item.Price, (selectedCount) => count = selectedCount);

        DialogueManager.Instance.CloseDialogue();

        int totalPrice = item.Price * count;

        if (count == 0)
        {
            // Can't afford
            yield return DialogueManager.Instance.ShowDialogue("Sorry, you don't seem to have enough money for that...");
            state = ShopState.Buying;
            yield break;
        }
        else if (count == -1)
        {
            // Opted to go back
            state = ShopState.Buying;
            yield break;
        }

        int selectedChoice = 0;
        List<string> choices = new List<string>() { "Yes", "No" };
        Action<int> onChoiceSelection = (i) => { selectedChoice = i; };
        Dialogue d = new Dialogue();
        if (count == 1)
        {
            d.AddLine($"{item.Name}, and you want one.\nThat'll be ${totalPrice}. Is that OK?");
        }
        else
        {
            d.AddLine($"{item.Name}, and you want {count}.\nThat'll be ${totalPrice}. Is that OK?");
        }
        yield return DialogueManager.Instance.ShowDialogueChoices(d, choices, (cI) => selectedChoice = cI, false);

        if (selectedChoice == 0)
        {
            //yes
            inventory.AddItem(item, count);
            Wallet.i.Pay(totalPrice);
            Dialogue postBuyDialogue = new Dialogue()
            {
                Lines =
                {
                    "Here you are!\nThank you!",
                    $"You put away the {item.Name}(s) in the {Inventory.GetPocketForItem(item)} pocket."
                }
            };
            yield return DialogueManager.Instance.ShowDialogue(postBuyDialogue);
        }
        state = ShopState.Buying;

    }

    IEnumerator SellItem(ItemBase item)
    {
        state = ShopState.Busy;

        if (!item.IsSellable)
        {
            yield return DialogueManager.Instance.ShowDialogue($"{item.Name}?\nSorry, we don't buy those.");
            state = ShopState.Selling;
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
                state = ShopState.Selling;
                yield break;
            }
        }

        int selectedChoice = 0;
        sellPrice *= countToSell;

        List<string> choices = new List<string>() { "Yes", "No"};
        Action<int> onChoiceSelection = (i) =>{ selectedChoice = i; };
        Dialogue d = new Dialogue(){ Lines = { $"I can offer you ${sellPrice} for that.", } };
        
        yield return DialogueManager.Instance.ShowDialogueChoices(d, choices, onChoiceSelection, false);
        if (selectedChoice == 0)
        {
            // Add money, remove item
            inventory.RemoveMany(item, countToSell);
            Wallet.i.AddMoney(sellPrice);
            yield return DialogueManager.Instance.ShowDialogue($"Handed over {item.Name} and received ${sellPrice}.");
        }
        state = ShopState.Selling;
    }

    IEnumerator OnBackFromBuying()
    {
        yield return GameController.i.MoveCamera(-shopCamOffset);
        shopOverlay.Close();
        StartCoroutine(StartMenuState());
    }

}
