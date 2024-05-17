using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MoveSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI ppText;
    [SerializeField] TextMeshProUGUI moveTypeText;
    [SerializeField] Image typeImage;

    Move currentMove;
    Image selectorImage;
    void Awake()
    {
        selectorImage = GetComponent<Image>();
        selectorImage.enabled = false;
    }

    public void SetSelected(bool selected)
    {
        selectorImage.enabled = selected;
        
    }

    public void Clear()
    {
        selectorImage.enabled = false;
        nameText.text = "-";
        ppText.text = "--/--";
        moveTypeText.text = "";
        //Imageclear
    }


    public void SetData(Move move)
    {
        
        if (move == null)
        {
            typeImage.sprite = TypeImageDB.i.Lookup(PokeType.None);
            nameText.text = "-";
            ppText.text = "--/--";
            moveTypeText.text = "";
            return;
        }
        typeImage.sprite = TypeImageDB.i.Lookup(move.Base.Type);
        currentMove = move;
        nameText.text = move.Base.Name.ToUpper();
        ppText.text = $"{move.PP}/{move.Base.Pp}";
        moveTypeText.text = move.Base.MoveType.ToString().ToUpper();
        
    }

    
}
