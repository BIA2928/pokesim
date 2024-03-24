using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] ItemDescriptionBarUI itemDescriptionBar;

    Inventory inventory;
    List<ItemSlotUI> slotUIList;
    int selectedItem = 0;

    private void Awake()
    {
        inventory = FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    private void Start()
    {
        UpdateItemList();
        UpdateItemSelection();
    }

    void UpdateItemList()
    {
        // Clear exisiting items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.Items)
        {
            var newSlotUI = Instantiate(itemSlotUI, itemList.transform);
            newSlotUI.SetData(itemSlot);
            slotUIList.Add(newSlotUI);
        }
    }
    public void HandleUpdate(Action onBack)
    {
        int prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --selectedItem;
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Items.Count - 1);

        if (selectedItem != prevSelection)
        {
            UpdateItemSelection();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //onMenuSlotSelected?.Invoke(selectedItem);
            //CloseMenu();
            onBack?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            //CloseMenu();
        }
    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].SelectItem();
            }
            else
                slotUIList[i].DeselectItem();
        }

        itemDescriptionBar.SetData(inventory.Items[selectedItem].ItemBase);
    }
}
