using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GenericSelectionUI;
using System.Linq;

public class MoveForgetScreen : SelectionUI<TextSlot>
{
    [SerializeField] Text messageText;
    [SerializeField] Text moveDescriptionText;
    MoveForgetUI[] moveSlots;
    MoveForgetUI moveToLearnSlot;
    List<Move> moves;
    MoveBase moveToLearn;


    public void Init()
    {
        moveSlots = GetComponentsInChildren<MoveForgetUI>(true);
        moveToLearnSlot = moveSlots[moveSlots.Length - 1];
        SetSelectionSettings(SelectionType.Grid, 2);
    }

    public void SetMoveData(List<Move> moves, MoveBase moveToLearn)
    {
        this.moves = moves;
        this.moveToLearn = moveToLearn;

        for (int i = 0; i < moves.Count; i++)
        {
            moveSlots[i].gameObject.SetActive(true);
            moveSlots[i].SetData(moves[i]);
        }
        moveToLearnSlot.SetData(moveToLearn);
        messageText.text = $"Which move should be forgotten for {moveToLearn.Name}?";
        SetMoveDescriptionText(moveToLearn);

        
        SetItems(moveSlots.Select(mbox => mbox.GetComponent<TextSlot>()).ToList());
        UpdateSelectionInUI();
    }


    public void SetMessageText(string text)
    {
        messageText.text = text;
    }

    public void SetMoveDescriptionText(MoveBase moveToLearn)
    {
        string movePower;
        if (moveToLearn.Power == 0)
            movePower = "-";
        else
            movePower = moveToLearn.Power.ToString();
        string topLine = $"Power: {movePower},  Accur: {moveToLearn.Accuracy}\n";
        moveDescriptionText .text = topLine + moveToLearn.Description;
    }





}
