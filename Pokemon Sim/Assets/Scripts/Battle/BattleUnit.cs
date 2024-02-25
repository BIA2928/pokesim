using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }

    Image image;
    Vector3 originalPos;
    Color origColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        origColor = image.color;
    }
    public void Setup()
    {
        Pokemon = new Pokemon(_base, level);
        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        } 
        else
        {
            image.sprite = Pokemon.Base.FrontSprite;
            ScalePokemon(Pokemon.Base.PokeSize);         
        }
        image.color = origColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(500f, originalPos.y);
        }
        image.transform.DOLocalMoveX(originalPos.x, 0.8f);
    }

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
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 120f, 0.3f));
        sequence.Join(image.DOFade(0f, 0.3f));
    }

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
        originalPos = rT.localPosition;
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



