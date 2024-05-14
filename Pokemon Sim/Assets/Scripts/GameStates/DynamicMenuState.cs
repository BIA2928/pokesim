using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class DynamicMenuState : State<GameController>
{
    [SerializeField] DynamicMenuUI dynamicMenuUI;
    [SerializeField] TextMeshProSlot textSlotPrefab;
    // Input
    public List<string> MenuItems { get; set; }

    //Output
    public int? SelectedItem { get; private set; }

    GameController gC;
    public override void EnterState(GameController owner)
    {
        gC = owner;

        foreach (Transform child in dynamicMenuUI.transform)
            Destroy(child.gameObject);

        List<TextMeshProSlot> textSlots = new List<TextMeshProSlot>();
        foreach (var item in MenuItems)
        {
            var newItem = Instantiate(textSlotPrefab, dynamicMenuUI.transform);
            newItem.SetText(item);
            textSlots.Add(newItem);
        }

        dynamicMenuUI.SetItems(textSlots);

        dynamicMenuUI.gameObject.SetActive(true);
        dynamicMenuUI.OnSelected += OnMenuItemSelected;
        dynamicMenuUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        dynamicMenuUI.HandleUpdate();
    }

    public override void ExitState()
    {
        dynamicMenuUI.ClearItems();
        dynamicMenuUI.gameObject.SetActive(false);
        dynamicMenuUI.OnSelected -= OnMenuItemSelected;
        dynamicMenuUI.OnBack -= OnBack;
    }

    void OnMenuItemSelected(int selection)
    {
        SelectedItem = selection;
        gC.StateMachine.Pop();
    }

    void OnBack()
    {
        SelectedItem = null;
        gC.StateMachine.Pop();
    }

    public static DynamicMenuState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
}
