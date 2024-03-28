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

    [SerializeField] PocketSelectorUI pocketUI;

    Action OnItemUsed;
    Inventory inventory;
    List<ItemSlotUI> slotUIList;
    RectTransform itemListRect;

    int selectedItem = 0;
    int selectedPocket = 0;
    
    InventoryUIState state;

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
        UpdateItemSelection();
        inventory.OnUpdated += UpdateItemList;
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
    }
    public void HandleUpdate(Action onBack, Action onItemUsed=null)
    {
        OnItemUsed = onItemUsed;
        
        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevPocket = selectedPocket;
            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedPocket;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedPocket;

            if (selectedPocket >= nPockets)
                selectedPocket = 0;
            else if (selectedPocket < 0)
                selectedPocket = nPockets - 1;
            selectedItem = MyClamp(selectedItem, 0, inventory.GetItemsByCategory(selectedPocket).Count - 1);

            if (selectedItem != prevSelection)
            {
                UpdateItemSelection();
            }
            else if (selectedPocket != prevPocket)
            {
                ResetOnPocketChange();
                UpdateItemList();
                pocketUI.UpdatePocket(selectedPocket);
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
                StartCoroutine(UseItem());
                
            };
            partyScreen.HandleUpdate(onSelected, OnBack);
        }
        
    }

    void UpdateItemSelection()
    {
        var items = inventory.GetItemsByCategory(selectedPocket);
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].SelectItem();
            }
            else
                slotUIList[i].DeselectItem();
        }

        selectedItem = MyClamp(selectedItem, 0, items.Count - 1);
        if (items.Count > 0)
        {
            itemDescriptionBar.SetData(items[selectedItem].ItemBase);
            HandleScrolling();
        }

    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= itemsInViewPort) return;
        float scrollPos = Mathf.Clamp(selectedItem - (itemsInViewPort / 2), 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }

    void ResetOnPocketChange()
    {
        itemDescriptionBar.ClearFields();
        selectedItem = 0;
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

    IEnumerator UseItem() 
    {
        state = InventoryUIState.Busy;
        // Use item on selected poke
        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember);
        if (usedItem == null)
        {
            yield return DialogueManager.Instance.ShowDialogue($"It won't have any effect.");
        }
        else
        {
            //if (usedItem.GetType().Equals(typeof(MedicineItem)))
            yield return DialogueManager.Instance.ShowDialogue($"One {usedItem.Name} was used.");
            OnItemUsed?.Invoke();
        }

        ClosePartyScreen();
    }

    public static int MyClamp(int n, int min, int max)
    {
        if (max < min || n < min)
            return min;
        if (n > max)
            return max;

        return n;
    }
}


