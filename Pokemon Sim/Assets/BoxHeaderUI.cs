using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoxHeaderUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] Image leftArrow;
    [SerializeField] Image rightArrow;

    const int BOX_INDEX_LIMIT = 9;
    public void SetBoxNumber(int index)
    {
        headerText.text = "BOX " + (index + 1);
        if (index == 9)
        {
            rightArrow.enabled = false;
            leftArrow.enabled = true;
        }    
        else if (index == 0)
        {
            leftArrow.enabled = false;
            rightArrow.enabled = true;
        }
        else
        {
            leftArrow.enabled = true;
            rightArrow.enabled = true;
        }
            
    }
}
