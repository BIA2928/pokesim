using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{


    [SerializeField] new string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokeSize pokeSize;

    [SerializeField] int regionalDexNum;
    [SerializeField] int nationalDexNum;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite boxSprite;

    [SerializeField] PokeType type1;
    [SerializeField] PokeType type2;

    // Base Stats 
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int spAttack;
    [SerializeField] int spDefence;
    [SerializeField] int speed;

    [SerializeField] [Range(0, 255)] int catchRate;

    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] List<MoveBase> tmMoves;
    [SerializeField] int expYield;
    [SerializeField] GrowthRate growthRate;

    public int GetExpForLevel(int level)
    {
        float nCubed = Mathf.Pow(level, 3);
        if (growthRate == GrowthRate.Fast)
        {
            // 4n^3/5
            return Mathf.FloorToInt(0.8f * nCubed);
        }
        else if (growthRate == GrowthRate.Medium)
        {
            // 4^3
            return Mathf.FloorToInt(nCubed);
        }
        else
        {
            //5/4 * n^3
            return Mathf.FloorToInt(nCubed * 1.2f);
        }
    }

    public static int MaxNMoves { get; set; } = 4;
    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public PokeSize PokeSize
    {
        get { return pokeSize; }
    }

    public int MaxHP
    {
        get { return maxHP; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defence
    {
        get { return defence; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefence
    {
        get { return spDefence; }
    }
    public int Speed
    {
        get { return speed; }
    }

    public int RegionalDexNum
    {
        get { return regionalDexNum; }
    }

    public int NationalDexNum
    {
        get { return nationalDexNum; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BoxSprite
    {
        get { return boxSprite; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }

    public PokeType Type1
    {
        get { return type1; }
    }

    public PokeType Type2
    {
        get { return type2; }
    }

    public int CatchRate
    {
        get { return catchRate; }
    }

    public int ExpYield
    {
        get { return expYield; }
    }

    public GrowthRate GrowthRate
    {
        get { return growthRate; }

    }

    public List<MoveBase> TmMoves => tmMoves;

    public bool CanLearnByTm(MoveBase attemptedToLearn)
    {
        return tmMoves.Contains(attemptedToLearn);
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int learnLevel;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int LearnLevel
    {
        get { return learnLevel; }
    }
}


public enum Stat
{
    Attack,
    Defence,
    SpAttack,
    SpDefence,
    Speed,

    Evasiveness,
    Accuracy
}

public enum PokeSize
{
    S,
    M,
    L,
    XL
}
public enum PokeType
{
    None,
    Bug,
    Dark,
    Dragon,
    Electric,
    Fairy,
    Fighting,
    Fire,
    Flying,
    Ghost,
    Grass,
    Ground,
    Ice,
    Normal,
    Rock,
    Poison,
    Psychic,
    Steel,
    Water
}

public class TypeChart
{
    // Attack ->
    // Bug0,Dar1,Drag2,Ele3,Fair4,Figh5,
    static float[][] typeChart =
    {                     /*BUG  DARK  DRAG  ELEC  FAIR  FIGH  FIRE  FLY   GHO   GRA   GRO   ICE   NORM  ROCK  POI   PSY   STE    WAT 
        /*Bug*/new float[] {1.0f,2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 0.5f, 0.5f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 0.5f, 1.0f},
        /*Dark*/new float[] {1f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f},
        /*Drag*/new float[] {1f, 1.0f, 2.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f},
        /*Elec*/new float[] {1f, 1.0f, 0.5f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f},
        /*Fair*/new float[] {1f, 2.0f, 2.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 0.5f, 1.0f},
        /*Figh*/new float[]{0.5f,2.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 0.0f, 1.0f, 1.0f, 2.0f, 2.0f, 2.0f, 0.5f, 0.5f, 2.0f, 1.0f},
        /*Fire*/new float[] {2f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 2.0f, 0.5f, 1.0f, 1.0f, 2.0f, 0.5f},
        /*Fly*/new float[] {2.0f,1.0f, 1.0f, 0.5f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f},
        /*Ghost*/new float[]{1f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f},
        /*Gra*/new float[] {0.5f,1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 0.5f, 2.0f},
        /*Grou*/new float[]{0.5f,1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 2.0f, 0.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 2.0f, 1.0f, 2.0f, 1.0f},
        /*Ice*/new float[]{1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 2.0f, 2.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 0.5f},
        /*Norm*/new float[] {1f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f},
        /*Rock*/new float[] {2f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f, 2.0f, 2.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 0.5f, 1.0f},
        /*Pois*/new float[] {1f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 1.0f, 1.0f, 0.5f, 0.5f, 1.0f, 0.0f, 1.0f},
        /*Psyc*/new float[] {1f, 0.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 0.5f, 0.5f, 1.0f},
        /*Steel*/new float[]{1f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 0.5f},
        /*Water*/new float[]{1f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 0.5f, 2.0f, 1.0f, 1.0f, 2.0f, 1.0f, 1.0f, 1.0f, 0.5f}
    };

    public static float GetTypeEffectiveness(PokeType attackType, PokeType defenceType)
    {
        if (attackType == PokeType.None || defenceType == PokeType.None)
        {
            return 1.0f;
        }

        int row = (int)attackType - 1;
        int column = (int)defenceType - 1;
        return typeChart[row][column];
    }
}

public enum GrowthRate
{
    Fast,
    Medium,
    Slow
}