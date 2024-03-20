using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB 
{
    static Dictionary<string, MoveBase> moveDict;

    public static void Init()
    {
        moveDict = new Dictionary<string, MoveBase>();
        var moveArray = Resources.LoadAll<MoveBase>("");
        foreach (var move in moveArray)
        {
            if (moveDict.ContainsKey(move.Name))
            {
                Debug.Log($"Two instances of {move.Name}");
                continue;
            }

            moveDict[move.Name] = move;
        }
    }

    public static MoveBase LookUpByName(string name)
    {
        if (!moveDict.ContainsKey(name))
        {
            Debug.Log($"move with name {name} does not exist in the database");
            return null;
        }
        return moveDict[name];
    }


}
