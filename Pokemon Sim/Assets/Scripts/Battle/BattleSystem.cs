using System;
using System.Collections;
using UnityEngine;

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
    OnPartyScreen,
    BattleOver
}

public class BattleSystem : MonoBehaviour
{

    [SerializeField] BattleUnit playerPoke;
    [SerializeField] BattleUnit enemyPoke;
    [SerializeField] BattleDialogue dialogueBox;
    [SerializeField] PartyScreen partyScreen;

    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currPartyPoke;

    PokemonParty playerParty;
    Pokemon wildPokemon;

    public event Action<bool> OnBattleOver;

    public void StartBattle(PokemonParty party, Pokemon wildPokemon)
    {
        playerParty = party;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerPoke.Setup(playerParty.GetFirstHealthy());

        enemyPoke.Setup(wildPokemon);

        partyScreen.Init();

        dialogueBox.SetMoveNames(playerPoke.Pokemon.Moves);

        yield return dialogueBox.TypeDialogue($"A wild {enemyPoke.Pokemon.Base.Name} appeared!");

        ActionSelection();
    }


    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogueBox.TypeDialogue($"What will {playerPoke.Pokemon.Base.Name} do?"));
        dialogueBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.OnPartyScreen;
        partyScreen.SetPartyData(playerParty.PokemonList);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    void BattleOver(bool playerWin)
    {
        state = BattleState.BattleOver;
        enemyPoke.ResetPosition();
        playerParty.PokemonList.ForEach(p => p.OnBattleOver());
        OnBattleOver(playerWin);

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
            if(playerAction == BattleAction.SwitchPoke)
            {
                var selected = playerParty.PokemonList[currPartyPoke];
                state = BattleState.Busy;
                yield return SwitchPokemon(selected);
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
        if(move.Base.MoveType == MoveType.Status)
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
            foreach  (var secEffect in move.Base.SecondEffects)
            {
                var rand = UnityEngine.Random.Range(0, 100);
                if (rand < secEffect.Chance)
                    yield return RunMoveEffects(secEffect, sourcePoke.Pokemon, targetPoke.Pokemon, move.Base.Target);
            }
        }

        // Check for faint after attack
        if (targetPoke.Pokemon.HP <= 0)
        {
            yield return dialogueBox.TypeDialogue($"The wild {targetPoke.Pokemon.Base.Name} fainted.");
            targetPoke.PlayFaintAnimation();

            yield return new WaitForSeconds(1f);

            CheckForBattleOver(targetPoke);
            
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
            yield return dialogueBox.TypeDialogue($"{source.Pokemon.Base.Name} fainted.");
            source.PlayFaintAnimation();

            yield return new WaitForSeconds(1f);

            CheckForBattleOver(source);
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
            BattleOver(true);
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
            }
            else if (currentAction == 2)
            {
                // Pokemon
                prevState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Run
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
            dialogueBox.EnableMoveSelector(true);
            dialogueBox.EnableDialogueText(true);
            ActionSelection();
        }
    }

    public void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currPartyPoke;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currPartyPoke;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currPartyPoke += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currPartyPoke -= 2;

        currPartyPoke = Mathf.Clamp(currPartyPoke, 0, playerParty.PokemonList.Count - 1);

        partyScreen.UpdatePokemonSelection(currPartyPoke);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.PokemonList[currPartyPoke];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText($"{playerParty.PokemonList[currPartyPoke].Base.Name} has fainted and cannot battle.");
                return;
            }
            if (selectedMember == playerPoke.Pokemon)
            {
                partyScreen.SetMessageText($"{playerParty.PokemonList[currPartyPoke].Base.Name} is already in battle!");
                return;
            }
            partyScreen.gameObject.SetActive(false);
            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPoke));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
  

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
            state = BattleState.ActionSelection;
        }

    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
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

        state = BattleState.RunningTurn;
        dialogueBox.HideActions(false);
    }
}
