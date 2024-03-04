using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    FreeRoam,
    Cutscene,
    InBattle,
    InDialogue
}
public class GameController : MonoBehaviour
{
    GameState state;

    [SerializeField] PlayerController pC;
    [SerializeField] BattleSystem bS;
    [SerializeField] Camera worldCam;

    private void Awake()
    {
        ConditionsDB.Init();
    }
    private void Start()
    {
        pC.OnWildEncounter += StartBattle;
        bS.OnBattleOver += EndBattle;
        pC.OnTrainerEncounter += (Collider2D trainerColl) =>
        {
            var trainer = trainerColl.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                state = GameState.Cutscene;
                StartCoroutine(trainer.TriggerBattle(pC));
            }
                
        };

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            state = GameState.InDialogue;
        };

        DialogueManager.Instance.OnCloseDialogue += () =>
        {
            if (state == GameState.InDialogue)
                state = GameState.FreeRoam;
        };
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
        else if (state == GameState.InDialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
    }
}
