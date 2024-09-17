
using System;
using GenericSelectionUI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PartyScreen : SelectionUI<TextSlot>
{
    [SerializeField] Text messageText;
    PartyMemberUI[] memberSlots;
    List<Pokemon> pokemonList;
    PokemonParty playerParty;
    



    public List<Pokemon> PokemonList
    {
        get { return pokemonList;  }
    }

    public int CurrPartyPoke
    {
        get { return selection; }
    }

    public Pokemon SelectedMember => pokemonList[selection];
    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        SetSelectionSettings(SelectionType.Grid, 2);
        playerParty = PokemonParty.GetPlayerParty();
        SetPartyData();

        playerParty.OnPartyUpdate += SetPartyData;
    }

    public void SetPartyData()
    {
        pokemonList = playerParty.PokemonList;
        ClearItems();
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

        var textSlots = memberSlots.Select(m => m.GetComponent<TextSlot>());
        SetItems(textSlots.Take(pokemonList.Count).ToList());
        messageText.text = "Choose a pokemon.";
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
