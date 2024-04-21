using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfableWater : InteractableObject
{

    Transform initiator;
    override public IEnumerator Interact(Transform initiator)
    {
        yield return DialogueManager.Instance.ShowDialogue($"The water here is a deep blue.");
        this.initiator = initiator;
        yield return base.Interact(initiator);
    }


    public override IEnumerator ApplyHM()
    {
        var anim = initiator.GetComponent<CharacterAnimator>();
        var dir = new Vector3(anim.MoveX, anim.MoveY);
        var targetPos = initiator.position + dir;
        AudioManager.i.PlaySFX(AudioID.Jump);
        yield return initiator.DOJump(targetPos, 0.15f, 1, 0.5f).WaitForCompletion();
        AudioManager.i.PlaySurfMusic();
        anim.SetSurfing(true);
        
    }
}
