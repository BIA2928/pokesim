using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI countText;
    Image selectedHighlighter;

    RectTransform rectTransform;
    private void Awake()
    {
        selectedHighlighter = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetData(ItemSlot itemSlot)
    {
        nameText.text = itemSlot.ItemBase.Name;
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
