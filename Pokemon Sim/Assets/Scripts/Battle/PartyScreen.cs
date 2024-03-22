using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] Text messageText;
    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemonList;

    int currPartyPoke = 0;

    public BattleState? CalledFromState { get; set; }


    public List<Pokemon> PokemonList
    {
        get { return pokemonList;  }
    }

    public int CurrPartyPoke
    {
        get { return currPartyPoke; }
    }

    public Pokemon SelectedMember => pokemonList[currPartyPoke];
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
        UpdatePokemonSelection(0);
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

    public void HandleUpdate(Action onSelection, Action onBack)
    {
        var prevSelection = currPartyPoke;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currPartyPoke;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currPartyPoke;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currPartyPoke += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currPartyPoke -= 2;

        currPartyPoke = Mathf.Clamp(currPartyPoke, 0, PokemonList.Count - 1);

        if (prevSelection != currPartyPoke)
            UpdatePokemonSelection(currPartyPoke);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelection?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            Debug.Log("Going back");
        }
    }

    public void SetMessageText(string text)
    {
        messageText.text = text;
    }
}
