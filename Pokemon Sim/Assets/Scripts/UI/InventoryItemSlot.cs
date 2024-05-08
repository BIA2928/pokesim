using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour, ISelectableItem
{
    //[SerializeField] Image selectedHighlighter;
    Image selectedHighlighter;
    public void Init()
    {
        selectedHighlighter = GetComponent<Image>();
    }

    public void SetSelected(bool selected)
    {
        selectedHighlighter.enabled = selected;
    }

    public void Clear()
    {
        selectedHighlighter.enabled = false;
    }
}
