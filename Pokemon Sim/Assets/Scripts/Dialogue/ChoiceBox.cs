using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] ChoiceText choiceTextPrefab;
    bool choiceSelected = false;
    List<ChoiceText> choiceTexts;
    int currChoice;
    public IEnumerator ShowChoices(List<string> choices, Action<int> onChoiceSelected) 
    {
        choiceSelected = false;
        gameObject.SetActive(true);

        //choiceTexts.Clear();
        currChoice = 0;
        
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        choiceTexts = new List<ChoiceText>();
        foreach (string choice in choices)
        {
            var obj = Instantiate(choiceTextPrefab, transform);
            obj.TextField.text = choice;
            choiceTexts.Add(obj);
        }
        choiceTexts[currChoice].SetSelected(true);
        yield return new WaitUntil(() => choiceSelected == true);

        onChoiceSelected?.Invoke(currChoice);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        int prevChoice = currChoice;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currChoice;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currChoice;

        if (currChoice > choiceTexts.Count - 1)
            currChoice = 0;
        else if (currChoice < 0)
            currChoice = choiceTexts.Count - 1;

        if (prevChoice != currChoice)
        {
            for (int i = 0; i < choiceTexts.Count; i++)
            {
                choiceTexts[i].SetSelected(i == currChoice);
            }
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }
        

        if (Input.GetKeyDown(KeyCode.Z))
        {
            choiceSelected = true;
            AudioManager.i.PlaySFX(AudioID.UISelect);  
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            choiceSelected = true;
            currChoice = -1;
            //AudioManager.i.PlaySFX(AudioID.UISelect);
        }
    }
}
