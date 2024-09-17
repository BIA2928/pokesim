using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBar : MonoBehaviour
{
    [SerializeField] GameObject changingBar;
    // Start is called before the first frame update
    public void SetEXP(float expNormalized)
    {
        changingBar.transform.localScale = new Vector3(expNormalized, 1f);
    }

    public void SetEXP(int xpNextLvl, int xpThisLvl, int currXp)
    {
        float diff = xpNextLvl - xpThisLvl;
        float progress = currXp - xpThisLvl;
        Debug.Log($"diff is {diff}, progress is {progress}");
        if (diff == 0)
            SetEXP(0f);
        else 
            SetEXP(Mathf.Clamp01(progress / diff));
    }
}
