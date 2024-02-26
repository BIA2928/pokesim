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

    Pokemon _pokemon;

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        pokemonNameText.text = pokemon.Base.Name;
        levelText.text = pokemon.Level.ToString();
        hpBar.SetHP((float)(pokemon.HP / pokemon.MaxHP));
        currHpText.text = pokemon.MaxHP.ToString() + " /";
        maxHpText.text = pokemon.MaxHP.ToString();

         
    }
}
