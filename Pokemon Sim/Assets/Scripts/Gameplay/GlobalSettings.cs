using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] Color highlightedColorRed;
    [SerializeField] Color highlightedColorBlue;

    public Color HighlightedColorRed => highlightedColorRed;
    public Color HighlightedColorBlue => highlightedColorBlue;

    public static GlobalSettings i { get; private set; }

    private void Awake()
    {
        i = this;
    }
}
