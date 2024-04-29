using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menu;
    List<MenuSlot> menuSlots;

    public event Action<int> onMenuSlotSelected;
    public event Action onBack;

    int selectedItem = 0;


    private void Awake()
    {
        menuSlots = menu.GetComponentsInChildren<MenuSlot>().ToList();
    }
    public void OpenMenu()
    {
        AudioManager.i.PlaySFX(AudioID.MenuOpen);
        menu.SetActive(true);
        UpdateMenuSelection();
    }

    public void CloseMenu(bool goneBack = false)
    {
        if (goneBack)
            AudioManager.i.PlaySFX(AudioID.MenuClose);
        selectedItem = 0;
        menu.SetActive(false);
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++selectedItem;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --selectedItem;
        }

        selectedItem = Mathf.Clamp(selectedItem, 0, menuSlots.Count - 1);

        if (selectedItem != prevSelection)
        {
            UpdateMenuSelection();
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.i.PlaySFX(AudioID.UISelect);
            onMenuSlotSelected?.Invoke(selectedItem);
            CloseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            CloseMenu(true);
        }
        
    }

    void UpdateMenuSelection()
    {
        for (int i = 0; i < menuSlots.Count; i++)
        {
            if (i == selectedItem)
            {
                menuSlots[i].Select();
            }
            else
            {
                menuSlots[i].Deselect();
            }
        }
        
    }
}
