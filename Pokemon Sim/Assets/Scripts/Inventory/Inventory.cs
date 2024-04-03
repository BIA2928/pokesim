using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemType { HoldableItem, MedicineItem, Pokeball, Tms, Berries, Mail, BattleItem, KeyItem}

public class Inventory : MonoBehaviour, ISavable
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

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            genItems = generalItems.Select(i => i.GetItemSaveData()).ToList(),
            medItems = medicineItems.Select(i => i.GetItemSaveData()).ToList(),
            pokeballs = pokeballItems.Select(i => i.GetItemSaveData()).ToList(),
            tms = tmItems.Select(i => i.GetItemSaveData()).ToList(),
            keys = keyItems.Select(i => i.GetItemSaveData()).ToList(),
            berries = berryItems.Select(i => i.GetItemSaveData()).ToList(),
            mails = mailItems.Select(i => i.GetItemSaveData()).ToList(),
            battles = berryItems.Select(i => i.GetItemSaveData()).ToList()

        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;

        generalItems = saveData.genItems.Select(i => new ItemSlot(i)).ToList();
        medicineItems = saveData.medItems.Select(i => new ItemSlot(i)).ToList();
        pokeballItems = saveData.pokeballs.Select(i => new ItemSlot(i)).ToList();
        tmItems = saveData.tms.Select(i => new ItemSlot(i)).ToList();
        keyItems = saveData.keys.Select(i => new ItemSlot(i)).ToList();
        berryItems = saveData.berries.Select(i => new ItemSlot(i)).ToList();
        battleItems = saveData.battles.Select(i => new ItemSlot(i)).ToList();
        mailItems = saveData.mails.Select(i => new ItemSlot(i)).ToList();

        pocketList = new List<List<ItemSlot>>()
        {
            generalItems, medicineItems, pokeballItems, tmItems, mailItems,
            berryItems, battleItems, keyItems
        };

        OnUpdated?.Invoke();
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

    public ItemSaveData GetItemSaveData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.Name,
            count = this.count,
        };
        return saveData;
    }

    public ItemSlot(ItemSaveData sD)
    {
        this.count = sD.count;
        item = ItemDB.GetMoveByName(sD.name);
    }

    public ItemSlot()
    {

    }
}

[Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}

[Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> genItems;
    public List<ItemSaveData> medItems;
    public List<ItemSaveData> pokeballs;
    public List<ItemSaveData> tms;
    public List<ItemSaveData> mails;
    public List<ItemSaveData> berries;
    public List<ItemSaveData> battles;
    public List<ItemSaveData> keys;
}
