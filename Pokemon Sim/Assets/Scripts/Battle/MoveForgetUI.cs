using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveForgetUI : MonoBehaviour
{
    [SerializeField] Color highlightColor;
    [SerializeField] Text moveNameText;
    [SerializeField] Text ppText;
    [SerializeField] Text moveTypeText;
    [SerializeField] Text moveCategoryText;

    Move _move;


    public void SetData(Move move)
    {
        _move = move;

        moveNameText.text = move.Base.Name;
        moveCategoryText.text = move.Base.MoveType.ToString();
        moveTypeText.text = move.Base.Type.ToString();
        ppText.text = move.PP + " / " + move.Base.Pp;

    }

    public void SetData(MoveBase move)
    {

        moveNameText.text = move.Name;
        moveCategoryText.text = move.MoveType.ToString();
        moveTypeText.text = move.Type.ToString();
        ppText.text = move.Pp + " / " + move.Pp;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            moveNameText.color = highlightColor;
        else
            moveNameText.color = Color.white;
    }

}
