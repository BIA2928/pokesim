using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new pokeball item")]
public class PokeballItem : ItemBase
{
    override public bool Use(Pokemon pokemon)
    {
        return true;
    }
}
