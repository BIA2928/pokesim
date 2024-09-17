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

    public IEnumerator FadeToBlack(float duration)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(leftFader.rectTransform.DOScaleX(1, duration));
        sequence.Join(rightFader.rectTransform.DOScaleX(-1, duration));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator FadeToColour(float duration)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(leftFader.rectTransform.DOScaleX(0, duration));
        sequence.Join(rightFader.rectTransform.DOScaleX(0, duration));
        yield return sequence.WaitForCompletion();
    }

    public IEnumerator FadeInAndOut(float totalDuration)
    {
        float duration = totalDuration * 0.5f;
        var sequence = DOTween.Sequence();
        sequence.Append(leftFader.rectTransform.DOScaleX(1, duration));
        sequence.Join(rightFader.rectTransform.DOScaleX(-1, duration));
        yield return sequence.WaitForCompletion();
        yield return new WaitForSeconds(0.2f);
        var sequence2 = DOTween.Sequence();
        sequence2.Append(leftFader.rectTransform.DOScaleX(0, duration));
        sequence2.Join(rightFader.rectTransform.DOScaleX(0, duration));
        yield return sequence2.WaitForCompletion();
    }

}
