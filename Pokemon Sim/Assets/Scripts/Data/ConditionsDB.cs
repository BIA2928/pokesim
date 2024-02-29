using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{

    static void PoisonEffect(Pokemon poke)
    {

    }

    static void BurnEffect(Pokemon poke)
    {

    }

    public static Dictionary<ConditionType, Condition> Conditions { get; set; } = 
        new Dictionary<ConditionType, Condition>
        {
            {
                ConditionType.psn, new Condition()
                {
                    Name = "Poison",
                    StartMessage = "is poisoned",
                    OnEndOfTurn = (Pokemon pokemon) => {
                        pokemon.UpdateHP(-(pokemon.MaxHP / 8));
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} was hurt from poison.");
                    }
                } 
            },
            {
                ConditionType.brn, new Condition()
                {
                    Name = "Burned",
                    StartMessage = "is burned",
                    OnEndOfTurn = (Pokemon pokemon) =>
                    {
                        pokemon.UpdateHP(-(pokemon.MaxHP / 16));
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} was hurt by its burn!");
                    }
                } 
            },
            {
                ConditionType.slp, new Condition()
                {
                    Name = "Asleep",
                    StartMessage = "is fast asleep",
                    OnCndStart = (Pokemon poke) =>
                    {
                        poke.StatusCndTime = Random.Range(1,4);
                        Debug.Log($"{poke.Base.Name} will sleep for {poke.StatusCndTime} moves");
                    },
                    OnStartOfTurn = (Pokemon poke) =>
                    {
                        if (poke.StatusCndTime <= 0)
                        {
                            poke.CureCondition();
                            poke.StatusChanges.Enqueue($"{poke.Base.Name} woke up!");
                        }
                        poke.StatusCndTime--;
                        poke.StatusChanges.Enqueue($"{poke.Base.Name} is fast asleep.");
                        return false;
                    }

                }
            },
            {
                ConditionType.frz, new Condition()
                {
                    Name = "Frozen",
                    StartMessage = "is frozen solid",
                    OnStartOfTurn = (Pokemon pokemon) =>
                    {
                        if (Random.Range(0,4) == 3)
                        {
                            pokemon.CureCondition();
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} thawed out!");
                            return true;
                        }

                        return false;
                    }
                }
            },
            {
                ConditionType.par, new Condition()
                {
                    Name = "Paralysis",
                    StartMessage = "is paralyzed",
                    OnStartOfTurn = (Pokemon pokemon) =>
                    {
                        // 1 in 4 chance of not moving
                        if (Random.Range(0,4) == 3)
                        {
                            pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is paralyzed and can't move!");
                            return false;
                        }
                        return true;
                    }
                }
            }
        };
}

public enum ConditionType
{
    none,
    psn,
    brn,
    slp,
    par,
    frz
}