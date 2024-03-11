using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemonList;
    
    public List<Pokemon> PokemonList
    {
        get { return pokemonList;  }
    }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    public void SetPartyData(List<Pokemon> pokemon)
    {
        this.pokemonList = pokemon;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemon.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemon[i]);
            }
                
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        messageText.text = "Choose a pokemon.";
    }

    public void UpdatePokemonSelection(int selectedPokeIndex)
    {
        for (int i = 0; i < pokemonList.Count; i++)
        {
            if (i == selectedPokeIndex)
            {
                memberSlots[i].SetSelected(true);
            }
            else
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public void SetMessageText(string text)
    {
        messageText.text = text;
    }
}
