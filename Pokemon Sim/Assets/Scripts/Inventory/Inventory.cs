using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemType { HoldableItem, MedicineItem, Pokeball, Tms, Berries, Mail, BattleItem, KeyItem}

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> generalItems;
    [SerializeField] List<ItemSlot> medicineItems;
    [SerializeField] List<ItemSlot> pokeballItems;
    [SerializeField] List<ItemSlot> tmItems;
    [SerializeField] List<ItemSlot> mailItems;
    [SerializeField] List<ItemSlot> berryItems;
    [SerializeField] List<ItemSlot> battleItems;
    [SerializeField] List<ItemSlot> keyItems;

    public event Action OnUpdated;

    List<List<ItemSlot>> pocketList;

    private void Awake()
    {
        pocketList = new List<List<ItemSlot>>()
        {
            generalItems, medicineItems, pokeballItems, tmItems, mailItems,
            berryItems, battleItems, keyItems
        };
    }

    public ItemBase UseItem(int selectedItemIndex, Pokemon selectedPokemon, int selectedPocket)
    {
        var pocketItems = GetItemsByCategory(selectedPocket);
        var currItem = pocketItems[selectedItemIndex].ItemBase;
        bool successful = currItem.Use(selectedPokemon);

        if (successful)
        {
            if (!currItem.IsResuable)
                RemoveOneOfItem(currItem, selectedPocket);
            return currItem;
        }

        return null;

    }

    public List<ItemSlot> GetItemsByCategory(int selectedCategory)
    {
        return pocketList[selectedCategory];
    }

    public ItemBase GetItemBase(int selectedItem, int selectedPocket)
    {
        var item = GetItemsByCategory(selectedPocket)[selectedItem].ItemBase;
        return item;
    }
     
    public void RemoveOneOfItem(ItemBase item, int selectedPocket)
    {
        var pocketItems = GetItemsByCategory(selectedPocket);
        var currItemSlot = pocketItems.First(slot => slot.ItemBase == item);

        currItemSlot.Count--;

        if (currItemSlot.Count == 0)
        {
            pocketItems.Remove(currItemSlot);
        }
        OnUpdated?.Invoke();
    }

    public ItemType GetPocketForItem(ItemBase item)
    {
        if (item is MedicineItem)
            return ItemType.MedicineItem;
        else if (item is PokeballItem)
            return ItemType.Pokeball;
        else if (item is TmItem)
            return ItemType.Tms;
        return ItemType.Berries;
    }

    public void AddItem(ItemBase item, int count=1)
    {
        var itemPocketIndex = (int)GetPocketForItem(item);
        var slots = GetItemsByCategory(itemPocketIndex);

        var itemSlot = slots.FirstOrDefault(slot => slot.ItemBase == item);
        if (itemSlot == null)
        {
            slots.Add(new ItemSlot()
            {
                ItemBase = item,
                Count = count
            });
        }
        else
        {
            itemSlot.Count += count;
        }

        OnUpdated?.Invoke();
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}


[System.Serializable]
public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase ItemBase
    { 
        get => item;
        set => item = value;
    }
    
    public int Count
    {
        get => count;
        set => count = value;
    }
}
