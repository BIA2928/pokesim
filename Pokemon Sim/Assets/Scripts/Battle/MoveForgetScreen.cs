using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveForgetScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    [SerializeField] Text moveDescriptionText;
    MoveForgetUI[] moveSlots;
    MoveForgetUI moveToLearnSlot;
    List<Move> moves;
    MoveBase moveToLearn;

    int currSelection = 0;

    // Start is called before the first frame update
    public void Init()
    {
        moveSlots = GetComponentsInChildren<MoveForgetUI>(true);
        moveToLearnSlot = moveSlots[moveSlots.Length - 1];
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

        UpdateMoveSelection(0);
    }

    public void UpdateMoveSelection(int selectedMoveIndex)
    { 
        if (selectedMoveIndex == 4)
        {
            // New move is selected
            moveToLearnSlot.SetSelected(true);
            for (int i = 0; i < moveSlots.Length; i++)
                moveSlots[i].SetSelected(false);
        }
        else
        {
            // One of the four old moves is selected
            moveToLearnSlot.SetSelected(false);
            for (int i = 0; i < moveSlots.Length; i++)
            {
                if (i == selectedMoveIndex)
                    moveSlots[i].SetSelected(true);
                else
                    moveSlots[i].SetSelected(false);
            }

        }
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

    public void HandleUpdate(Action<int> onMoveSelected, Action onBack)
    {
        int prevSelection = currSelection;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            onMoveSelected?.Invoke(currSelection);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log($"Going back, onBack = null is {onBack == null}");
            onBack?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currSelection += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currSelection -= 2;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currSelection++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currSelection--;
        }

        currSelection = InventoryUI.MyClamp(currSelection, 0, 4);
        if (currSelection != prevSelection)
            UpdateMoveSelection(currSelection);

    }


}
