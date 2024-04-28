using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreenAction : CutsceneAction
{
    [SerializeField] FadeType fadeType;
    [SerializeField][Range(0.1f, 1f)] float fadeDuration = 0.5f;

    public override IEnumerator Play()
    {
        if (fadeType == FadeType.FadeToBlack)
            yield return Fader.instance.FadeIn(fadeDuration);
        else
            yield return Fader.instance.FadeOut(fadeDuration);
    }
}

public enum FadeType { FadeToBlack, FadeToColour }

