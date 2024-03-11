using DG.Tweening;
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
    [SerializeField] Image expBar;

    Pokemon _pokemon;
    

    public void SetData(Pokemon pokemon)
    {
        _pokemon = pokemon;

        pokemonNameText.text = pokemon.Base.Name;
        SetLevel();
        float currHp = (float)pokemon.HP; 
        float maxHp = (float)pokemon.MaxHP;
        hpBar.SetHP(currHp / maxHp);
        currHpText.text = pokemon.HP.ToString();
        maxHpText.text = pokemon.MaxHP.ToString();

        SetExp();
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
            hpBar.SetHPBarColour(((float)_pokemon.HP) / _pokemon.MaxHP);
        }
        
    }

    float GetXpNomalised()
    {
        int currLevelXp = _pokemon.Base.GetExpForLevel(_pokemon.Level);
        int nextLevelXp = _pokemon.Base.GetExpForLevel(_pokemon.Level + 1);

        float xpNorm = (float)(_pokemon.Exp - currLevelXp) / (nextLevelXp - currLevelXp);

        return Mathf.Clamp(xpNorm, 0, 1);
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float xpNorm = GetXpNomalised();

        expBar.transform.localScale = new Vector3(xpNorm, 1, 1);
    }

    public IEnumerator SetExpAnimation(bool overflow=false)
    {
        if (expBar == null) yield break;

        if (overflow)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }
        float normXp = GetXpNomalised();

        yield return expBar.transform.DOScaleX(normXp, 1f).WaitForCompletion();
    }

    public void SetLevel()
    {
        levelText.text = _pokemon.Level.ToString();
    }

    public void UpdateHpOnLevelUp()
    {
        maxHpText.text = _pokemon.MaxHP.ToString();
        currHpText.text = _pokemon.HP.ToString();
    }
}