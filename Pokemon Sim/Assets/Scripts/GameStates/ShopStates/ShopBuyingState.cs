using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class ShopBuyingState : State<GameController>
{
    [SerializeField] Vector2 shopCamOffset;
    [SerializeField] WalletUI walletUI;
    [SerializeField] CountSelectorUI countSelector;
    [SerializeField] ShopUI shopOverlay;
    Inventory inventory;

    public static ShopBuyingState i { get; private set; }

    private void Awake()
    {
        i = this;
        
    }

    //Input
    public List<ItemBase> AvailableItems { get; set; }

    GameController gC;
    public override void EnterState(GameController owner)
    {
        browseItems = false;
        inventory = Inventory.GetInventory();
        gC = owner;
        StartCoroutine(StartBuyingState());
    }

    public override void Execute()
    {
        if (!browseItems)
            return;
        shopOverlay.HandleUpdate();
    }


    bool browseItems = false;
    IEnumerator StartBuyingState()
    {
        yield return GameController.i.MoveCamera(shopCamOffset);
        shopOverlay.Show(AvailableItems, (selItem) => StartCoroutine(BuyItem(selItem)), 
            () => StartCoroutine(OnBackFromBuying()));
        browseItems = true;
    }

    IEnumerator BuyItem(ItemBase item)
    {
        browseItems = false;
        int count = 1;
        int maxToBuy = Mathf.Min(999, Wallet.i.Balance / item.Price);
        yield return DialogueManager.Instance.ShowDialogue($"{item.Name}? Certainly.\nHow many would you like?", false, false);
        yield return countSelector.ShowSelector(maxToBuy, item.Price, (selectedCount) => count = selectedCount);

        DialogueManager.Instance.CloseDialogue();

        int totalPrice = item.Price * count;

        if (count == 0)
        {
            // Can't afford
            yield return DialogueManager.Instance.ShowDialogue("Sorry, you don't seem to have enough money for that...");
            yield break;
        }
        else if (count == -1)
        {
            // Opted to go back
            yield break;
        }

        int selectedChoice = 0;
        List<string> choices = new List<string>() { "Yes", "No" };
        System.Action<int> onChoiceSelection = (i) => { selectedChoice = i; };
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
        browseItems = true;
    }

    IEnumerator OnBackFromBuying()
    {
        yield return GameController.i.MoveCamera(-shopCamOffset);
        shopOverlay.Close();
        gC.StateMachine.Pop();
    }
}
