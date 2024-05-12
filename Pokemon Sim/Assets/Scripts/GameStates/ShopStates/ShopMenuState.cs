using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class ShopMenuState : State<GameController>
{
    public static ShopMenuState i { get; private set; }
    
    private void Awake()
    {
        i = this;
    }

    //Input
    public List<ItemBase> AvailableItems { get; set; }
    public Merchant CurrMerchant { get; set; }

    GameController gC;
    public override void EnterState(GameController owner)
    {
        gC = owner;

        StartCoroutine(StartMenuState());
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void ExitState()
    {
        StartCoroutine(ExitShop());
    }

    IEnumerator StartMenuState()
    {
        int selectedChoice = 0;
        List<string> choices = new List<string>() { "Buy", "Sell", "Exit" };
        System.Action<int> onChoiceSelection = (i) =>
        {
            selectedChoice = i;
        };
        yield return DialogueManager.Instance.ShowDialogueChoices(CurrMerchant.OpeningDialogue, choices, onChoiceSelection, false);

        if (selectedChoice == 0)
        {
            // Buy
            ShopBuyingState.i.AvailableItems = AvailableItems;
            yield return gC.StateMachine.PushAndWait(ShopBuyingState.i);
        }
        else if (selectedChoice == 1)
        {
            // sell
            yield return gC.StateMachine.PushAndWait(ShopSellingState.i);
        }
        else
        {
            //Exit
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }

        
    }

    IEnumerator ExitShop()
    {
        AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        yield return DialogueManager.Instance.ShowDialogue(CurrMerchant.ClosingDialogue);
        gC.StateMachine.Pop();
    }

    
}
