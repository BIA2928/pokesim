using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PocketSelectorUI : MonoBehaviour
{
    [SerializeField] Image selector;
    [SerializeField] Text pocketText;
    const float pocketDistance = 35.8f;
    RectTransform selectorPosition;
    float originalXPosition;
    float originalYPosition;

    private void Awake()
    {
        selectorPosition = selector.GetComponent<RectTransform>();
        originalXPosition = selectorPosition.anchoredPosition.x;
        originalYPosition = selectorPosition.anchoredPosition.y;
    }
    public void UpdatePocket(int selectedPocket)
    {
        Debug.Log($"Updating pocket with selection = {selectedPocket}");
        pocketText.text = ItemCategories[selectedPocket];
        selectorPosition.anchoredPosition = new Vector2(originalXPosition + pocketDistance * selectedPocket, originalYPosition);
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "ITEMS", "MEDICINE", "POKEBALLS", "TMs/HMs", "BERRIES", "MAIL", "BATTLE ITEMS", "KEY ITEMS"
    };
}
