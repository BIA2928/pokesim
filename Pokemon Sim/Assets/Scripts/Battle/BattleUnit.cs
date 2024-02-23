using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }
    public void Setup()
    {
        Pokemon = new Pokemon(_base, level);
        if (isPlayerUnit)
        {
            GetComponent<Image>().sprite = Pokemon.Base.BackSprite;
        } 
        else
        {
            GetComponent<Image>().sprite = Pokemon.Base.FrontSprite;
            ScalePokemon(Pokemon.Base.PokeSize);         
        }

    }

    private void ScalePokemon(PokeSize size)
    {
        switch(size)
        {
            case PokeSize.S:
                break;
            case PokeSize.M:
                this.GetComponent<RectTransform>().localScale = new Vector3(SizeScales.MediumScale, SizeScales.MediumScale, 1.0f);
                this.GetComponent<RectTransform>().position += new Vector3(0f, SizeScales.MediumYTranslation, 0f);
                break;
            case PokeSize.L:
                this.GetComponent<RectTransform>().localScale = new Vector3(SizeScales.LargeScale, SizeScales.LargeScale, 1.0f);
                this.GetComponent<RectTransform>().position += new Vector3(0f, SizeScales.LargeYTranslation, 0f);
                break;
            case PokeSize.XL:
                this.GetComponent<RectTransform>().localScale = new Vector3(SizeScales.XLScale, SizeScales.XLScale, 1.0f);
                this.GetComponent<RectTransform>().position += new Vector3(0f, SizeScales.XLYTranslation, 0f);
                break;
            default:
                break;
        }
    }
}

static class SizeScales
{
    public const float MediumScale = 1.05f;
    public const float MediumYTranslation = 0.2f;
    public const float LargeScale = 1.14f;
    public const float LargeYTranslation = 0.4f;
    public const float XLScale = 1.2f;
    public const float XLYTranslation = 0.45f;
}



