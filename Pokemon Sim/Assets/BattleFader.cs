using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;

public class BattleFader : MonoBehaviour
{
    [SerializeField] Image leftFader;
    [SerializeField] Image rightFader;

    public IEnumerator FadeToBlack()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(leftFader.rectTransform.DOScaleX(1, 0.8f));
        sequence.Join(rightFader.rectTransform.DOScaleX(-1, 0.8f));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator FadeToColour()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(leftFader.rectTransform.DOScaleX(0, 0.8f));
        sequence.Join(rightFader.rectTransform.DOScaleX(0, 0.8f));
        yield return sequence.WaitForCompletion();
    }

}
