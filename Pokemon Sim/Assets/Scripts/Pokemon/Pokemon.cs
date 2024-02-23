using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon 
{
    public PokemonBase Base { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }
    
    public List<Move> Moves { get; set; }
    public Pokemon(PokemonBase _base, int level)
    {
        this.Base = _base;
        this.Level = level;
        HP = MaxHP;


        // Generate moves from potential moves at the given level
        List<Move> potentialMoves = new List<Move>();
        Moves = new List<Move>();
        foreach (var move in _base.LearnableMoves)
        {
            if (move.LearnLevel <= level)
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
    }

    public int MaxHP
    {
        get { return Mathf.FloorToInt((Base.MaxHP * Level) / 100f) + 10;  }
    }
    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }

    public int Defence
    {
        get { return Mathf.FloorToInt((Base.Defence * Level) / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5; }
    }

    public int SpDefence
    {
        get { return Mathf.FloorToInt((Base.SpDefence * Level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
    }

    public bool TakeDamage(Move move, Pokemon Attacker)
    {
        // Uses standard pokemon series damage calculator
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * Attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)Attacker.Attack / Defence) + 2;
        int damage = Mathf.FloorToInt(modifiers * d);

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}
