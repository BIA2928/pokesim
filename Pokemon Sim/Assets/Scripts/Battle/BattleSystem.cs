using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils.StateMachine;

public enum BattleAction
{
    Move, SwitchPoke, UseItem, Run
}

public enum BattleStates
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    AskToForgetMove,
    ForgettingMove,
    AboutToUseNewPoke,
    OnPartyScreen,
    BattleOver,
    Bag
}

public enum BattleEnvironment { LongGrass, Water, Cave}

public class BattleSystem : MonoBehaviour
{ 
    [Header("Battle Imagery")]
    [SerializeField] Image battleBackground;
    [SerializeField] Sprite grassEnvironment;
    [SerializeField] Sprite waterEnvironment;
    [SerializeField] Sprite caveEnvironment;
    [SerializeField] Sprite stadiumEnvironment;

    [Header("Battle Components")]
    [SerializeField] BattleUnit playerPoke;
    [SerializeField] BattleUnit enemyPoke;
    [SerializeField] BattleDialogue dialogueBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveForgetScreen moveForgetScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image enemyImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] InventoryUI inventoryUI;

    [Header("Music")]
    [SerializeField] AudioClip wildBattleMusic;
    [SerializeField] AudioClip wildVictoryMusic;

    BattleStates state;

    public StateMachine<BattleSystem> StateMachine { get; private set; }

    int currentAction = 0;
    int currentMove = 0;
    bool aboutToUseChoice = true;
    MoveBase currMoveToLearn;

    public PokemonParty PlayerParty { get; private set; } 
    public PokemonParty EnemyParty { get; private set; }
    public Pokemon WildPokemon { get; private set; }

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController enemyTrainer;

    BattleEnvironment battleEnvironment;
    public event Action<bool> OnBattleOver;

    public int SelectedMove { get; set; }
    public BattleAction SelectedAction { get; set; }
    public Pokemon SelectedSwitchPokemon { get; set; }

    public bool IsBattleOver { get; private set; }

    public int NumEscapesTried { get; set; }


    public BattleDialogue DialogueBox => dialogueBox;
    public PartyScreen PartyScreen => partyScreen;
    public Pokemon CurrentAllyPokemon => playerPoke.Pokemon;

    public BattleUnit AllyUnit => playerPoke;
    public BattleUnit EnemyUnit => enemyPoke;
    public bool IsTrainerBattle => isTrainerBattle;

    public TrainerController EnemyTrainer => enemyTrainer;

    public AudioClip WildBattleMusic => wildBattleMusic;
    public AudioClip WildVictoryMusic => wildVictoryMusic;
    public void StartBattle(PokemonParty party, Pokemon wildPokemon, 
        BattleEnvironment battleEnvironment = BattleEnvironment.LongGrass)
    {
        isTrainerBattle = false;
        PlayerParty = party;
        player = PlayerParty.GetComponent<PlayerController>();
        this.WildPokemon = wildPokemon;

        this.battleEnvironment = battleEnvironment;

        dialogueBox.ClearDialogue();
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty party, PokemonParty enemyParty, 
        BattleEnvironment battleEnvironment = BattleEnvironment.LongGrass)
    {
        PlayerParty = party;
        this.EnemyParty = enemyParty;

        isTrainerBattle = true;
        player = PlayerParty.GetComponent<PlayerController>();
        enemyTrainer = enemyParty.GetComponent<TrainerController>();

        this.battleEnvironment = battleEnvironment;
        dialogueBox.ClearDialogue();
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        IsBattleOver = false;
        StateMachine = new StateMachine<BattleSystem>(this);
        partyScreen.Init();
        NumEscapesTried = 0;
        playerPoke.Clear();
        enemyPoke.Clear();
        SetBackground();
        if (!isTrainerBattle)
        {
            playerImage.gameObject.SetActive(true);
            playerPoke.EnableImage(false);
            playerImage.sprite = player.BattleSprite;
            enemyPoke.Setup(WildPokemon);
            yield return enemyPoke.PlayEnterAnimation2(true);
            yield return dialogueBox.TypeDialogue($"A wild {enemyPoke.Pokemon.Base.Name} appeared!");
            
            


            playerImage.gameObject.SetActive(false);
            playerPoke.Setup(PlayerParty.GetFirstHealthy());
            yield return dialogueBox.TypeDialogue($"Go {playerPoke.Pokemon.Base.Name}!");
            yield return playerPoke.PlayEnterAnimation2();
            dialogueBox.SetMoveNames(playerPoke.Pokemon.Moves);
        }
        else
        {
            // Trainer battle

            // Hide information images
            playerPoke.EnableImage(false);
            enemyPoke.EnableImage(false);


            // Show battle sprites
            playerImage.gameObject.SetActive(true);
            enemyImage.gameObject.SetActive(true);

            playerImage.sprite = player.BattleSprite;
            enemyImage.sprite = enemyTrainer.BattleSprite;

            yield return dialogueBox.TypeDialogue($"You are challenged by {enemyTrainer.Name}!");

            // Send out enemy first Poke
            enemyImage.gameObject.SetActive(false);
            var enemyPokemon = EnemyParty.GetFirstHealthy();
                      
            enemyPoke.Setup(enemyPokemon);
            yield return dialogueBox.TypeDialogue($"{enemyTrainer.Name} sent out {enemyPokemon.Base.Name}.");
            yield return enemyPoke.PlayEnterAnimation2();


            // Send out player first Poke
            playerImage.gameObject.SetActive(false);
            var playerPokemon = PlayerParty.GetFirstHealthy();
              
            playerPoke.Setup(playerPokemon);
            yield return dialogueBox.TypeDialogue($"Go {playerPokemon.Base.Name}!");
            yield return playerPoke.PlayEnterAnimation2();

            dialogueBox.SetMoveNames(playerPokemon.Moves);

        }

        dialogueBox.HideActions(false);
        StateMachine.ChangeState(ActionSelectionState.i);
    }

    public void SetBackground()
    {
        battleBackground.sprite = battleEnvironment switch
        {
            BattleEnvironment.LongGrass => grassEnvironment,
            BattleEnvironment.Water => waterEnvironment,
            BattleEnvironment.Cave => caveEnvironment,
            _ => grassEnvironment,
        };
    }

    public void HandleUpdate()
    {
        StateMachine.Execute();
        if (state == BattleStates.OnPartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleStates.AboutToUseNewPoke)
        {
            HandleAboutToUse();
        }
        else if (state == BattleStates.AskToForgetMove)
        {
            HandleAskMoveForget();
        }
        else if (state == BattleStates.ForgettingMove)
        {
            StartCoroutine(HandleForgetMove());
        }
        else if (state == BattleStates.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleStates.ActionSelection;
            };

            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                //StartCoroutine(OnItemUsed(usedItem));
            };
            //inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
    }


    void ActionSelection()
    {
        state = BattleStates.ActionSelection;
        StartCoroutine(dialogueBox.TypeDialogue($"What will {playerPoke.Pokemon.Base.Name} do?"));
        dialogueBox.UpdateActionSelection(0);
        dialogueBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        //partyScreen.CalledFromState = state;
        state = BattleStates.OnPartyScreen;
        partyScreen.gameObject.SetActive(true);
    }

    void OpenBag()
    {
        inventoryUI.gameObject.SetActive(true);
        state = BattleStates.Bag;
    }

    void OpenMoveForgetScreen()
    {
        state = BattleStates.ForgettingMove;
        moveForgetScreen.Init();
        moveForgetScreen.SetMoveData(playerPoke.Pokemon.Moves, currMoveToLearn);
        moveForgetScreen.gameObject.SetActive(true);
        //moveForgetScreen.UpdateMoveSelection(0);
    }

    void MoveSelection()
    {
        state = BattleStates.MoveSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    IEnumerator AboutToUse(Pokemon pokemon)
    {
        state = BattleStates.Busy;
        Dialogue d = new Dialogue();
        d.Lines.Add($"{enemyTrainer.Name} is about to use {pokemon.Base.Name}.");
        d.Lines.Add("Do you want to switch?");
        yield return dialogueBox.ShowDialogue(d, false);
        state = BattleStates.AboutToUseNewPoke;
        aboutToUseChoice = true;
        dialogueBox.EnableChoiceSelector(true);

    }

    IEnumerator AskToForget()
    {
        state = BattleStates.Busy;
        yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} wants to learn {currMoveToLearn.Name}.");
        yield return new WaitForSeconds(0.1f);
        yield return dialogueBox.TypeDialogue($"But {playerPoke.Pokemon.Base.Name} already knows four moves.");
        yield return new WaitForSeconds(0.1f);
        yield return dialogueBox.TypeDialogue($"Shall {playerPoke.Pokemon.Base.Name} forget a move to make space for {currMoveToLearn.Name}?");
        state = BattleStates.AskToForgetMove;
        aboutToUseChoice = true;
        dialogueBox.EnableChoiceSelector(true);
    }

    /*
    IEnumerator OnItemUsed(ItemBase usedItem)
    {
        state = BattleStates.Busy;
        inventoryUI.gameObject.SetActive(false);
        if (usedItem is PokeballItem)
        {
            yield return ThrowPokeball((PokeballItem)usedItem);
        }
        StartCoroutine(RunTurns(BattleAction.UseItem));
    }*/

    public void BattleOver(bool playerWin)
    {
        IsBattleOver = true;

        PlayerParty.PokemonList.ForEach(p => p.OnBattleOver());
        playerPoke.BattleHUD.ClearData();
        enemyPoke.BattleHUD.ClearData();
        if (playerWin)
        {
            enemyPoke.ResetAfterFaint();
            OnBattleOver(playerWin);
        }
        else
        {
            playerPoke.ResetAfterFaint();
            OnBattleOver(!playerWin);
        }
    }

    void HandleActionSelection()
    {
        int prevAction = currentAction;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        if (prevAction != currentAction)
        {
            dialogueBox.UpdateActionSelection(currentAction);
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }
        

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.i.PlaySFX(AudioID.UISelect);
            if (currentAction == 0)
            {
                // Fihgt
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                OpenBag();
            }
            else if (currentAction == 2)
            {
                // Pokemon
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Run
                if (isTrainerBattle)
                {
                    //StartCoroutine(dialogueBox.TypeDialogue($"There's no running from\n a trainer battle!"));
                    dialogueBox.IsBusy = true;
                    StartCoroutine(HandleRunFromBattle());
                }
                else
                {
                    //StartCoroutine(RunTurns(BattleAction.Run));
                }

            }
        }
    }

    private IEnumerator HandleRunFromBattle()
    {
        Debug.Log("Set state to busy");
        state = BattleStates.Busy;
        yield return dialogueBox.ShowDialogue($"There's no running from a trainer battle!", true, true);
        yield return new WaitUntil(() => dialogueBox.IsBusy == false);
        dialogueBox.EnableActionSelector(true);
        yield return dialogueBox.TypeDialogue($"What will {playerPoke.Pokemon.Base.Name} do?");
        state = BattleStates.ActionSelection;
        Debug.Log("Set state back to actionSeleciton");
    }

    void HandleMoveSelection()
    {
        int prevMove = currentMove;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;
        else if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            ActionSelection();
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.i.PlaySFX(AudioID.UISelect);
            var move = playerPoke.Pokemon.Moves[currentMove];
            if (move.PP == 0)
            {
                StartCoroutine(dialogueBox.TypeDialogue("Can't select a move with 0 PP!"));
                return;
            }
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            //StartCoroutine(RunTurns(BattleAction.Move));
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerPoke.Pokemon.Moves.Count - 1);

        if (currentMove != prevMove)
        {
            dialogueBox.UpdateMoveSelection(currentMove, playerPoke.Pokemon.Moves[currentMove]);
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }
        

    }

    public void HandlePartySelection()
    {
        Action onSelected = () =>
        {
            var selectedMember = partyScreen.SelectedMember;
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText($"{selectedMember.Base.Name} has fainted and cannot battle.");
                return;
            }
            if (selectedMember == playerPoke.Pokemon)
            {
                partyScreen.SetMessageText($"{selectedMember.Base.Name} is already in battle!");
                return;
            }
            partyScreen.gameObject.SetActive(false);
            /*if (partyScreen.CalledFromState == BattleStates.ActionSelection)
            {

                StartCoroutine(RunTurns(BattleAction.SwitchPoke));
            }
            else
            {
                state = BattleStates.Busy;
                bool isTrainerAboutToUse = partyScreen.CalledFromState == BattleStates.AboutToUseNewPoke;
                StartCoroutine(SwitchPokemon(selectedMember, isTrainerAboutToUse));
            }
            partyScreen.CalledFromState = null;*/
        };

        Action onBack = () =>
        {
            if (playerPoke.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You must select a pokemon to continue.");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            /*if (partyScreen.CalledFromState == BattleStates.AboutToUseNewPoke)
            {

                StartCoroutine(SendNextTrainerPokemon());
            }
            else
            {
                ActionSelection();
                state = BattleStates.ActionSelection;
            }
            partyScreen.CalledFromState = null;*/
        };


        //partyScreen.HandleUpdate(onSelected, onBack);

    }

    IEnumerator HandleForgetMove()
    {
        int prevSelection = currentMove;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, 3);

        if (currentMove != prevSelection)
        {
            //moveForgetScreen.UpdateMoveSelection(currentMove);
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }
        

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMove = playerPoke.Pokemon.Moves[currentMove];
            AudioManager.i.PlaySFX(AudioID.UISelect);
            playerPoke.Pokemon.ReplaceMove(selectedMove, new Move(currMoveToLearn));
            dialogueBox.SetMoveNames(playerPoke.Pokemon.Moves);
            moveForgetScreen.gameObject.SetActive(false);
            yield return dialogueBox.TypeDialogue($"1.....2.....3..... and poof!");
            yield return new WaitForSeconds(0.5f);
            yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} forgot {selectedMove.Base.Name} and learnt {currMoveToLearn.Name}!");
            state = BattleStates.ActionSelection;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            moveForgetScreen.gameObject.SetActive(false);
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
            state = BattleStates.AskToForgetMove;
            yield return AskToForget();
        }
        yield return null;
    }


    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            aboutToUseChoice = !aboutToUseChoice;
            dialogueBox.UpdateChoiceSelection(aboutToUseChoice);
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableChoiceSelector(false);
            AudioManager.i.PlaySFX(AudioID.UISelect);
            if (aboutToUseChoice)
            {
                // Switch
                //prevState = BattleStates.AboutToUseNewPoke;
                OpenPartyScreen();
            }
            else
            {
                // Continue
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
            dialogueBox.EnableChoiceSelector(false);
            StartCoroutine(SendNextTrainerPokemon());
        }

    }

    void HandleAskMoveForget()
    {
        Debug.Log("Checkpoint2");
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            aboutToUseChoice = !aboutToUseChoice;
            dialogueBox.UpdateChoiceSelection(aboutToUseChoice);
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableChoiceSelector(false);
            AudioManager.i.PlaySFX(AudioID.UISelect);
            if (aboutToUseChoice)
            {
                // Forget move for new move
                OpenMoveForgetScreen();
            }
            else
            {
                // Don't forget, continue turns
                StartCoroutine(dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} did not learn {currMoveToLearn.Name}."));
                state = BattleStates.ActionSelection;
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // Also continue turns
            dialogueBox.EnableChoiceSelector(false);
            AudioManager.i.PlaySFX(AudioID.UISwitchSelection);
            StartCoroutine(dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} did not learn {currMoveToLearn.Name}."));
            state = BattleStates.ActionSelection;
        }
    }

    

    public IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        playerPoke.Pokemon.CureVolatileCondition();
        bool pokemonFainted = !(playerPoke.Pokemon.HP > 0);
        dialogueBox.HideActions(true);
        if (!pokemonFainted)
        {
            yield return dialogueBox.TypeDialogue(
                $"That's enough for now {playerPoke.Pokemon.Base.Name}, come back!");
            yield return playerPoke.PlayFaintAnimation();
            yield return new WaitForSeconds(0.9f);
        }


        playerPoke.Setup(newPokemon);
        yield return playerPoke.PlayEnterAnimation2();
        dialogueBox.SetMoveNames(newPokemon.Moves);
        yield return dialogueBox.TypeDialogue($"Go {newPokemon.Base.Name}!");
    }

    public IEnumerator SendNextTrainerPokemon()
    {

        var nextPoke = EnemyParty.GetFirstHealthy();

        enemyPoke.Setup(nextPoke);
        yield return dialogueBox.TypeDialogue($"{enemyTrainer.Name} sent out {nextPoke.Base.Name}.");
        yield return enemyPoke.PlayEnterAnimation2();

    }

    IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        state = BattleStates.Busy;

        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue($"Cannot capture pokemon in a trainer battle!");
            state = BattleStates.RunningTurn;
            yield break;
        }


        dialogueBox.HideActions(true);
        yield return dialogueBox.TypeDialogue($"{player.Name} used a {pokeballItem.Name}!");

        var pokeballObj = Instantiate(pokeballSprite, playerPoke.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeballItem.BagIcon;
        // Animations

        // Throw and spin
        Vector3 buffer = new Vector3(0, 1f);
        var sequence = DOTween.Sequence();
        sequence.Append(pokeball.transform.DOJump(enemyPoke.transform.position + buffer, 2f, 1, 1.2f));
        sequence.Join(pokeball.transform.DORotate(new Vector3(0, 0, -1080f), 1.2f, RotateMode.FastBeyond360));
        yield return sequence.WaitForCompletion();

        // Shrink into ball
        yield return enemyPoke.PlayCaptureAnimation();

        // Drop to floor
        yield return pokeball.transform.DOMoveY(enemyPoke.transform.position.y - 1.25f, 0.4f).WaitForCompletion();
        
        //Shake
        int shakeCount = TryCatchPokemon(enemyPoke.Pokemon, pokeballItem);
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 8f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            yield return pokeball.DOColor(Color.gray, 0.1f).WaitForCompletion();
            yield return dialogueBox.TypeDialogue($"{enemyPoke.Pokemon.Base.Name} was successfully caught!");
            yield return pokeball.DOFade(0, 1f).WaitForCompletion();

            var inParty = PlayerParty.AddPokemon(enemyPoke.Pokemon);
            if (inParty)
                yield return dialogueBox.TypeDialogue($"{enemyPoke.Pokemon.Base.Name} was added to the party.");
            else
                yield return dialogueBox.TypeDialogue($"{enemyPoke.Pokemon.Base.Name} was sent to the PC.");

            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.3f);
            yield return enemyPoke.PlayBreakOutAnimation();
            if (shakeCount == 3)
            {
                yield return dialogueBox.TypeDialogue($"The wild {enemyPoke.Pokemon.Base.Name} broke free.");
                yield return dialogueBox.TypeDialogue($"It was so close too!");
            }
            else
            {
                yield return dialogueBox.TypeDialogue($"The wild {enemyPoke.Pokemon.Base.Name} broke out!");
            }

            Destroy(pokeball);
            state = BattleStates.RunningTurn;

        }

        //dialogueBox.HideActions(false);
    }

    /// <summary>
    /// Returns shake count. ShakeCount = 4 means caught
    /// </summary>
    /// <param name="pokemon">Pokemon attempting to be caught</param>
    /// <returns></returns>
    int TryCatchPokemon(Pokemon pokemon, PokeballItem pokeballItem)
    {

        float statusBonus = ConditionsDB.GetStatusBonus(pokemon.Cnd);

        float a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * pokeballItem.CatchRateModifier * statusBonus / (3 * pokemon.MaxHP);

        if (a >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
            {
                break;
            }

            shakeCount++;
        }

        return shakeCount;

    }

}
