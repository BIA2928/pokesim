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
    PokemonParty playerParty;
    
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
        playerParty = PokemonParty.GetPlayerParty();
        SetPartyData();

        playerParty.OnPartyUpdate += SetPartyData;
    }

    public void SetPartyData()
    {
        pokemonList = playerParty.PokemonList;
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemonList.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(pokemonList[i]);
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
        {
            UpdatePokemonSelection(currPartyPoke);
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }
            

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelection?.Invoke();
            AudioManager.i.PlaySFX(AudioID.UISelect);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }
    }

    public void SetMessageText(string text)
    {
        messageText.text = text;
    }

    public void ClearMemberSlotMessages()
    {
        foreach (var item in memberSlots)
        {
            item.SetMesage("");
        }
    }

    public void ShowIfTMUsable(TmItem tm)
    {
        for (int i = 0; i < pokemonList.Count; i++)
        {
            string message = pokemonList[i].Base.CanLearnByTm(tm.Move) ? "ABLE!" : "UNABLE!";
            if (pokemonList[i].HasMove(tm.Move))
                message = "LEARNED.";
            
            memberSlots[i].SetMesage(message);
        }
    }
}
