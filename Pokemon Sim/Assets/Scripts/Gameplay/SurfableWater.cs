using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfableWater : InteractableObject, IPlayerTriggerable
{
    [SerializeField] [Range(0, 20)] int encounterRate = 1;
    bool isJumpingToWater = false;
    Transform initiator;

    public bool TriggerRepeatedly => true;

    override public IEnumerator Interact(Transform initiator)
    {
        if (initiator.GetComponent<CharacterAnimator>().IsSurfing || isJumpingToWater)
            yield break;
        yield return DialogueManager.Instance.ShowDialogue($"The water here is a deep blue.");
        this.initiator = initiator;
        yield return base.Interact(initiator);
    }


    public override IEnumerator ApplyHM()
    {
        var anim = initiator.GetComponent<CharacterAnimator>();
        var dir = new Vector3(anim.MoveX, anim.MoveY);
        var targetPos = initiator.position + dir;

        isJumpingToWater = true;
        AudioManager.i.PlaySFX(AudioID.Jump);
        yield return initiator.DOJump(targetPos, 0.15f, 1, 0.5f).WaitForCompletion();
        isJumpingToWater = false;

        AudioManager.i.PlaySurfMusic();
        anim.SetSurfing(true);
    }

    public void OnPlayerTrigger(PlayerController player)
    {
        
        if (Random.Range(1, 20) >= (20 - encounterRate))
        {
            player.Character.Animator.IsMoving = false;
            GameController.i.StartBattle(BattleEnvironment.Water);
        }
        
    }
}
