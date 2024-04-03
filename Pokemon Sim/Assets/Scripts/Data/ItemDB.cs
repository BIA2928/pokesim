using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{

    static Dictionary<string, ItemBase> items;

    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();

        var itemList = Resources.LoadAll<ItemBase>("");
        foreach (var item in itemList)
        {
            if (items.ContainsKey(item.Name))
            {
                Debug.Log($"Repeat instance of {item.Name}");
                continue;
            }
            items[item.Name] = item;
        }
    }


    public static ItemBase GetMoveByName(string lookupName)
    {
        if (!items.ContainsKey(lookupName))
        {
            Debug.Log($"{lookupName} does not exist in ItemsDB");
            return null;
        }
        return items[lookupName];
    }
}
