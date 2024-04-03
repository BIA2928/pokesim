using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] Image selectedHighlighter;

    RectTransform rectTransform;
    private void Awake()
    {
        
        //selectedHighlighter = GetComponent<Image>();
        //Debug.Log("selected highlighter is now" + selectedHighlighter.ToString());

    }

    public void SetData(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();
        nameText.text = itemSlot.ItemBase.Name;
        if (itemSlot.ItemBase is TmItem)
        {
            var tm = (TmItem)itemSlot.ItemBase;
            nameText.text += $" : {tm.Move.Name}";
        }
        countText.text = "x" + itemSlot.Count.ToString();
    }

    public void SelectItem()
    {
        selectedHighlighter.enabled = true;
    }

    public void DeselectItem()
    {
        selectedHighlighter.enabled = false;
    }

    public float Height => rectTransform.rect.height;
}
