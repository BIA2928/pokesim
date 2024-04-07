using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    FreeRoam,
    Cutscene,
    InBattle,
    InDialogue,
    Paused,
    InMenu,
    InPartyScreen,
    InBag
}
public class GameController : MonoBehaviour
{
    GameState state;
    GameState prevState;

    [SerializeField] PlayerController pC;
    [SerializeField] BattleSystem bS;
    [SerializeField] Camera worldCam;
    [SerializeField] PartyScreen pS;
    [SerializeField] InventoryUI iUI;

    public static GameController i;

    TrainerController currentTrainer;
    MenuController menuController;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }


    private void Awake()
    {
        i = this;
        menuController = GetComponent<MenuController>();
        ConditionsDB.Init();
        PokemonDB.Init();
        MoveDB.Init();
        ItemDB.Init();
        QuestDB.Init();

        // Mouse not needed, turn off
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    private void Start()
    {
        pS.Init();
        bS.OnBattleOver += EndBattle;
        menuController.onBack += () =>
        {
            state = GameState.FreeRoam;
        };
        menuController.onMenuSlotSelected += OnMenuSelection;

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            prevState = state;
            state = GameState.InDialogue;
        };

        DialogueManager.Instance.OnCloseDialogue += () =>
        {
            if (state == GameState.InDialogue)
                state = prevState;
        };
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }

        else
            state = prevState;
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        currentTrainer = trainer;
        state = GameState.InBattle;
        bS.gameObject.SetActive(true);
        worldCam.gameObject.SetActive(false);

        var playerParty = pC.GetComponent<PokemonParty>();
        bS.StartTrainerBattle(playerParty, trainer.GetComponent<PokemonParty>());

    }

    public void OnEnterTrainerFOV(TrainerController trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerBattle(pC));
    }

    public void StartBattle()
    {
        state = GameState.InBattle;
        bS.gameObject.SetActive(true);
        worldCam.gameObject.SetActive(false);

        var playerParty = pC.GetComponent<PokemonParty>();
        var wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon();
        var copy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
        bS.StartBattle(playerParty, copy);

    }

    void EndBattle(bool won)
    {
        if (won)
        {
            if (currentTrainer != null)
            {
                currentTrainer.Beat();
                currentTrainer = null;
            }

            var playerParty = pC.GetComponent<PokemonParty>();
            StartCoroutine(playerParty.CheckForEvolutions());
        }
        else
        {
            //Defeat logic
            if (currentTrainer != null)
            {
                //pay up
                currentTrainer = null;
            }
            // black out and go to poke centre
        }
        
        state = GameState.FreeRoam;
        bS.gameObject.SetActive(false);
        worldCam.gameObject.SetActive(true);

        
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            pC.HandleUpdate();
            if (Input.GetKeyDown(KeyCode.Return))
            {
                pC.StopPlayerMovement();
                //Pause all NPCs as well
                menuController.OpenMenu();
                state = GameState.InMenu;
            }
            // Only for testing purposes, to be remvoed later
            else if (Input.GetKeyDown(KeyCode.L))
            {
                SavingSystem.i.Load("testSave1");
            }
        } 
        else if (state == GameState.InBattle)
        {
            bS.HandleUpdate();
        }
        else if (state == GameState.InDialogue)
        {
            DialogueManager.Instance.HandleUpdate();
        }
        else if (state == GameState.InMenu)
        {
            menuController.HandleUpdate();
        }
        else if (state == GameState.InPartyScreen)
        {
            Action onSelected = () => 
            {
                // Open summary screen
            };
            Action onBack = () => 
            {
                pS.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            pS.HandleUpdate(onSelected, onBack);
        }
        else if (state == GameState.InBag)
        {
            Action onBack = () =>
            {
                iUI.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };
            iUI.HandleUpdate(onBack);
        }

        
    }

    public void SetCurrentScene(SceneDetails scene)
    {
        PrevScene = CurrentScene;
        CurrentScene = scene;
    }

    void OnMenuSelection(int selectedIndex)
    {
        if (selectedIndex == 0)
        {
            // Open Party
            pS.gameObject.SetActive(true);
            state = GameState.InPartyScreen;
        }
        else if (selectedIndex == 1)
        {
            //Open pokedex 
        }
        else if (selectedIndex == 2)
        {
            // Open bag
            iUI.gameObject.SetActive(true);
            state = GameState.InBag;
        }
        else if (selectedIndex == 3)
        {
            // Open badges
        }
        else if (selectedIndex == 4)
        {
            // Save
            SavingSystem.i.Save("testSave1");
            state = GameState.FreeRoam;
        }
        else if (selectedIndex == 5)
        {
            // Open options

        }
    }

        public GameState GameState => state;
}
