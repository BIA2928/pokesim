using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create a new evolution item")]
public class EvolutionItem : GeneralItem
{
    public override bool CanUseInBattle => false;
    public override bool CanUseOutsideBattle => true;
    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
}
