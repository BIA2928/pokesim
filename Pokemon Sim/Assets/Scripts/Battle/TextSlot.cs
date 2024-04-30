using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] Text textField;

    Color originalTextColor;

    public void SetSelected(bool selected)
    {
        textField.color = (selected) ? GlobalSettings.i.HighlightedColorRed: originalTextColor;
    }

    public void Init()
    {
        originalTextColor = textField.color;
    }
}
