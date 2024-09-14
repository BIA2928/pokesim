using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoxHeaderUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI headerText;

    public void SetBoxNumber(int number)
    {
        headerText.text = "BOX " + (number + 1);
    }
}
