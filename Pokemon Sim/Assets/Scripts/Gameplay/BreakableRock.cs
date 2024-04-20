using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableRock : InteractableObject
{
    public override IEnumerator Interact(Transform initiator)
    {
        yield return DialogueManager.Instance.ShowDialogue("This rock looks like it could be broken!");

        yield return base.Interact(initiator);
    }

    public override IEnumerator ApplyHM()
    {
        yield return base.ApplyHM();
    }
}
