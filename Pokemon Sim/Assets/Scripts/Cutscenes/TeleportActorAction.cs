using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportActorAction : CutsceneAction
{
    [SerializeField] GameObject gO;
    [SerializeField] Vector2 posToGo;

    public override IEnumerator Play()
    {
        gO.transform.position = posToGo;
        yield break;
    }
}
