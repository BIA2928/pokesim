using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveActorAction : CutsceneAction
{
    [SerializeField] CutsceneActor actor;
    [SerializeField] List<Vector2> movePattern;

    public override IEnumerator Play()
    {
        var character = actor.GetCharacter();
        foreach (var move in movePattern)
        {
            yield return character.Move(move, checkCollisions:false);
        }
    }
}


[System.Serializable]
public class CutsceneActor
{
    [SerializeField] bool isPlayer;
    [SerializeField] Character character;

    public Character GetCharacter()
    {
        if (!isPlayer)
            return character;
        else
            return  PlayerController.i.Character;
    }

}