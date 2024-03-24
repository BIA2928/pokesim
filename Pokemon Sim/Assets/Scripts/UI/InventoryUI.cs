using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InventoryUIState { ItemSelection, PartySelection, Busy}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] ItemDescriptionBarUI itemDescriptionBar;
    [SerializeField] PartyScreen partyScreen;

    Inventory inventory;
    List<ItemSlotUI> slotUIList;
    RectTransform itemListRect;
    int selectedItem = 0;
    InventoryUIState state;

    const int itemsInViewPort = 7;

    private void Awake()
    {
        inventory = FindObjectOfType<PlayerController>().GetComponent<Inventory>();
        itemListRect = itemList.GetComponent<RectTransform>();
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
        if (state == InventoryUIState.ItemSelection)
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
                OpenPartyScreen();
                
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }
        }
        else if (state == InventoryUIState.PartySelection)
        {
            Action OnBack = () =>
            {
                ClosePartyScreen();
            };

            Action onSelected = () =>
            {
                // Use item on selected poke
            };
            partyScreen.HandleUpdate(onSelected, OnBack);
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

        HandleScrolling();
    }

    void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - (itemsInViewPort / 2), 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }

    void OpenPartyScreen()
    {

        state = InventoryUIState.PartySelection;

        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {

        state = InventoryUIState.ItemSelection;

        partyScreen.gameObject.SetActive(false);
    }
}
