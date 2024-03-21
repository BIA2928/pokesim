using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSlot : MonoBehaviour
{
    [SerializeField] Text textField;
    [SerializeField] Image imageField;

    [SerializeField] Sprite selectedSprite;
    [SerializeField] Sprite deselectedSprite;



    public void Select()
    {
        imageField.sprite = selectedSprite;
        textField.color = GlobalSettings.i.HighlightedColorBlue;
    }

    public void Deselect()
    {
        imageField.sprite = deselectedSprite;
        textField.color = Color.black;
    }
}
