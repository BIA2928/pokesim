using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{

    [SerializeField] PokemonBase _base;
    [SerializeField] int _level;
    public PokemonBase Base { get { return _base; }  }
    public int Level { get { return _level; } }

    public int HP { get; set; }

    public List<Move> Moves { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public void Init()
    {
        


        // Generate moves from potential moves at the given level
        List<Move> potentialMoves = new List<Move>();
        Moves = new List<Move>();
        foreach (var move in _base.LearnableMoves)
        {
            if (move.LearnLevel <= _level)
            {
                potentialMoves.Add(new Move(move.Base));
            }
        }

        if (potentialMoves.Count <= 4)
        {
            Moves = potentialMoves;
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                int randomInt = Random.Range(0, potentialMoves.Count);
                Moves.Add(potentialMoves[randomInt]);
                potentialMoves.RemoveAt(randomInt);
            }
        }

        CalculateStats();
        HP = MaxHP;

        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defence, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefence, 0},
            {Stat.Speed, 0}
        };
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defence, Mathf.FloorToInt((Base.Defence * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefence, Mathf.FloorToInt((Base.SpDefence * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10;

    }

    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //multipliers
        int boost = StatBoosts[stat];

        /*                              0    1    2    3    4    5    6 */
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal *= Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal /= Mathf.FloorToInt(statVal * boostValues[Mathf.Abs(boost)]);

        return statVal;
    }

    public int MaxHP
    {
        get; private set;
    }
    public int Attack
    {
        get { return Stats[Stat.Attack]; }
    }

    public int Defence
    {
        get { return Stats[Stat.Defence]; }
    }

    public int SpAttack
    {
        get { return Stats[Stat.SpAttack]; }
    }

    public int SpDefence
    {
        get { return Stats[Stat.SpDefence]; }
    }

    public int Speed
    {
        get { return Stats[Stat.Speed]; }
    }

    public DamageDetails TakeDamage(Move move, Pokemon Attacker)
    {
        float crit = 1f;
        if (Random.value * 100f <= 6.25f)
        {
            crit = 2f;
        }

        float typeModifier = TypeChart.GetTypeEffectiveness(move.Base.Type, this.Base.Type1);
        typeModifier *= TypeChart.GetTypeEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = typeModifier,
            Crit = crit,
            DidFaint = false
        };

        float attackStat = (float)Attack;
        int defenceStat = Defence;

        if (move.Base.MoveType == MoveType.Special)
        {
            attackStat = (float)SpAttack;
            defenceStat = SpDefence;
        }

        // Uses standard pokemon series damage calculator
        float modifiers = Random.Range(0.85f, 1f) * typeModifier * crit;

        float a = (2 * Attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attackStat/ defenceStat) + 2;
        int damage = Mathf.FloorToInt(modifiers * d);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            damageDetails.DidFaint = true;
        }
        return damageDetails;
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }

    public class DamageDetails
    {

        public float Crit { get; set; }
        public bool DidFaint { get; set; }

        public float TypeEffectiveness { get; set; }

    }
}
