using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create a new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string moveName;

    [TextArea][SerializeField] string description;

    [SerializeField] PokeType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] bool neverMiss;
    [SerializeField] int pp;
    [SerializeField] int movePriority;
    [SerializeField] MoveType moveType;
    [SerializeField] MoveEffects effects;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] MoveTarget target;

    

    
    public bool NeverMiss
    {
        get { return neverMiss; }
    }

    public int Priority
    {
        get { return movePriority; }
    }

    public List<SecondaryEffects> SecondEffects {  get { return secondaryEffects; } }

    public string Name
    {
        get { return moveName; }

    }

    public string Description
    {
        get { return description; }

    }

    public PokeType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power;  }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public int Pp
    {
        get { return pp; }
    }

    public MoveType MoveType
    {
        get { return moveType;  }
    }

    public MoveEffects Effects
    {
        get { return effects; }
    }

    public MoveTarget Target
    {
        get { return target; }
    }


}

public enum MoveType
{
    Physical,
    Special,
    Status,
    Other
}
public enum MoveTarget
{
    Foe, Self
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionType cnd;
    [SerializeField] ConditionType volatileCnd;

    public List<StatBoost> Boosts
    {
        get { return boosts;  }
    }

    public ConditionType Cnd
    {
        get { return cnd;  }
    }

    public ConditionType VolatileCnd
    {
        get { return volatileCnd; }
    }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance {  get { return chance; } }
    public MoveTarget Target { get { return target; } }
}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}
