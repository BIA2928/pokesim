using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class AskToForgetState : State<BattleSystem>
{
    public static AskToForgetState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    BattleSystem bS;
    public override void EnterState(BattleSystem owner)
    {
        bS = owner;
        StartCoroutine(AskToForget());
    }

    //Input
    public MoveBase MoveToLearn { get; set; }
    //Output
    public bool ForgetMoveChoice { get; private set; } = true;
    public override void Execute()
    {
        if (!bS.DialogueBox.IsChoiceBoxEnabled)
            return;


        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ForgetMoveChoice = !ForgetMoveChoice;
            bS.DialogueBox.UpdateChoiceSelection(ForgetMoveChoice);
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            bS.DialogueBox.EnableChoiceSelector(false);
            AudioManager.i.PlaySFX(AudioID.UISelect);
            StartCoroutine(ChooseMoveToForget());
            
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
            bS.StateMachine.Pop();
            
        }
    }

    public override void ExitState()
    {
        bS.DialogueBox.EnableChoiceSelector(false);
        MoveToLearn = null;
    }

    IEnumerator AskToForget()
    {
        yield return bS.DialogueBox.ShowDialogue($"{bS.CurrentAllyPokemon.Base.Name} wants to learn {MoveToLearn.Name}.",  hideActions: true);
        yield return new WaitForSeconds(0.1f);
        yield return bS.DialogueBox.ShowDialogue($"But {bS.CurrentAllyPokemon.Base.Name} already knows four moves.", hideActions: true);
        yield return new WaitForSeconds(0.1f);
        yield return bS.DialogueBox.ShowDialogue($"Shall {bS.CurrentAllyPokemon.Base.Name} forget a move to make space for {MoveToLearn.Name}?", false, true);

        bS.DialogueBox.EnableChoiceSelector(true);
        bS.DialogueBox.UpdateChoiceSelection(ForgetMoveChoice);
    }

    IEnumerator ChooseMoveToForget()
    {
        bS.DialogueBox.EnableChoiceSelector(false);
        if (ForgetMoveChoice)
            yield return bS.DialogueBox.ShowDialogue("Select a move to forget.", false, hideActions: true);
        bS.StateMachine.Pop();
    }

    
}
