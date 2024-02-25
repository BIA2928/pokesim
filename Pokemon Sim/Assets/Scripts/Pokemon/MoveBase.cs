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
    [SerializeField] int pp;

    [SerializeField] MoveType moveType;

    

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


}

public enum MoveType
{
    Physical,
    Special,
    Other
}
