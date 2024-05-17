using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScreen : MonoBehaviour
{
    [SerializeField] MoveDescriptor moveDescriptor;
    [SerializeField] MoveList moveList;
    

    public void SetData(List<Move> moves)
    {
        int j = 0;
        for (int i = 0; i < moveList.MoveSlots.Count; i++)
        {
            if (j < moves.Count)
                moveList.MoveSlots[i].SetData(moves[j]);
            else
                moveList.MoveSlots[i].SetData(null);
            j++;
        }
    }

    public void SetMoveSelected(int selectedMove)
    {
        for (int i = 0; i < moveList.MoveSlots.Count; i++)
        {
            if (i == selectedMove)
            {
                moveList.MoveSlots[i].SetSelected(true);
            }
            else
                moveList.MoveSlots[i].SetSelected(false);
        }
        
    }

    public void SetDescriptorData(Move move)
    {
        moveDescriptor.SetData(move);
    }

    public void ClearDescriptorData()
    {
        moveDescriptor.Clear();
    }

    public void Clear()
    {
        foreach (var slot in moveList.MoveSlots)
        {
            slot.SetSelected(false);
        }
        moveDescriptor.Clear();
    }
}
