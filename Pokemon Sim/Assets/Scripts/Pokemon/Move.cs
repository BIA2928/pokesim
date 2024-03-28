using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move 
{
    public MoveBase Base { get; set; }
    public int PP { get; set; }

    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = pBase.Pp;
    }

    public void RestorePP(int restoreAmount)
    {
        int newPP = PP + restoreAmount;
        PP = Mathf.FloorToInt(Mathf.Clamp(newPP, 0, Base.Pp));
    }

    public Move(MoveSaveData sD)
    {
        PP = sD.currPP;
        Base = MoveDB.LookUpByName(sD.baseName);
    }

    public MoveSaveData GetSaveData()
    {
        var sD = new MoveSaveData()
        {
            baseName = Base.Name,
            currPP = PP
        };
        return sD;
    }

}

[System.Serializable]
public class MoveSaveData
{
    public string baseName;
    public int currPP;
}
