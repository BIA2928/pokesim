using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pokedexNumText;
    [SerializeField] TextMeshProUGUI pokeNameText;
    [SerializeField] UITypeBar pokeType;
    [SerializeField] TextMeshProUGUI trainerNameText;
    [SerializeField] TextMeshProUGUI idText;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI memoText;

    public void SetInfo(Pokemon pokemon)
    {
        pokedexNumText.text = pokemon.Base.NationalDexNum.ToString();
        pokeNameText.text = pokemon.Base.Name.ToUpper();
        pokeType.SetImages(pokemon.Base.Type1, pokemon.Base.Type2);
        trainerNameText.text = PlayerController.i.Name;
        idText.text = "69420";
        itemText.text = "-";
        memoText.text = "This is a test placeholder as catch location and nature fields of pokemon have not be set up.";
    }



}
