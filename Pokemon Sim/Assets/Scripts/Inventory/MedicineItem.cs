using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Items/Create new medicine item")]
public class MedicineItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;

    [Header("Status Conditions")]
    [SerializeField] ConditionType status;
    [SerializeField] bool recoverAllStatuses;

    [Header("Revives")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;


    override public bool Use(Pokemon pokemon) 
    { 
        if (revive || maxRevive)
        {
            if (pokemon.HP > 0)
            {
                return false;
            }
            if (revive)
            {
                pokemon.UpdateHP(pokemon.MaxHP / 2);
            }
            else
            {
                pokemon.UpdateHP(pokemon.MaxHP);
            }
            pokemon.CureCondition();
            return true;
        }

        if (pokemon.HP <= 0)
            return false;

        if (restoreMaxHP || hpAmount > 0)
        {
            if (pokemon.HP == pokemon.MaxHP)
                return false;

            if (restoreMaxHP)
            {
                pokemon.UpdateHP(pokemon.MaxHP);
            }
            else
            {
                pokemon.UpdateHP(hpAmount);
            }
        }

        if (recoverAllStatuses || status != ConditionType.none)
        {
            if (pokemon.Cnd == null && pokemon.VolatileCnd == null)
                return false;
            if (recoverAllStatuses)
            {
                pokemon.CureCondition();
                pokemon.CureVolatileCondition();
            }
            else
            {
                if (pokemon.Cnd.CndType == status)
                {
                    pokemon.CureCondition();
                }
                else if (pokemon.VolatileCnd.CndType == status)
                {
                    pokemon.CureVolatileCondition();
                }
                else 
                {
                    return false;
                }
            }
                
        }

        if (restoreMaxPP || ppAmount > 0)
        {
            if (pokemon.AllMovesFullPP())
            {
                return false;
            }
            int restoreAmount = ppAmount;
            if (restoreMaxPP)
            {
                restoreAmount = 100;
            }
            foreach (Move move in pokemon.Moves)
            {
                move.RestorePP(restoreAmount);
            }
        }
        
        return false;
    }

}
