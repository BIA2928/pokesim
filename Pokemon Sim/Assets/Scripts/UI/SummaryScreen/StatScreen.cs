using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatScreen : MonoBehaviour
{
    [Header("Adjustable Bars")]
    [SerializeField] HPBar hpbar;
    [SerializeField] ExpBar expBar;
    [Header("Text Details")]
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI attackText;
    [SerializeField] TextMeshProUGUI defText;
    [SerializeField] TextMeshProUGUI spAttackText;
    [SerializeField] TextMeshProUGUI spDefText;
    [SerializeField] TextMeshProUGUI speedText;
    [SerializeField] TextMeshProUGUI expText;
    [SerializeField] TextMeshProUGUI nextLvlExpText;
    [SerializeField] TextMeshProUGUI abilityName;
    [SerializeField] TextMeshProUGUI abilityText;

    public void SetStats(Pokemon pokemon)
    {
        hpText.text = $"{pokemon.HP} / {pokemon.MaxHP}";
        attackText.text = $"{pokemon.Attack}";
        defText.text = $"{pokemon.Defence}";
        spAttackText.text = $"{pokemon.SpAttack}";
        spDefText.text = $"{pokemon.SpDefence}";
        speedText.text = $"{pokemon.Speed}";
        expText.text = pokemon.Exp.ToString();
        nextLvlExpText.text = $"{pokemon.Base.GetExpForLevel(pokemon.Level + 1) - pokemon.Exp}";

        float hpNorm = ((float)pokemon.HP) / ((float)pokemon.MaxHP);
        hpbar.SetHP(hpNorm);
        expBar.SetEXP(pokemon.Base.GetExpForLevel(pokemon.Level + 1), pokemon.Base.GetExpForLevel(pokemon.Level), pokemon.Exp);
    }
}
