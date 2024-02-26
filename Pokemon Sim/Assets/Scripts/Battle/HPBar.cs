using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;
    [SerializeField] Sprite medHpImage;
    [SerializeField] Sprite lowHpImage;

    public static Color lowHpBar = new Color(255, 70, 52);
    public static Color medHpBar = new Color(255, 182, 16);
    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);
        if (hpNormalized < 0.5f && hpNormalized > 0.2)
        {
            this.GetComponent<Image>().sprite = medHpImage;
        } else if (hpNormalized < 0.2)
        {
            this.GetComponent<Image>().sprite = lowHpImage;
        }
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
 
}
