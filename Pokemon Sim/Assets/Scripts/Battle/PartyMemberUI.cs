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
    [SerializeField] Text messageText;


    Pokemon _pokemon;

    public void Init(Pokemon pokemon)
    {
        _pokemon = pokemon;
        UpdateData();
        _pokemon.OnHPChanged += UpdateData;
        SetMesage("");
    }

    void UpdateData()
    {
        float newHp = ((float)_pokemon.HP) / _pokemon.MaxHP; 
        pokemonNameText.text = _pokemon.Base.Name;
        levelText.text = "Lv." + _pokemon.Level.ToString();
        hpBar.SetHP(newHp);
        currHpText.text = _pokemon.HP.ToString() + " /";
        maxHpText.text = _pokemon.MaxHP.ToString();

        spriteImage.sprite = _pokemon.Base.BoxSprite;
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            pokemonNameText.color = GlobalSettings.i.HighlightedColorRed;
        else
            pokemonNameText.color = Color.white;
    }

    public void SetMesage(string input)
    {
        messageText.text = input;
    }
}
