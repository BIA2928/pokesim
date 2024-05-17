using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PokemonSection : MonoBehaviour
{
    [SerializeField] Image pokeImage;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI lvlText;

    public void SetData(Pokemon pokemon)
    {
        nameText.text = pokemon.Base.Name;
        lvlText.text = $"Lv{pokemon.Level}";
        pokeImage.sprite = pokemon.Base.FrontSprite;
    }
}
