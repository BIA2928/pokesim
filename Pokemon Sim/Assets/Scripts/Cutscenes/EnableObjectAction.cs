using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectAction : CutsceneAction
{
    [SerializeField] GameObject gO;
    [SerializeField] ObjectActions objectAction;

    public override IEnumerator Play()
    {
        bool setActive = objectAction == ObjectActions.Enable;
        gO.SetActive(setActive);
        yield break;
    }
}
