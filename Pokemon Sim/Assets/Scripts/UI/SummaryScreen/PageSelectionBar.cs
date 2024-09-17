using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageSelectionBar : MonoBehaviour
{
    [SerializeField] Sprite page0;
    [SerializeField] Sprite page1;
    [SerializeField] Sprite page2;
    [SerializeField] Image parentImage;
    [SerializeField] TextMeshProUGUI pageText;
    readonly string[] headerNames = { "POKeMON INFO", "POKeMON SKILLS", "BATTLE MOVES" };
    
    public void SetPage(int pageNumber)
    {
        if (pageNumber == 1)
        {
            parentImage.sprite = page1;
        }
        else if (pageNumber == 2)
        {
            parentImage.sprite = page2;
        }
        else
        {
            parentImage.sprite = page0;

        }
        pageText.text = headerNames[pageNumber];
    }
}
