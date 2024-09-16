using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoxInfoOverlay : MonoBehaviour
{
    [SerializeField] Image pokeImage;
    [SerializeField] TextMeshProUGUI upperNameText;
    [SerializeField] TextMeshProUGUI lowerNameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI pokedexNumText;
    [SerializeField] UITypeBar typeBar;

    public void Clear()
    {
        pokeImage.enabled = false;
        upperNameText.text = "";
        lowerNameText.text = "";
        levelText.text = "";
        pokedexNumText.text = "";
        typeBar.Clear();
    }

    public void ShowDetails(Pokemon pokemon)
    {
        if (pokemon == null)
            Clear();
        else
        {
            pokeImage.sprite = pokemon.Base.FrontSprite;
            upperNameText.text = pokemon.Base.Name.ToUpper();
            lowerNameText.text = pokemon.Base.Name.ToUpper();
            levelText.text = "Lv. " + pokemon.Level;
            pokedexNumText.text = "No. " + pokemon.Base.RegionalDexNum;
            pokeImage.enabled = true;
            typeBar.SetImages(pokemon.Base.Type1, pokemon.Base.Type2);
        }
        
    }

}
