using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenericSelectionUI;
using System.Linq;

public class ActionSelectionUI : SelectionUI<TextSlot>
{
    private void Start()
    {
        SetSelectionSettings(SelectionType.Grid, 2);
        SetItems(GetComponentsInChildren<TextSlot>().ToList());

    }
}
