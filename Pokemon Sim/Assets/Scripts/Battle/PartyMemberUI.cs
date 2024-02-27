using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text pokemonNameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Text currHpText;
    [SerializeField] Text maxHpText;
    [SerializeField] Image spriteImage;

    [SerializeField] Color highlightColor;

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        pokemonNameText.text = pokemon.Base.Name;
        levelText.text = pokemon.Level.ToString();
        hpBar.SetHP((float)(pokemon.HP / pokemon.MaxHP));
        currHpText.text = pokemon.MaxHP.ToString() + " /";
        maxHpText.text = pokemon.MaxHP.ToString();

        spriteImage.sprite = pokemon.Base.BoxSprite;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            pokemonNameText.color = highlightColor;
        else
            pokemonNameText.color = Color.white;
    }
}
