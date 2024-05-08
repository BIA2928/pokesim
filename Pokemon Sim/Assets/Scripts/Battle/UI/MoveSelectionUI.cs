using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenericSelectionUI;
using System.Linq;
using UnityEngine.UI;

public class MoveSelectionUI : SelectionUI<TextSlot>
{
    [SerializeField] List<TextSlot> moveTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    private void Start()
    {
        SetSelectionSettings(SelectionType.Grid, 2);
    }

    List<Move> moves;
    public void SetMoves(List<Move> moves)
    {
        this.moves = moves;
        SetItems(moveTexts.Take(moves.Count).ToList());
        
        Debug.Log($"Set moves with moves=null == {this.moves == null}");
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].SetText(moves[i].Base.Name);
            }
            else
            {
                moveTexts[i].SetText("-");
            }
        }
    }

    protected override void UpdateSelectionInUI()
    {
        base.UpdateSelectionInUI();
        Debug.Log(moves);
        var move = moves[selection];
        ppText.text = $"PP {move.PP}/{move.Base.Pp}";
        typeText.text = move.Base.Type.ToString();

        if (move.PP == 0)
        {
            ppText.color = Color.red;
        }
        else
        {
            ppText.color = Color.black;
        }
    }
}
