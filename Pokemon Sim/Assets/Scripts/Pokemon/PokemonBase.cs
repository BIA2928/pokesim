using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;
    
    [TextArea]
    [SerializeField] string description;

    [SerializeField] int nationalDexNum;
    [SerializeField] int pokedexNum;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] PokeType type1;
    [SerializeField] PokeType type2;

    // Base Stats 
    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int defence;
    [SerializeField] int spAttack;
    [SerializeField] int spDefence;
    [SerializeField] int speed;

    [SerializeField] List<LearnableMove> learnableMoves;


    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
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
        get { return Speed; }
    }

    public int NationalDexNum
    {
        get { return nationalDexNum; }
    }

    public int PokedexNum
    {
        get { return pokedexNum; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
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


public enum PokeType
{
    None,
    Grass,
    Water,
    Fire,
    Electric,
    Flying,
    Ground,
    Ice,
    Fighting,
    Rock,
    Poison,
    Normal,
    Dragon,
    Ghost,
    Dark,
    Psychic,
    Fairy,
    Bug
}