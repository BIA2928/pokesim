using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveDescriptor : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI powerText;
    [SerializeField] TextMeshProUGUI accuracyText;
    [SerializeField] TextMeshProUGUI descriptionText;

    public void SetData(Move move)
    {
        if (move.Base.MoveType == MoveType.Status)
            powerText.text = "-";
        else
            powerText.text = move.Base.Power.ToString();

        accuracyText.text = move.Base.Accuracy.ToString();
        descriptionText.text = move.Base.Description;

    }

    public void Clear()
    {
        powerText.text = "";
        accuracyText.text = "";
        descriptionText.text = "";
    }
}
