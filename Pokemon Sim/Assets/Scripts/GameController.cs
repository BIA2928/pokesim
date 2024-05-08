using System;
using System.Collections;
using UnityEngine;
using Utils.StateMachine;

public enum GameState
{
    FreeRoam,
    Cutscene,
    InBattle,
    InDialogue,
    Paused,
    InMenu,
    InPartyScreen,
    InBag,
    InEvolution,
    Shop,
}

public class GameController : MonoBehaviour
{
    GameState state;
    GameState prevState;
    GameState stateBeforeEvo;

    public StateMachine<GameController> StateMachine { get; private set; }

    [SerializeField] PlayerController pC;
    [SerializeField] BattleSystem bS;
    [SerializeField] Camera worldCam;
    [SerializeField] PartyScreen pS;
    [SerializeField] InventoryUI iUI;

    public static GameController i;

    TrainerController currentTrainer;
    
    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PrevScene { get; private set; }

    public Camera WorldCam => worldCam;
    public void SurfMusicChange(bool IsSurfing)
    {
        Debug.Log("Changing music");
        if (IsSurfing)
            AudioManager.i.PlaySurfMusic();
        else
            AudioManager.i.PlayMusic(CurrentScene.SceneMusic);
        Debug.Log("Music change complete");
    }

    private void Awake()
    {
        i = this;
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
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.ChangeState(FreeRoamState.i);
        pS.Init();
        bS.OnBattleOver += EndBattle;

        DialogueManager.Instance.OnShowDialogue += () =>
        {
            StateMachine.Push(DialogueState.i);
        };

        DialogueManager.Instance.OnCloseDialogue += () =>
        {
            StateMachine.Pop();
        };

        EvolutionManager.i.OnStartEvolution += () =>
        {
            stateBeforeEvo = state;
            state = GameState.InEvolution;
        };

        EvolutionManager.i.OnFinishEvolution += () =>
        {
            state = stateBeforeEvo;
            pS.SetPartyData();
            AudioManager.i.PlayMusic(CurrentScene.SceneMusic);
        };

        ShopController.instance.OnStartShopping += () =>
        {
            state = GameState.Shop;
        };
        ShopController.instance.OnCloseShop += () =>
        {
            state = GameState.FreeRoam;
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

    public void OnEnterTrainerFOV(TrainerController trainer)
    {
        
        StartCoroutine(trainer.TriggerBattle(pC));
    }

    public void StartBattle(BattleEnvironment environment)
    {
        BattleState.i.CurrEnvironment = environment;
        StateMachine.Push(BattleState.i);
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        currentTrainer = trainer;
        BattleState.i.Trainer = trainer;
        BattleState.i.CurrEnvironment = BattleEnvironment.LongGrass;
        StateMachine.Push(BattleState.i);
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
            bool willEvolve = playerParty.CheckForEvolution();
            if (willEvolve)
            {
                state = GameState.FreeRoam;
                StartCoroutine(playerParty.RunEvolutions());
            }
            
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

        pS.SetPartyData();
        state = GameState.FreeRoam;
        bS.gameObject.SetActive(false);
        worldCam.gameObject.SetActive(true);

        
    }

    private void Update()
    {
        StateMachine.Execute();

        if (state == GameState.Shop)
        {
            ShopController.instance.HandleUpdate();
        }

        
    }

    public void SetCurrentScene(SceneDetails scene)
    {
        PrevScene = CurrentScene;
        CurrentScene = scene;
    }


    public IEnumerator MoveCamera(Vector2 moveVec, bool waitForFade=false)
    {
        yield return Fader.instance.FadeIn(0.5f);
        worldCam.transform.position += new Vector3(moveVec.x, moveVec.y);
        if (waitForFade)
            yield return Fader.instance.FadeOut(0.5f);
        else
            StartCoroutine(Fader.instance.FadeOut(0.5f));
    }

    //StateStack Tesing Purposes Only
    private void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 22;
        GUILayout.Label("STATE STACK", style);
        foreach(var state in StateMachine.StateStack)
        {
            GUILayout.Label(state.GetType().ToString(), style);
        }
    }

    public GameState GameState => state;
}
