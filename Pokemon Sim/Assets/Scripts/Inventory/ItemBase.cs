using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] string description;
    [SerializeField] Sprite bagIcon;
    [SerializeField] int price = 10;
    [SerializeField] bool isSellable = true;

    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;

    public virtual bool IsResuable => false;

    public string Name => name;
    public string Description => description;
    public Sprite BagIcon => bagIcon;
    public int Price => price;
    public bool IsSellable => isSellable;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }


}
