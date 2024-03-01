using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB 
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;
            condition.CndType = conditionId;
        }
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
                            return true;
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
            },
            // Volatile Statuses
            {
                ConditionType.confusion, new Condition()
                {
                    Name = "Confusion",
                    StartMessage = "is confused",
                    OnCndStart = (Pokemon poke) =>
                    {
                        poke.VolatileCndTime = Random.Range(1,4);
                        Debug.Log($"{poke.Base.Name} will be confused for {poke.VolatileCndTime} moves");
                    },
                    OnStartOfTurn = (Pokemon poke) =>
                    {
                        if (poke.VolatileCndTime <= 0)
                        {
                            poke.CureVolatileCondition();
                            poke.StatusChanges.Enqueue($"{poke.Base.Name} snapped out of confusion!");
                            return true;
                        }
                        poke.VolatileCndTime--;

                        // 50% of move completion, otherwise self damage
                        poke.StatusChanges.Enqueue($"{poke.Base.Name} is confused...");
                        if (Random.Range(1,3) == 2)
                        {
                            return true;
                        }

                        // Self damage
                        poke.UpdateHP(-(poke.MaxHP / 9));
                        poke.StatusChanges.Enqueue($"It hurt itself in its confusion!");
                        return false;
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
    frz,
    confusion
}