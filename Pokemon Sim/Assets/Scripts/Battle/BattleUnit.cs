using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHUD battleHUD;

    public BattleHUD BattleHUD
    {
        get { return battleHUD;  }
    }

    public Pokemon Pokemon { get; set; }
    public bool IsPlayerUnit { get { return isPlayerUnit; } }

    Image image;
    Vector3 originalPos;
    Vector3 origScale = new Vector3(1f, 1f, 1f);
    Color origColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = transform.localPosition;
        origColor = image.color;
    }
    public void Setup(Pokemon poke)
    {
        transform.localScale = origScale; 
        Pokemon = poke;
        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        } 
        else
        {
            image.sprite = Pokemon.Base.FrontSprite;
            ScalePokemon(Pokemon.Base.PokeSize);         
        }

        battleHUD.SetData(poke);
        battleHUD.gameObject.SetActive(true);
        PlayEnterAnimation();
    }

    public void Clear()
    {
        battleHUD.gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets the pokeImage after the pokemon has fainted.
    /// </summary>
    public void ResetAfterFaint()
    {
        image.transform.localPosition = originalPos;
        image.color = origColor;
    }

    /// <summary>
    /// Scales the pokemon to ensure larger pokemon sprites actually appear larger than smaller counterparts
    /// </summary>
    /// <param name="size">The PokeSize enum of the qualitative size of the pokemon.</param>
    private void ScalePokemon(PokeSize size)
    {
        Transform rT = image.transform;
        switch (size)
        {
            case PokeSize.S:
                break;
            case PokeSize.M:
                rT.localScale = new Vector3(SizeScales.MediumScale, SizeScales.MediumScale, 1.0f);
                rT.position += new Vector3(0f, SizeScales.MediumYTranslation, 0f);
                break;
            case PokeSize.L:
                rT.localScale = new Vector3(SizeScales.LargeScale, SizeScales.LargeScale, 1.0f);
                rT.position += new Vector3(0f, SizeScales.LargeYTranslation, 0f);
                break;
            case PokeSize.XL:
                rT.localScale = new Vector3(SizeScales.XLScale, SizeScales.XLScale, 1.0f);
                rT.position += new Vector3(0f, SizeScales.XLYTranslation, 0f);
                break;
            default:
                break;
        }
    }

    // Animations here

    public void PlayAttackAnimation()
    {
        float animationDistance = 45f;
        // Animationtime = total time / 2
        float animationTime = 0.15f;
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + animationDistance, animationTime));
        } 
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - animationDistance, animationTime));
        }

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, animationTime));

    }

    

    public void PlayHitEffect()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.05f));
        sequence.Append(image.DOColor(origColor, 0.05f));
    }

    public void PlayFaintAnimation() 
    {
        Vector3 startPos = transform.localPosition;
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 120f, 0.3f));
        sequence.Join(image.DOFade(0f, 0.3f));

        transform.localPosition = startPos;
    }

    public void PlayEnterAnimation()
    {
        // Ensure in the correct position

        image.transform.localPosition = originalPos;
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(500f, originalPos.y);
        }
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.8f));
        sequence.Join(image.DOFade(origColor.a, 0.1f));
    } 

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0f, 0.4f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 40f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.4f, 0.4f ,1f), 0.5f));

        yield return sequence.WaitForCompletion();

    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1f, 0.3f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, 0.4f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.4f));

        yield return sequence.WaitForCompletion();

    }


}






static class SizeScales
{
    public const float MediumScale = 1.05f;
    public const float MediumYTranslation = 0.1f;
    public const float LargeScale = 1.14f;
    public const float LargeYTranslation = 0.3f;
    public const float XLScale = 1.2f;
    public const float XLYTranslation = 0.5f;
}



