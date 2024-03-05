using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] Sprite highHpImage;
    [SerializeField] Sprite medHpImage;
    [SerializeField] Sprite lowHpImage;
    private Image hpBarImage;

    public static Color lowHpBar = new Color(255, 70, 52);
    public static Color medHpBar = new Color(255, 182, 16);

    public void Awake()
    {
        hpBarImage = this.GetComponent<Image>();
    }
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
        SetHPBarColour(hpNormalized);
    }
    

    public IEnumerator SmoothHPBarDescend(float newHp)
    {
        float currHp = health.transform.localScale.x;
        float delta = currHp - newHp;
        while (currHp - newHp > Mathf.Epsilon)
        {
            currHp -= delta * Time.deltaTime;
            health.transform.localScale = new Vector3(currHp, 1f);
            yield return null;
        }


        health.transform.localScale = new Vector3(newHp, 1f);
       
    }

    public void SetHPBarColour(float newHp) 
    {
        Debug.Log($"New hp is MaxHp * {newHp}");
        if (newHp > 0.5f)
        {
            hpBarImage.sprite = highHpImage;
        }
        else if (newHp <= 0.5f && newHp > 0.22f) 
        {
            hpBarImage.sprite = medHpImage;
        }
        else
        {
            hpBarImage.sprite = lowHpImage;
        }

        
    }
 
}
