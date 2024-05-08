using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextSlot : MonoBehaviour, ISelectableItem
{
    [SerializeField] Text textField;

    Color originalTextColor;

    [SerializeField] bool selectedIsRed = false;

    public void SetSelected(bool selected)
    {
        if (selectedIsRed)
            textField.color = (selected) ? GlobalSettings.i.HighlightedColorRed : originalTextColor;
        else
            textField.color = (selected) ? GlobalSettings.i.HighlightedColorBlue : originalTextColor;
    }

    public void Init()
    {
        originalTextColor = textField.color;
    }

    public void SetText(string text)
    {
        this.textField.text = text;
    }

    public void Clear()
    {
        textField.color = originalTextColor;
    }

}
