using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class AboutToUseState : State<BattleSystem>
{
    public static AboutToUseState i { get; private set; }
    public Pokemon NewPokemon { get; set; }
    private void Awake()
    {
        i = this;
    }

    BattleSystem bS;
    public override void EnterState(BattleSystem owner)
    {
        bS = owner;
        StartCoroutine(AskAboutToUse());
    }

    bool aboutToUseChoice = true;
    public override void Execute()
    {
        if (!bS.DialogueBox.IsChoiceBoxEnabled)
            return;


        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            aboutToUseChoice = !aboutToUseChoice;
            bS.DialogueBox.UpdateChoiceSelection(aboutToUseChoice);
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            bS.DialogueBox.EnableChoiceSelector(false);
            AudioManager.i.PlaySFX(AudioID.UISelect);
            if (aboutToUseChoice)
            {
                StartCoroutine(WaitForUserChooseNextPokemon());
            }
            else
            {
                StartCoroutine(WaitForTrainerSendNextPokemon());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
            bS.DialogueBox.EnableChoiceSelector(false);
            StartCoroutine(WaitForTrainerSendNextPokemon());
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    IEnumerator AskAboutToUse()
    {
        Dialogue d = new Dialogue();
        d.Lines.Add($"{bS.EnemyTrainer.Name} is about to use {NewPokemon.Base.Name}.");
        d.Lines.Add("Do you want to switch?");
        yield return bS.DialogueBox.ShowDialogue(d, false, true);
        bS.DialogueBox.EnableChoiceSelector(true);
        bS.DialogueBox.UpdateChoiceSelection(aboutToUseChoice);
    }

    IEnumerator WaitForTrainerSendNextPokemon()
    {
        yield return bS.SendNextTrainerPokemon();
        bS.StateMachine.Pop();
    }

    IEnumerator WaitForUserChooseNextPokemon()
    {
        yield return GameController.i.StateMachine.PushAndWait(PartyScreenState.i);
        var selected = PartyScreenState.i.SelectedPokemon;
        if (selected != null)
        {
            yield return bS.SwitchPokemon(selected);
        }
        yield return WaitForTrainerSendNextPokemon();
        
    }
}
