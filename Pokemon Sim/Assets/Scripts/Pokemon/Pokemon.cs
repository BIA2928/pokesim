using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon 
{
    PokemonBase _base;
    int level;

    public int HP { get; set; }
    
    public List<Move> Moves { get; set; }
    public Pokemon(PokemonBase _base, int level)
    {
        this._base = _base;
        this.level = level;
        HP = _base.MaxHP;


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
        get { return Mathf.FloorToInt((_base.MaxHP * level) / 100f) + 10;  }
    }
    public int Attack
    {
        get { return Mathf.FloorToInt((_base.Attack * level) / 100f) + 5; }
    }

    public int Defence
    {
        get { return Mathf.FloorToInt((_base.Defence * level) / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((_base.SpAttack * level) / 100f) + 5; }
    }

    public int SpDefence
    {
        get { return Mathf.FloorToInt((_base.SpDefence * level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((_base.Speed * level) / 100f) + 5; }
    }
}
