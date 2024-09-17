using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using GenericSelectionUI;

public class SummaryScreenUI : MonoBehaviour
{
    [SerializeField] PokemonSection pokemon;
    [SerializeField] PageSelectionBar pageSelectionBar;
    [SerializeField] StatScreen statScreen;
    [SerializeField] InfoScreen infoScreen;
    [SerializeField] MoveScreen moveScreen;

    Pokemon currPokemon;
    int currScreen;

    public bool InMoveSelection { get; set; } = false;
    public MoveScreen MoveScreen => moveScreen;
    public void SetData(Pokemon pokemon)
    {
        currPokemon = pokemon;
        SetPokemon();
    }


    public void ShowPage(int pageNum)
    {
        if (pageNum == 0)
        {
            // Info
            moveScreen.gameObject.SetActive(false);
            statScreen.gameObject.SetActive(false);
            SetInfoScreen();
            infoScreen.gameObject.SetActive(true);

        }
        else if (pageNum == 1)
        {
            // Stats
            moveScreen.gameObject.SetActive(false);
            infoScreen.gameObject.SetActive(false);
            SetStatScreen();
            statScreen.gameObject.SetActive(true);
        }
        else if (pageNum == 2)
        {
            // Moves
            infoScreen.gameObject.SetActive(false);
            statScreen.gameObject.SetActive(false);
            SetMoveScreen();
            moveScreen.gameObject.SetActive(true);
        }
        pageSelectionBar.SetPage(pageNum);
    }
    public void SetStatScreen()
    {
        statScreen.SetStats(currPokemon);
    }

    public void SetInfoScreen()
    {
        infoScreen.SetInfo(currPokemon);
    }

    public void SetMoveScreen()
    {
        moveScreen.SetData(currPokemon.Moves);
    }

    public void SetPokemon()
    {
        pokemon.SetData(currPokemon);
    }

    public void SetMoves()
    {
        moveScreen.SetData(currPokemon.Moves);
    }
}
