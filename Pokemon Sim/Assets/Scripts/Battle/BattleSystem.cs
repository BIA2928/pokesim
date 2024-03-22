using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum BattleAction
{
    Move, SwitchPoke, UseItem, Run
}

public enum BattleState
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
    BattleOver
}

public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerPoke;
    [SerializeField] BattleUnit enemyPoke;
    [SerializeField] BattleDialogue dialogueBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveForgetScreen moveForgetScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image enemyImage;
    [SerializeField] GameObject pokeballSprite;

    BattleState state;
    int currentAction;
    int currentMove;
    bool aboutToUseChoice = true;

    MoveBase currMoveToLearn;
    PokemonParty playerParty;
    PokemonParty enemyParty;
    Pokemon wildPokemon;

    bool isTrainerBattle = false;
    PlayerController player;
    TrainerController enemyTrainer;

    public event Action<bool> OnBattleOver;

    int nEscapeAttempts;

    public void StartBattle(PokemonParty party, Pokemon wildPokemon)
    {
        isTrainerBattle = false;
        playerParty = party;
        player = playerParty.GetComponent<PlayerController>();
        this.wildPokemon = wildPokemon;
        partyScreen.Init();
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty party, PokemonParty enemyParty)
    {
        playerParty = party;
        this.enemyParty = enemyParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        enemyTrainer = enemyParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        partyScreen.Init();
        nEscapeAttempts = 0;
        playerPoke.Clear();
        enemyPoke.Clear();
        if (!isTrainerBattle)
        {
            playerPoke.Setup(playerParty.GetFirstHealthy());
            enemyPoke.Setup(wildPokemon);
            dialogueBox.SetMoveNames(playerPoke.Pokemon.Moves);
            yield return dialogueBox.TypeDialogue($"A wild {enemyPoke.Pokemon.Base.Name} appeared!");

        }
        else
        {
            // Trainer battle

            // Hide information images
            playerPoke.gameObject.SetActive(false);
            enemyPoke.gameObject.SetActive(false);


            // Show battle sprites
            playerImage.gameObject.SetActive(true);
            enemyImage.gameObject.SetActive(true);

            playerImage.sprite = player.BattleSprite;
            enemyImage.sprite = enemyTrainer.BattleSprite;

            yield return dialogueBox.TypeDialogue($"You are challenged by {enemyTrainer.Name}!");

            // Send out enemy first Poke
            enemyImage.gameObject.SetActive(false);
            enemyPoke.gameObject.SetActive(true);
            var enemyPokemon = enemyParty.GetFirstHealthy();
            enemyPoke.Setup(enemyPokemon);

            yield return dialogueBox.TypeDialogue($"{enemyTrainer.Name} sent out {enemyPokemon.Base.Name}.");

            // Send out player first Poke
            playerImage.gameObject.SetActive(false);
            playerPoke.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetFirstHealthy();
            playerPoke.Setup(playerPokemon);

            dialogueBox.SetMoveNames(playerPokemon.Moves);

            yield return dialogueBox.TypeDialogue($"Go {playerPokemon.Base.Name}!");

        }

        dialogueBox.HideActions(false);
        ActionSelection();
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.OnPartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.AboutToUseNewPoke)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.AskToForgetMove)
        {
            HandleAskMoveForget();
        }
        else if (state == BattleState.ForgettingMove)
        {
            StartCoroutine(HandleForgetMove());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(ThrowPokeball());
        }
    }


    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogueBox.TypeDialogue($"What will {playerPoke.Pokemon.Base.Name} do?"));
        dialogueBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        partyScreen.CalledFromState = state;
        state = BattleState.OnPartyScreen;
        partyScreen.SetPartyData(playerParty.PokemonList);
        partyScreen.gameObject.SetActive(true);
    }

    void OpenMoveForgetScreen()
    {
        state = BattleState.ForgettingMove;
        moveForgetScreen.Init();
        moveForgetScreen.SetMoveData(playerPoke.Pokemon.Moves, currMoveToLearn);
        moveForgetScreen.gameObject.SetActive(true);      
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    IEnumerator AboutToUse(Pokemon pokemon)
    {
        state = BattleState.Busy;
        yield return dialogueBox.TypeDialogue($"{enemyTrainer.Name} is about to use {pokemon.Base.Name}. Do you want to switch?");
        state = BattleState.AboutToUseNewPoke;
        aboutToUseChoice = true;
        dialogueBox.EnableChoiceSelector(true);

    }

    IEnumerator AskToForget()
    {
        state = BattleState.Busy;
        yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} wants to learn {currMoveToLearn.Name}.");
        yield return new WaitForSeconds(0.1f);
        yield return dialogueBox.TypeDialogue($"But {playerPoke.Pokemon.Base.Name} already knows four moves.");
        yield return new WaitForSeconds(0.1f);
        yield return dialogueBox.TypeDialogue($"Shall {playerPoke.Pokemon.Base.Name} forget a move to make space for {currMoveToLearn.Name}?");
        state = BattleState.AskToForgetMove;
        aboutToUseChoice = true;
        dialogueBox.EnableChoiceSelector(true);
    }

    void BattleOver(bool playerWin)
    {
        state = BattleState.BattleOver;

        playerParty.PokemonList.ForEach(p => p.OnBattleOver());
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

    IEnumerator RunTurns(BattleAction playerAction)
    {
        dialogueBox.HideActions(true);

        state = BattleState.RunningTurn;
        if (playerAction == BattleAction.Move)
        {
            playerPoke.Pokemon.CurrentMove = playerPoke.Pokemon.Moves[currentMove];
            enemyPoke.Pokemon.CurrentMove = enemyPoke.Pokemon.GetRandomMove();

            //Check who goes first
            bool playerFirst = true;
            int playerMovePrio = playerPoke.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePrio = enemyPoke.Pokemon.CurrentMove.Base.Priority;
            if (enemyMovePrio > playerMovePrio)
            {
                playerFirst = false;
            }
            else if (enemyMovePrio == playerMovePrio)
            {
                playerFirst = playerPoke.Pokemon.Speed >= enemyPoke.Pokemon.Speed;
            }

            var first = (playerFirst) ? playerPoke : enemyPoke;
            var second = (playerFirst) ? enemyPoke : playerPoke;

            var secondPoke = second.Pokemon;

            yield return RunMove(first, second, first.Pokemon.CurrentMove);
            yield return RunAfterTurn(first);
            if (state == BattleState.BattleOver) yield break;

            // Second move
            // IF pokemon faints from first move in turn, do not play second move
            if (secondPoke.HP > 0)
            {
                yield return RunMove(second, first, second.Pokemon.CurrentMove);
                yield return RunAfterTurn(second);
                if (state == BattleState.BattleOver) yield break;
            }

        }
        else
        {
            if (playerAction == BattleAction.SwitchPoke)
            {
                var selected = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchPokemon(selected);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                yield return ThrowPokeball();
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            // Is now enemy turn
            var enemyMove = enemyPoke.Pokemon.GetRandomMove();
            yield return RunMove(enemyPoke, playerPoke, enemyMove);
            yield return RunAfterTurn(enemyPoke);
            if (state == BattleState.BattleOver) yield break;

        }

        if (state != BattleState.BattleOver)
            ActionSelection();
        dialogueBox.HideActions(false);
    }

    IEnumerator RunMove(BattleUnit sourcePoke, BattleUnit targetPoke, Move move)
    {
        // Dialogue updated and action area hidden
        //dialogueBox.HideActions(true);


        // Check for conditions preventing a move
        bool canRunMove = sourcePoke.Pokemon.OnStartOfTurn();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourcePoke.Pokemon);
            yield return sourcePoke.BattleHUD.UpdateHP();
            dialogueBox.HideActions(false);

            yield break;
        }
        yield return ShowStatusChanges(sourcePoke.Pokemon);
        --move.PP;
        yield return dialogueBox.TypeDialogue($"{sourcePoke.Pokemon.Base.Name} used {move.Base.Name}.");

        if (!CheckIfMoveHits(move, sourcePoke.Pokemon, targetPoke.Pokemon))
        {
            yield return dialogueBox.TypeDialogue($"{sourcePoke.Pokemon.Base.Name} missed!");
            yield break;
        }

        // Play move animation
        sourcePoke.PlayAttackAnimation();
        yield return new WaitForSeconds(0.5f);
        targetPoke.PlayHitEffect();

        // Apply move
        if (move.Base.MoveType == MoveType.Status)
        {
            yield return RunMoveEffects(move.Base.Effects, sourcePoke.Pokemon, targetPoke.Pokemon, move.Base.Target);
        }
        else
        {
            var damageDetails = targetPoke.Pokemon.TakeDamage(move, playerPoke.Pokemon);

            yield return targetPoke.BattleHUD.UpdateHP();

            yield return TypeDamageDetails(damageDetails, targetPoke.Pokemon.Base.Name);
        }

        // Check secondary effects
        if (move.Base.SecondEffects != null && move.Base.SecondEffects.Count > 0 && targetPoke.Pokemon.HP > 0)
        {
            foreach (var secEffect in move.Base.SecondEffects)
            {
                var rand = UnityEngine.Random.Range(0, 100);
                if (rand < secEffect.Chance)
                    yield return RunMoveEffects(secEffect, sourcePoke.Pokemon, targetPoke.Pokemon, move.Base.Target);
            }
        }

        // Check for faint after attack
        if (targetPoke.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(targetPoke);
        }

        //dialogueBox.HideActions(false);
    }



    IEnumerator ShowStatusChanges(Pokemon poke)
    {

        while (poke.StatusChanges.Count > 0)
        {
            var msg = poke.StatusChanges.Dequeue();
            yield return dialogueBox.TypeDialogue(msg);
        }
    }

    IEnumerator RunAfterTurn(BattleUnit source)
    {
        if (state == BattleState.BattleOver)
            yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);
        // Apply after-turn conditions to pokemon
        source.Pokemon.OnEndOfTurn();
        yield return ShowStatusChanges(source.Pokemon);
        yield return source.BattleHUD.UpdateHP();

        // Check for source poke faint
        if (source.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(source);

            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget mvTarg)
    {
        // Stat Boosting Effects
        if (effects != null)
        {
            if (mvTarg == MoveTarget.Self)
                source.ApplyBoost(effects.Boosts);
            else
                target.ApplyBoost(effects.Boosts);

        }

        // Status Conditions
        if (effects.Cnd != ConditionType.none)
        {
            target.SetCondition(effects.Cnd);
        }


        // Volatile Conditions
        if (effects.VolatileCnd != ConditionType.none)
        {
            target.SetVolatileCondition(effects.VolatileCnd);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    void CheckForBattleOver(BattleUnit fainted)
    {
        if (fainted.IsPlayerUnit)
        {
            var next = playerParty.GetFirstHealthy();
            if (next != null)
            {
                OpenPartyScreen();

            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            if (!isTrainerBattle)
                BattleOver(true);
            else
            {
                var nextPoke = enemyParty.GetFirstHealthy();
                if (nextPoke != null)
                {
                    StartCoroutine(AboutToUse(nextPoke));
                    //StartCoroutine(SendNextTrainerPokemon(nextPoke));
                }
                else
                    BattleOver(true);
            }
        }
    }

    public bool CheckIfMoveHits(Move move, Pokemon sourcePoke, Pokemon targetPoke)
    {
        if (move.Base.NeverMiss)
        {
            return true;
        }

        float moveAccur = move.Base.Accuracy;
        int accuracy = sourcePoke.StatBoosts[Stat.Accuracy];
        int evasiveness = targetPoke.StatBoosts[Stat.Evasiveness];

        var boostValues = new float[] { 1f, 4f / 3, 5f / 3, 2f, 7f / 3, 8f / 3, 3f };

        if (accuracy > 0)
            moveAccur *= boostValues[accuracy];
        else
            moveAccur /= boostValues[-accuracy];

        if (evasiveness > 0)
            moveAccur /= boostValues[-evasiveness];
        else
            moveAccur *= boostValues[evasiveness];

        return (UnityEngine.Random.Range(0, 100) < moveAccur);

    }
    IEnumerator TypeDamageDetails(Pokemon.DamageDetails damageDetails, string name)
    {
        if (damageDetails.TypeEffectiveness > 1.0f)
        {
            // Super effective
            yield return dialogueBox.TypeDialogue("It's super effective!");
        }
        else if (damageDetails.TypeEffectiveness < 1.0f && damageDetails.TypeEffectiveness > 0.0f)
        {
            // Not very effective
            yield return dialogueBox.TypeDialogue("It's not very effective.");
        }
        else if (damageDetails.TypeEffectiveness == 0.0f)
        {
            // No effect
            yield return dialogueBox.TypeDialogue($"It doesn't effect {name}...");
        }

        if (damageDetails.Crit > 1.0f)
        {
            yield return dialogueBox.TypeDialogue("A critical hit!");
        }
    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        if (faintedUnit == playerPoke)
        {
            yield return dialogueBox.TypeDialogue($"{faintedUnit.Pokemon.Base.Name} fainted.");
        }
        else
        {
            float trainerBonus = 1f;
            if (isTrainerBattle)
            {
                yield return dialogueBox.TypeDialogue($"The opponent's {faintedUnit.Pokemon.Base.Name} fainted.");
                trainerBonus = 1.5f;
            }
            else
                yield return dialogueBox.TypeDialogue($"The wild {faintedUnit.Pokemon.Base.Name} fainted.");

            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLvl = faintedUnit.Pokemon.Level;

            int expGain = Mathf.FloorToInt((expYield * enemyLvl * trainerBonus) / 7);

            playerPoke.Pokemon.Exp += expGain;

            yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} gained {expGain} EXP.");
            yield return playerPoke.BattleHUD.SetExpAnimation();

            // Check for level up(s)
            while (playerPoke.Pokemon.LevelUp())
            {
                playerPoke.BattleHUD.SetLevel();
                playerPoke.BattleHUD.UpdateHpOnLevelUp();
                yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} grew to level {playerPoke.Pokemon.Level}!");

                var move = playerPoke.Pokemon.GetMoveAtCurrLevel();
                if (move != null)
                {
                    currMoveToLearn = move.Base;
                    if (playerPoke.Pokemon.Moves.Count < PokemonBase.MaxNMoves)
                    {
                        playerPoke.Pokemon.LearnMove(move);
                        yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} learnt {move.Base.Name}!");
                        dialogueBox.SetMoveNames(playerPoke.Pokemon.Moves);
                    }
                    else
                    {
                        yield return AskToForget();
                        yield return new WaitUntil(() => state != BattleState.ForgettingMove && state != BattleState.AskToForgetMove && state != BattleState.Busy);
                    }
                    yield return new WaitForSeconds(1f);
                }
                currMoveToLearn = null;
                yield return playerPoke.BattleHUD.SetExpAnimation(true);
            }
            yield return new WaitForSeconds(0.8f);
        }

        faintedUnit.PlayFaintAnimation();

        yield return new WaitForSeconds(1f);

        CheckForBattleOver(faintedUnit);
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fihgt
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
                StartCoroutine(RunTurns(BattleAction.UseItem));
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
                    StartCoroutine(dialogueBox.TypeDialogue($"There's no running from\n a trainer battle!"));

                }
                else
                {
                    StartCoroutine(RunTurns(BattleAction.Run));
                }

            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerPoke.Pokemon.Moves.Count);


        dialogueBox.UpdateMoveSelection(currentMove, playerPoke.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerPoke.Pokemon.Moves[currentMove];
            if (move.PP == 0)
            {
                dialogueBox.TypeDialogue("Can't select a move with 0 PP!");
                return;
            }
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }

        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            ActionSelection();
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
            if (partyScreen.CalledFromState == BattleState.ActionSelection)
            {

                StartCoroutine(RunTurns(BattleAction.SwitchPoke));
            }
            else
            {
                state = BattleState.Busy;
                bool isTrainerAboutToUse = partyScreen.CalledFromState == BattleState.AboutToUseNewPoke;
                StartCoroutine(SwitchPokemon(selectedMember, isTrainerAboutToUse));
            }
            partyScreen.CalledFromState = null;
        };

        Action onBack = () =>
        {
            if (playerPoke.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You must select a pokemon to continue.");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            if (partyScreen.CalledFromState == BattleState.AboutToUseNewPoke)
            {

                StartCoroutine(SendNextTrainerPokemon());
            }
            else
            {
                ActionSelection();
                state = BattleState.ActionSelection;
            }
            partyScreen.CalledFromState = null;
        };


        partyScreen.HandleUpdate(onSelected, onBack);

    }

    IEnumerator HandleForgetMove()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, 3);

        moveForgetScreen.UpdateMoveSelection(currentMove);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMove = playerPoke.Pokemon.Moves[currentMove];
            playerPoke.Pokemon.ReplaceMove(selectedMove, new Move(currMoveToLearn));
            dialogueBox.SetMoveNames(playerPoke.Pokemon.Moves);
            moveForgetScreen.gameObject.SetActive(false);
            yield return dialogueBox.TypeDialogue($"1.....2.....3..... and poof!");
            yield return new WaitForSeconds(0.5f);
            yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} forgot {selectedMove.Base.Name} and learnt {currMoveToLearn.Name}!");
            state = BattleState.ActionSelection;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            moveForgetScreen.gameObject.SetActive(false);
            state = BattleState.AskToForgetMove;
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
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableChoiceSelector(false);
            if (aboutToUseChoice)
            {
                // Switch
                //prevState = BattleState.AboutToUseNewPoke;
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
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableChoiceSelector(false);
            if (aboutToUseChoice)
            {
                // Forget move for new move
                OpenMoveForgetScreen();
            }
            else
            {
                // Don't forget, continue turns
                StartCoroutine(dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} did not learn {currMoveToLearn.Name}."));
                state = BattleState.ActionSelection;
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            // Also continue turns
            dialogueBox.EnableChoiceSelector(false);
            StartCoroutine(dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} did not learn {currMoveToLearn.Name}."));
            state = BattleState.ActionSelection;
        }
    }

    

    IEnumerator SwitchPokemon(Pokemon newPokemon, bool isTrainerAboutToUse=false)
    {
        playerPoke.Pokemon.CureVolatileCondition();
        bool pokemonFainted = !(playerPoke.Pokemon.HP > 0);
        dialogueBox.HideActions(true);
        if (!pokemonFainted)
        {
            yield return dialogueBox.TypeDialogue(
                $"That's enough for now {playerPoke.Pokemon.Base.Name}, come back!");
            playerPoke.PlayFaintAnimation();
            yield return new WaitForSeconds(0.9f);
        }


        playerPoke.Setup(newPokemon);
        dialogueBox.SetMoveNames(newPokemon.Moves);
        yield return dialogueBox.TypeDialogue($"Go {newPokemon.Base.Name}!");

        if (isTrainerAboutToUse)
        {
            StartCoroutine(SendNextTrainerPokemon());
        }
        else
        {
            state = BattleState.RunningTurn;
        }
    }

    IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState.Busy;

        var nextPoke = enemyParty.GetFirstHealthy();

        enemyPoke.Setup(nextPoke);

        yield return dialogueBox.TypeDialogue($"{enemyTrainer.Name} sent out {nextPoke.Base.Name}.");

        state = BattleState.RunningTurn;
    }

    IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue($"Cannot capture pokemon in a trainer battle!");
            state = BattleState.RunningTurn;
            yield break;
        }


        dialogueBox.HideActions(true);
        yield return dialogueBox.TypeDialogue($"{player.Name} used a {pokeballSprite.name}!");

        var pokeballObj = Instantiate(pokeballSprite, playerPoke.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();

        // Animations
        // Throw
        Vector3 buffer = new Vector3(0, 1f);
        yield return pokeball.transform.DOJump(enemyPoke.transform.position + buffer, 2f, 1, 1.2f).WaitForCompletion();
        // Shrink into ball
        yield return enemyPoke.PlayCaptureAnimation();
        // Drop to floor
        yield return pokeball.transform.DOMoveY(enemyPoke.transform.position.y - 1.25f, 0.4f).WaitForCompletion();
        //Shake

        int shakeCount = TryCatchPokemon(enemyPoke.Pokemon);
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

            var inParty = playerParty.AddPokemon(enemyPoke.Pokemon);
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
            state = BattleState.RunningTurn;

        }

        //dialogueBox.HideActions(false);
    }

    /// <summary>
    /// Returns shake count. ShakeCount = 4 means caught
    /// </summary>
    /// <param name="pokemon">Pokemon attempting to be caught</param>
    /// <returns></returns>
    int TryCatchPokemon(Pokemon pokemon)
    {

        float statusBonus = ConditionsDB.GetStatusBonus(pokemon.Cnd);

        float a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * statusBonus / (3 * pokemon.MaxHP);

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

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue($"There's no runnning from a trainer battle!");
            state = BattleState.ActionSelection;
            yield break;
        }

        int playerPokeSpeed = playerPoke.Pokemon.Speed;
        int enemySpeed = enemyPoke.Pokemon.Speed;

        if (enemySpeed < playerPokeSpeed)
        {
            yield return dialogueBox.TypeDialogue($"Got away safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerPokeSpeed * 128) / enemySpeed + 30 * ++nEscapeAttempts;
            f %= 256;

            if (UnityEngine.Random.Range(0, 266) < f)
            {
                yield return dialogueBox.TypeDialogue($"Got away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogueBox.TypeDialogue("Couldn't get away!");
                state = BattleState.RunningTurn;
            }
        }

    }

}
