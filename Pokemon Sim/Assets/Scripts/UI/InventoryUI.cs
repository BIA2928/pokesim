using GenericSelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class InventoryUI : SelectionUI<InventoryItemSlot>
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] ItemDescriptionBarUI itemDescriptionBar;
    [SerializeField] PartyScreen partyScreen;

    [SerializeField] PocketSelectorUI pocketUI;
    [SerializeField] MoveForgetScreen moveForgetScreen;

    Inventory inventory;
    List<ItemSlotUI> slotUIList;
    RectTransform itemListRect;

    int selectedPocket = 0;

    const int itemsInViewPort = 7;
    
    const int nPockets = 8;

    private void Awake()
    {
        inventory = FindObjectOfType<PlayerController>().GetComponent<Inventory>();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
        UpdateSelectionInUI();
        inventory.OnUpdated += UpdateItemList;
        inventory.OnUpdated += UpdateSelectionInUI;
    }

    void UpdateItemList()
    {
        // Clear exisiting items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetItemsByCategory(selectedPocket))
        {
            var newSlotUI = Instantiate(itemSlotUI, itemList.transform);
            newSlotUI.SetData(itemSlot);
            slotUIList.Add(newSlotUI);
        }

        SetItems(slotUIList.Select(s => s.GetComponent<InventoryItemSlot>()).ToList());
        UpdateSelectionInUI();
    }

    public override void HandleUpdate()
    {
        int prevPocket = selectedPocket;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selectedPocket;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedPocket;

        if (selectedPocket >= nPockets)
            selectedPocket = 0;
        else if (selectedPocket < 0)
            selectedPocket = nPockets - 1;

        if (selectedPocket != prevPocket)
        {
            ResetOnPocketChange();
            UpdateItemList();
            pocketUI.UpdatePocket(selectedPocket);
            AudioManager.i.PlaySFX(AudioID.ChangePocket);
            UpdateSelectionInUI();
        }
        base.HandleUpdate();
    }

    protected override void UpdateSelectionInUI()
    {
        var items = inventory.GetItemsByCategory(selectedPocket);
        selection = MyClamp(selection, 0, items.Count - 1);

        if (items.Count > 0)
        {
            itemDescriptionBar.SetData(items[selection].ItemBase);
            HandleScrolling();
        }
        else
        {
            itemDescriptionBar.ClearFields();
        }
        base.UpdateSelectionInUI();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewPort) return;
        float scrollPos = Mathf.Clamp(selection - (itemsInViewPort / 2), 0, selection) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }

    void ResetOnPocketChange()
    {
        itemDescriptionBar.ClearFields();
        selection = 0;
    }

    public ItemBase SelectedItem => inventory.GetItemBase(selection, selectedPocket);

    public int SelectedPocket => selectedPocket;

    public static int MyClamp(int n, int min, int max)
    {
        if (max < min || n < min)
            return min;
        if (n > max)
            return max;

        return n;
    }
}


