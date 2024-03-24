using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionBarUI : MonoBehaviour
{
    [SerializeField] Image selectedItemImage;
    [SerializeField] Text selectedItemDescription;

    public void SetData(ItemBase itemBase)
    {
        selectedItemDescription.text = itemBase.Description;
        selectedItemImage.sprite = itemBase.BagIcon;
    }
}
