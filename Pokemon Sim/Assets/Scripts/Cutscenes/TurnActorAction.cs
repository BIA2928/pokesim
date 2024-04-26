using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnActorAction : CutsceneAction
{

    [SerializeField] CutsceneActor actor;
    [SerializeField] TurnAction action;
    
    override public IEnumerator Play()
    {
        var targetChar = action.CharacterToLookAt;
        var character = actor.GetCharacter();
        if (targetChar != null)
        {
            character.LookTowards(targetChar.transform.position);
            yield return new WaitUntil(() => character.IsMoving == false);
            yield break;
        }

        if (action.FacingDirection == FacingDirection.Up)
            character.LookTowards(character.transform.position + Vector3.up);
        else if (action.FacingDirection == FacingDirection.Down)
            character.LookTowards(character.transform.position + Vector3.down);
        else if (action.FacingDirection == FacingDirection.Left)
            character.LookTowards(character.transform.position + Vector3.left);
        else
            character.LookTowards(character.transform.position + Vector3.right);

        yield return new WaitUntil(() => character.IsMoving == false);
    }
    
}

[System.Serializable]
public class TurnAction
{
    [SerializeField] FacingDirection direction;
    [SerializeField] Character characterToLookAt;


    public FacingDirection FacingDirection => direction;
    public Character CharacterToLookAt => characterToLookAt;

}
