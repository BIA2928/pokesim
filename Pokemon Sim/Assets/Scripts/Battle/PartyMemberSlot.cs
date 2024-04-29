using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] Text textField;
    [SerializeField] Image image;
    [SerializeField] Sprite selectedSprite;
    [SerializeField] Sprite deselectedSprite;

    Color originalTextColor;
    private void Awake()
    {
        originalTextColor = textField.color;
    }
    public void SetSelected(bool selected)
    {
        textField.color = (selected) ? GlobalSettings.i.HighlightedColorRed: originalTextColor;
    }
}
