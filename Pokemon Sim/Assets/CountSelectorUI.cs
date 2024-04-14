using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class CountSelectorUI : MonoBehaviour
{
    [SerializeField] Text countText;
    [SerializeField] Text priceText;
    [SerializeField] Image downArrow;
    [SerializeField] Image upArrow;
    readonly CultureInfo cultureInfo = new CultureInfo("en-US");

    bool selected = false;
    bool goneBack = false;
    int currCount;
    int maxCount;
    int pricePerUnit;
    public IEnumerator ShowSelector(int maxCount, int pricePerUnit, 
        Action<int> onCountSelected)
    {
        selected = false;
        goneBack = false;
        currCount = 1;

        this.maxCount = maxCount;
        this.pricePerUnit = pricePerUnit;

        SetValues();
        gameObject.SetActive(true);

        yield return new WaitUntil(() => selected == true || goneBack == true);

        if (selected)
            onCountSelected?.Invoke(currCount);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        int prevCount = currCount;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            ++currCount;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            --currCount;
        }

        currCount = InventoryUI.MyClamp(currCount, 1, maxCount);
        if (prevCount != currCount)
        {
            SetValues();
        }
        if (Input.GetKeyDown(KeyCode.Z))
            selected = true;
        else if (Input.GetKeyDown(KeyCode.X))
        {
            currCount = -1;
            selected = true;
        }

    }

    public void SetValues()
    {
        priceText.text = (pricePerUnit * currCount).ToString("c0", cultureInfo);
        if (currCount < 10)
            countText.text = "x" + "0" + currCount;
        else
            countText.text = "x" + currCount;
        if (currCount == 1)
        {
            downArrow.enabled = false;
            upArrow.enabled = true;
        }
        else if (currCount == maxCount)
        {
            upArrow.enabled = false;
            downArrow.enabled = true;
        }    
        else
        {
            downArrow.enabled = true;
            upArrow.enabled = true;
        }
    }
}
