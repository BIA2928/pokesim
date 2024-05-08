using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] Text textField;
    [SerializeField] Image image;
    [SerializeField] Sprite selectedSprite;
    [SerializeField] Sprite deselectedSprite;

    Color originalTextColor;

    public void SetSelected(bool selected)
    {
        textField.color = (selected) ? GlobalSettings.i.HighlightedColorBlue : originalTextColor;
        image.sprite = (selected) ? selectedSprite : deselectedSprite;
    }

    public void Init()
    {
        originalTextColor = textField.color;
    }

    public void Clear()
    {
        textField.color = originalTextColor;
    }

}
