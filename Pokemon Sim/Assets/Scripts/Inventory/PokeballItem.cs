using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new pokeball item")]
public class PokeballItem : ItemBase
{
    [SerializeField] float catchRateModifier = 1;

    public float CatchRateModifier => catchRateModifier;
    override public bool Use(Pokemon pokemon)
    {
        if (GameController.i.GameState == GameState.InBattle)
        {
            return true;
        }
        return false;
    }

    public override bool CanUseOutsideBattle => false;
}
