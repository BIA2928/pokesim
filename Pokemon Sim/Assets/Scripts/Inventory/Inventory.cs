using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public ItemBase UseItem(int selectedItemIndex, Pokemon selectedPokemon)
    {
        var currItem = medicineItems[selectedItemIndex].ItemBase;
        bool successful = currItem.Use(selectedPokemon);

        if (successful)
        {
            RemoveOneOfItem(currItem);
            return currItem;
        }

        return null;

    }

    public List<ItemSlot> GetItemsByCategory(int selectedCategory)
    {
        return pocketList[selectedCategory];
    }
     
    public void RemoveOneOfItem(ItemBase item)
    {
        var currItemSlot = medicineItems.First(slot => slot.ItemBase == item);

        currItemSlot.Count--;

        if (currItemSlot.Count == 0)
        {
            medicineItems.Remove(currItemSlot);
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

    public ItemBase ItemBase => item;
    public int Count
    {
        get => count;
        set => count = value;
    }
}
