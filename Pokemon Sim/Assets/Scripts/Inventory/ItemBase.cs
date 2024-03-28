using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] string description;
    [SerializeField] Sprite bagIcon;

    public string Name => name;
    public string Description => description;
    public Sprite BagIcon => bagIcon;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }

}
