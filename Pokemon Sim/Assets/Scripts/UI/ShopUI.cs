using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] ItemDescriptionBarUI itemDescriptionBar;
    [SerializeField] WalletUI walletUI;

    List<ItemSlotUI> slotUIList;
    RectTransform itemListRect;
    List<ItemBase> availableItems;

    int selectedItem = 0;
    const int itemsInViewPort = 7;

    Action onBack;
    Action<ItemBase> onItemSelected;

    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }
    public void Show(List<ItemBase> availableItems, Action<ItemBase> onItemSelected, Action onBack)
    {
        this.onItemSelected = onItemSelected;
        this.onBack = onBack;
        gameObject.SetActive(true);
        this.availableItems = availableItems;

        UpdateItemList();
        walletUI.ShowBalance();

        
    }

    public void Close()
    {
        availableItems = null;
        walletUI.CloseWallet();
        gameObject.SetActive(false);
    }

    void UpdateItemList()
    {
        // Clear exisiting items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var item in availableItems)
        {
            var newSlotUI = Instantiate(itemSlotUI, itemList.transform);
            newSlotUI.SetData(item, item.Price);
            slotUIList.Add(newSlotUI);
        }
        UpdateItemSelection();
    }

    void UpdateItemSelection()
    {
        var items = availableItems;
        selectedItem = InventoryUI.MyClamp(selectedItem, 0, items.Count - 1);
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].SelectItem();
            }
            else
                slotUIList[i].DeselectItem();
        }


        if (items.Count > 0)
        {
            itemDescriptionBar.SetData(items[selectedItem]);
            HandleScrolling();
        }
        else
        {
            itemDescriptionBar.ClearFields();
        }

    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewPort) return;
        float scrollPos = Mathf.Clamp(selectedItem - (itemsInViewPort / 2), 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;
        

        selectedItem = InventoryUI.MyClamp(selectedItem, 0, availableItems.Count - 1);
        if (prevSelection != selectedItem)
        {
            UpdateItemSelection();
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onItemSelected?.Invoke(availableItems[selectedItem]);
            AudioManager.i.PlaySFX(AudioID.UISelect);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }


    }
}
