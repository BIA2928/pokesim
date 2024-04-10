using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pokemon
{

    [SerializeField] PokemonBase _base;
    [SerializeField] int _level;
    public PokemonBase Base { get { return _base; } }
    public int Level { get { return _level; } }

    public int HP { get; set; }

    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Condition Cnd { get; private set; }
    public Condition VolatileCnd { get; private set; }
    public int StatusCndTime { get; set; }
    public int VolatileCndTime { get; set; }

    public event System.Action OnStatusCndChange;
    public event System.Action OnHPChanged;

    public int Exp { get; set; }
    public Queue<string> StatusChanges { get; private set; }

    public Pokemon(PokemonBase _base, int level)
    {
        this._base = _base;
        _level = level;
        Init();
    }
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
            for (int i = 0; i < PokemonBase.MaxNMoves; i++)
            {
                int randomInt = Random.Range(0, potentialMoves.Count);
                Moves.Add(potentialMoves[randomInt]);
                potentialMoves.RemoveAt(randomInt);
            }
        }

        Exp = Base.GetExpForLevel(_level);

        StatusChanges = new Queue<string>();
        CalculateStats();
        HP = MaxHP;
        ResetStats();
        VolatileCnd = null;
        Cnd = null;
    }

    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>
        {
            { Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5 },
            { Stat.Defence, Mathf.FloorToInt((Base.Defence * Level) / 100f) + 5 },
            { Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5 },
            { Stat.SpDefence, Mathf.FloorToInt((Base.SpDefence * Level) / 100f) + 5 },
            { Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5 }
        };

        MaxHP = Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10 + Level;

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

    public PokemonSaveData GetSaveData()
    {
        PokemonSaveData sD = new PokemonSaveData()
        {
            level = _level,
            baseName = Base.name,
            hp = this.HP,
            exp = Exp,
            condition = Cnd?.CndType,
            moveSaveDatas = Moves.Select(m => m.GetSaveData()).ToList()
        };

        
        return sD;
    }
    
    public Pokemon(PokemonSaveData sD)
    {
        _base = PokemonDB.LookupByName(sD.baseName);
        HP = sD.hp;
        _level = sD.level;
        Exp = sD.exp;

        if (sD.condition != null)
            Cnd = ConditionsDB.Conditions[sD.condition.Value];
        else
            Cnd = null;

        Moves = sD.moveSaveDatas.Select(m => new Move(m)).ToList();
        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStats();
        VolatileCnd = null;
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

        Debug.Log($"Move effectiveness is {typeModifier}, crit is {crit}");

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
        float d = a * move.Base.Power * ((float)attackStat / defenceStat) + 2;
        int damage = Mathf.FloorToInt(modifiers * d);

        UpdateHP(-damage);
        return damageDetails;
    }

    public LearnableMove GetMoveAtCurrLevel()
    {
        return Base.LearnableMoves.Where(x => x.LearnLevel == _level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove move)
    {
        Moves.Add(new Move(move.Base));
    }

    public void LearnMove(MoveBase moveBase)
    {
        Moves.Add(new Move(moveBase));
    }

    public void SetCondition(ConditionType cndType)
    {
        if (Cnd != null)
        {
            StatusChanges.Enqueue($"{Base.Name} is already affected by {Cnd.Name.ToLowerInvariant()}.");
            return;
        }


        Cnd = ConditionsDB.Conditions[cndType];
        Cnd?.OnCndStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Cnd.StartMessage}.");
        OnStatusCndChange?.Invoke();
    }

    public void CureCondition()
    {
        Cnd = null;
        OnStatusCndChange?.Invoke();
    }

    public void SetVolatileCondition(ConditionType cndType)
    {
        if (VolatileCnd != null)
            return;

        VolatileCnd = ConditionsDB.Conditions[cndType];
        VolatileCnd?.OnCndStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileCnd.StartMessage}.");
    }

    public void CureVolatileCondition()
    {
        VolatileCnd = null;
    }

    void ResetStats()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0},
            {Stat.Defence, 0},
            {Stat.SpAttack, 0},
            {Stat.SpDefence, 0},
            {Stat.Speed, 0},
            {Stat.Accuracy, 0},
            {Stat.Evasiveness, 0}
        };
    }

    /// <summary>
    /// Updates the hp of the pokemon given an integer, damage values are negative, heals positive.
    /// </summary>
    /// <param name="hpChange">The amount of hp to change.</param>
    public void UpdateHP(int hpChange)
    {
        HP = Mathf.Clamp(HP + hpChange, 0, MaxHP);
        OnHPChanged?.Invoke();
    }

    public void SetHpByPercentage(float hpPercentage)
    {
        int currHp = Mathf.FloorToInt(hpPercentage * MaxHP);
        currHp = Mathf.Clamp(currHp, 0, MaxHP);
        HP = currHp;
    }

    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();
        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public bool OnStartOfTurn()
    {
        bool canPerformMove = true;
        if (Cnd?.OnStartOfTurn != null)
        {
            if (!Cnd.OnStartOfTurn(this))
                canPerformMove = false;
        }

        if (VolatileCnd?.OnStartOfTurn != null)
        {
            if (!VolatileCnd.OnStartOfTurn(this))
                canPerformMove = false;
        }


        return canPerformMove;
    }

    public void OnEndOfTurn()
    {
        Cnd?.OnEndOfTurn?.Invoke(this);
        VolatileCnd?.OnEndOfTurn?.Invoke(this);
    }


    public void ApplyBoost(List<StatBoost> boosts)
    {
        foreach (var statboost in boosts)
        {
            var stat = statboost.stat;
            var boost = statboost.boost;
            StatBoosts[statboost.stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
            {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            }
            else
            {
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");
            }
        }


    }

    public Evolution CheckForEvolution()
    {
        var evos = Base.Evolutions.FirstOrDefault(e => e.RequiredLevel <= _level);
        if (evos != null && evos.RequiredItem == null)
            return evos;
        return null;
    }

    public Evolution CheckForEvolution(ItemBase itemToCheck)
    {
        return Base.Evolutions.FirstOrDefault(e => e.RequiredItem == itemToCheck);
    }

    public void Evolve(Evolution evo)
    {
        _base = evo.EvolvesInto;
        CalculateStats();
    }

    
    public void OnBattleOver()
    {
        VolatileCnd = null;
        ResetStats();
    }

    public bool LevelUp()
    {
        if (Exp > Base.GetExpForLevel(_level + 1))
        {
            // Level up
            _level++;
            UpdateStatsOnLevelUp();
            return true;
        }
        return false;
    }

    void UpdateStatsOnLevelUp()
    {
        var prevMaxHp = MaxHP;
        CalculateStats();
        var diff = MaxHP - prevMaxHp;

        UpdateHP(diff);
    }

    void ForgetMove(Move move)
    {
        Moves.Remove(move);
    }

    public bool AllMovesFullPP()
    {
        foreach (Move move in Moves)
        {
            if (move.PP != move.Base.Pp)
                return false;
        }
        return true;
    }

    public void ReplaceMove(Move toBeReplaced, Move newMove)
    {
        int i;
        for (i = 0; i < Moves.Count; i++)
        {
            if (Moves[i] == toBeReplaced)
                break;
        }

        Moves[i] = newMove;

    }

    public bool HasMove(MoveBase move)
    {
        return this.Moves.Count(m => m.Base == move) > 0;
    }


}

public class DamageDetails
{
    public float Crit { get; set; }
    public bool DidFaint { get; set; }
    public float TypeEffectiveness { get; set; }
}

[System.Serializable]
public class PokemonSaveData
{
    public int hp;
    public int level;
    public int exp;
    public ConditionType? condition;
    public string baseName;
    public List<MoveSaveData> moveSaveDatas;
}


