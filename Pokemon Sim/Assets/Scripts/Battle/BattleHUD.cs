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

    [SerializeField] Sprite psnSprite;
    [SerializeField] Sprite brnSprite;
    [SerializeField] Sprite parSprite;
    [SerializeField] Sprite slpSprite;
    [SerializeField] Sprite frzSprite;
    [SerializeField] Image statusCndImage;



    Pokemon _pokemon;
    


    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        pokemonNameText.text = pokemon.Base.Name;
        levelText.text = pokemon.Level.ToString();
        hpBar.SetHP((float)(pokemon.HP / pokemon.MaxHP));
        currHpText.text = pokemon.MaxHP.ToString();
        maxHpText.text = pokemon.MaxHP.ToString();

        SetCndImage();
        _pokemon.OnStatusCndChange += SetCndImage;
    }

    void SetCndImage()
    {
        if (_pokemon.Cnd == null)
        {
            statusCndImage.gameObject.SetActive(false);
        }
        else
        {
            
            switch (_pokemon.Cnd.CndType)
            {
                case ConditionType.psn:
                    statusCndImage.sprite = psnSprite;
                    break;
                case ConditionType.brn:
                    statusCndImage.sprite = brnSprite;
                    break;
                case ConditionType.slp:
                    statusCndImage.sprite = slpSprite;
                    break;
                case ConditionType.frz:
                    statusCndImage.sprite = frzSprite;
                    break;
                case ConditionType.par:
                    statusCndImage.sprite = parSprite;
                    break;
            }
            statusCndImage.gameObject.SetActive(true);
        }
    }

    public IEnumerator UpdateHP()
    {
        //Debug.Log($"Pokemon has taken damage.\n It is now on {_pokemon.HP.ToString()}HP and has {((float)_pokemon.HP) / _pokemon.MaxHP} times its original HP");
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SmoothHPBarDescend(((float)_pokemon.HP) / _pokemon.MaxHP);
            currHpText.text = _pokemon.HP.ToString();
            _pokemon.HpChanged = false;
            yield return hpBar.SetHPBarColour(((float)_pokemon.HP) / _pokemon.MaxHP);
        }
        
    }
}
