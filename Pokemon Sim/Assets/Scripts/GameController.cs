using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    FreeRoam,
    InBattle
}
public class GameController : MonoBehaviour
{
    GameState state;

    [SerializeField] PlayerController pC;
    [SerializeField] BattleSystem bS;
    [SerializeField] Camera worldCam;

    private void Start()
    {
        pC.OnWildEncounter += StartBattle;
        bS.OnBattleOver += EndBattle;
    }

    void StartBattle()
    {
        state = GameState.InBattle;
        bS.gameObject.SetActive(true);
        worldCam.gameObject.SetActive(false);

        var playerParty = pC.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();
        bS.StartBattle(playerParty, wildPokemon);

    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        bS.gameObject.SetActive(false);
        worldCam.gameObject.SetActive(true);
        if (won)
        {

        }
        else
        {

        }
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            pC.HandleUpdate();
        } 
        else if (state == GameState.InBattle)
        {
            bS.HandleUpdate();
        }
    }
}
