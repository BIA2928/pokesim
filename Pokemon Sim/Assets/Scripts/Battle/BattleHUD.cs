using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
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
        currHpText.text = pokemon.MaxHP.ToString();
        maxHpText.text = pokemon.MaxHP.ToString();
    }

    public IEnumerator UpdateHP()
    {
        //Debug.Log($"Pokemon has taken damage.\n It is now on {_pokemon.HP.ToString()}HP and has {((float)_pokemon.HP) / _pokemon.MaxHP} times its original HP");
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SmoothHPBarDescend(((float)_pokemon.HP) / _pokemon.MaxHP);
            currHpText.text = _pokemon.HP.ToString();
            _pokemon.HpChanged = false;
        }
        
    }
}
