using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenericSelectionUI;
using System.Linq;

public class MenuController1 : SelectionUI<MenuItemSlot>
{
    private void Start()
    {
        SetItems(GetComponentsInChildren<MenuItemSlot>().ToList());
    }
}
