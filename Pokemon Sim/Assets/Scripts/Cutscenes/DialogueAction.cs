using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueAction : CutsceneAction
{
    [SerializeField] Dialogue dialogue;

    public override IEnumerator Play()
    {
        yield return DialogueManager.Instance.ShowDialogue(dialogue);
    }
}
