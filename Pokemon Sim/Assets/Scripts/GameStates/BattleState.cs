using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.StateMachine;

public class BattleState : State<GameController>
{
    [SerializeField] BattleSystem battleSystem;
    public BattleSystem BattleSystem => battleSystem;

    //Required input before state
    public BattleEnvironment CurrEnvironment { get; set; }
    public TrainerController Trainer { get; set; }



    public static BattleState i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    GameController gC;
    public override void EnterState(GameController owner)
    {
        gC = owner;

        battleSystem.gameObject.SetActive(true);
        gC.WorldCam.gameObject.SetActive(false);
        var playerParty = PlayerController.i.GetComponent<PokemonParty>();
        var music = battleSystem.WildBattleMusic; 
        if (Trainer == null)
        {
            var wildPokemon = gC.CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon(CurrEnvironment);
            var copy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
            AudioManager.i.PlayBattleMusic(music, true, false);
            battleSystem.StartBattle(playerParty, copy, CurrEnvironment);
        }
        else
        {
            music = Trainer.BattleMusic;
            AudioManager.i.PlayBattleMusic(music, true, false);
            battleSystem.StartTrainerBattle(playerParty, Trainer.GetComponent<PokemonParty>());
        }

        battleSystem.OnBattleOver += OnBattleOver;
    }

    public override void Execute()
    {
        battleSystem.HandleUpdate();
    }

    public override void ExitState()
    {
        AudioManager.i.StopBattleMusic();
        battleSystem.gameObject.SetActive(false);
        gC.WorldCam.gameObject.SetActive(true);
        battleSystem.OnBattleOver -= OnBattleOver;
    }

    void OnBattleOver(bool playerWon)
    {
        if (playerWon)
        {
            if (Trainer != null)
            {
                Trainer.Beat();
                Trainer = null;
            }

            /*var playerParty = pC.GetComponent<PokemonParty>();
            bool willEvolve = playerParty.CheckForEvolution();
            if (willEvolve)
            {
                state = GameState.FreeRoam;
                StartCoroutine(playerParty.RunEvolutions());
            }
            else
                AudioManager.i.StopBattleMusic();*/
            

        }
        else
        {
            //Defeat logic
            if (Trainer != null)
            {
                //pay up
                Trainer = null;
            }
            // black out and go to poke centre
        }
        gC.StateMachine.Pop();
        //playerParty.SetPartyData();
        
    }
}
