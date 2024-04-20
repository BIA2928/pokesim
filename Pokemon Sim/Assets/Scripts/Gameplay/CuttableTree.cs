using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class CuttableTree : InteractableObject
{
    public override IEnumerator Interact(Transform initiator)
    {
        yield return DialogueManager.Instance.ShowDialogue("This tree looks like it can be cut down!");

        yield return base.Interact(initiator);
    }

    public override IEnumerator ApplyHM()
    {
        yield return base.ApplyHM();
    }
}
