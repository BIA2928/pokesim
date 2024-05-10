using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.StateMachine;

public class RunTurnState : State<BattleSystem>
{
    public static RunTurnState i { get; private set; }

    void Awake()
    {
        i = this;
    }
    BattleSystem bS;

    BattleUnit playerUnit;
    BattleUnit enemyUnit;
    BattleDialogue dialogueBox;
    MoveBase currMoveToLearn;
    public override void EnterState(BattleSystem owner)
    {
        bS = owner;
        playerUnit = bS.AllyUnit;
        enemyUnit = bS.EnemyUnit;
        dialogueBox = bS.DialogueBox;

        StartCoroutine(RunTurns(bS.SelectedAction));
    }

    public override void Execute()
    {
        base.Execute();
    }

    public override void ExitState()
    {
        base.ExitState();
    }


    IEnumerator RunTurns(BattleAction playerAction)
    {
        dialogueBox.HideActions(true);
        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[bS.SelectedMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            //Check who goes first
            bool playerFirst = true;
            int playerMovePrio = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePrio = enemyUnit.Pokemon.CurrentMove.Base.Priority;
            if (enemyMovePrio > playerMovePrio)
            {
                playerFirst = false;
            }
            else if (enemyMovePrio == playerMovePrio)
            {
                playerFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;
            }

            var first = (playerFirst) ? playerUnit : enemyUnit;
            var second = (playerFirst) ? enemyUnit : playerUnit;

            var secondPoke = second.Pokemon;

            yield return RunMove(first, second, first.Pokemon.CurrentMove);
            yield return RunAfterTurn(first);
            if (bS.IsBattleOver) yield break;

            // Second move
            // IF pokemon faints from first move in turn, do not play second move
            if (secondPoke.HP > 0)
            {
                yield return RunMove(second, first, second.Pokemon.CurrentMove);
                yield return RunAfterTurn(second);
                if (bS.IsBattleOver) yield break;
            }

        }
        else
        {
            if (playerAction == BattleAction.SwitchPoke)
            {
                yield return bS.SwitchPokemon(bS.SelectedSwitchPokemon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                if (bS.SelectedItem is PokeballItem)
                {
                    yield return bS.ThrowPokeball(bS.SelectedItem as PokeballItem);
                    if (bS.IsBattleOver) yield break;
                }
                else
                {

                }
                dialogueBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            // Is now enemy turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (bS.IsBattleOver) yield break;

        }

        if (!bS.IsBattleOver)
            bS.StateMachine.ChangeState(ActionSelectionState.i);

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
            yield return sourcePoke.BattleHUD.WaitForHPUpdate();
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
        if (move.Base.Sound != null)
        {
            AudioManager.i.PlaySFX(move.Base.Sound);
            yield return new WaitForSeconds(move.Base.Sound.length);
        }

        targetPoke.PlayHitEffect();


        // Apply move
        if (move.Base.MoveType == MoveType.Status)
        {
            yield return RunMoveEffects(move.Base.Effects, sourcePoke.Pokemon, targetPoke.Pokemon, move.Base.Target);
        }
        else
        {
            var damageDetails = targetPoke.Pokemon.TakeDamage(move, playerUnit.Pokemon);
            if (damageDetails.TypeEffectiveness >= 2)
                AudioManager.i.PlaySFX(AudioID.HitSprEft);
            else if (damageDetails.TypeEffectiveness < 1)
                AudioManager.i.PlaySFX(AudioID.HitNVEft);
            else
                AudioManager.i.PlaySFX(AudioID.Hit);
            yield return targetPoke.BattleHUD.WaitForHPUpdate();

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
        if (bS.IsBattleOver)
            yield break;
        // Apply after-turn conditions to pokemon
        source.Pokemon.OnEndOfTurn();
        yield return ShowStatusChanges(source.Pokemon);
        yield return source.BattleHUD.WaitForHPUpdate();

        // Check for source poke faint
        if (source.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(source);

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

    IEnumerator TypeDamageDetails(DamageDetails damageDetails, string name)
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

        return (Random.Range(0, 100) < moveAccur);

    }

    IEnumerator CheckForBattleOver(BattleUnit fainted)
    {
        if (fainted.IsPlayerUnit)
        {
            var next = bS.PlayerParty.GetFirstHealthy();
            if (next != null)
            {
                yield return GameController.i.StateMachine.PushAndWait(PartyScreenState.i);
                var pokemon = PartyScreenState.i.SelectedPokemon;
                yield return bS.SwitchPokemon(pokemon);
            }
            else
            {
                bS.BattleOver(false);
            }
        }
        else
        {
            if (!bS.IsTrainerBattle)
                bS.BattleOver(true);
            else
            {
                var nextPoke = bS.EnemyParty.GetFirstHealthy();
                if (nextPoke != null)
                {
                    AboutToUseState.i.NewPokemon = nextPoke;
                    yield return bS.StateMachine.PushAndWait(AboutToUseState.i);
                }
                else
                    bS.BattleOver(true);
            }
        }
    }

    IEnumerator TryToEscape()
    {

        if (bS.IsTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue($"There's no runnning from a trainer battle!");
            bS.StateMachine.ChangeState(ActionSelectionState.i);
            yield break;
        }

        int playerPokeSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if (enemySpeed < playerPokeSpeed)
        {
            yield return dialogueBox.TypeDialogue($"Got away safely!");
            bS.BattleOver(true);
        }
        else
        {
            float f = (playerPokeSpeed * 128) / enemySpeed + 30 * ++bS.NumEscapesTried;
            f %= 256;

            if (Random.Range(0, 266) < f)
            {
                yield return dialogueBox.TypeDialogue($"Got away safely!");
                bS.BattleOver(true);
            }
            else
            {
                yield return dialogueBox.TypeDialogue("Couldn't get away!");
            }
        }

    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        if (faintedUnit == playerUnit)
        {
            yield return dialogueBox.TypeDialogue($"{faintedUnit.Pokemon.Base.Name} fainted.");
        }
        else
        {
            bool battleWon = true;
            if (bS.IsTrainerBattle)
                battleWon = (bS.EnemyParty.GetFirstHealthy() == null);

            float trainerBonus = 1f;
            if (bS.IsTrainerBattle)
            {
                yield return dialogueBox.TypeDialogue($"The foe's {faintedUnit.Pokemon.Base.Name} fainted.");
                trainerBonus = 1.5f;
            }
            else
            {
                yield return dialogueBox.TypeDialogue($"The wild {faintedUnit.Pokemon.Base.Name} fainted.");
            }
            if (battleWon)
            {
                if (bS.IsTrainerBattle)
                    AudioManager.i.PlayMusic(bS.EnemyTrainer.VictoryMusic, fade: false);
                else
                {
                    AudioManager.i.PlayMusic(bS.WildVictoryMusic, fade: false);
                    Debug.Log("Playing wild victory music!");
                }

            }

            int expYield = faintedUnit.Pokemon.Base.ExpYield;
            int enemyLvl = faintedUnit.Pokemon.Level;

            int expGain = Mathf.FloorToInt((expYield * enemyLvl * trainerBonus) / 7);

            playerUnit.Pokemon.Exp += expGain;

            yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} gained {expGain} EXP.");
            yield return playerUnit.BattleHUD.SetExpAnimation();

            // Check for level up(s)
            while (playerUnit.Pokemon.LevelUp())
            {
                playerUnit.BattleHUD.SetLevel();
                playerUnit.BattleHUD.UpdateHpOnLevelUp();
                AudioManager.i.PlaySFX(AudioID.LvlUp);
                yield return new WaitUntil(() => AudioManager.i.ExtraAudioPlayer.isPlaying == false);
                yield return dialogueBox.TypeDialogue($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}!");

                var move = playerUnit.Pokemon.GetMoveAtCurrLevel();
                if (move != null)
                {
                    currMoveToLearn = move.Base;
                    if (playerUnit.Pokemon.Moves.Count < PokemonBase.MaxNMoves)
                    {
                        playerUnit.Pokemon.LearnMove(move);
                        AudioManager.i.PlaySFX(AudioID.LvlUp);
                        yield return new WaitUntil(() => AudioManager.i.ExtraAudioPlayer.isPlaying == false);
                        yield return dialogueBox.ShowDialogue($"{playerUnit.Pokemon.Base.Name} learnt {move.Base.Name}!", hideActions: true);
                        dialogueBox.SetMoveNames(bS.AllyUnit.Pokemon.Moves);
                    }
                    else
                    {
                        AskToForgetState.i.MoveToLearn = move.Base;
                        yield return bS.StateMachine.PushAndWait(AskToForgetState.i);

                        if (AskToForgetState.i.ForgetMoveChoice)
                        {
                            ForgettingMoveState.i.Moves = playerUnit.Pokemon.Moves;
                            ForgettingMoveState.i.MoveToLearn = move.Base;
                            yield return GameController.i.StateMachine.PushAndWait(ForgettingMoveState.i);

                            int moveIndex = ForgettingMoveState.i.Selection;
                            if (moveIndex == -1 || moveIndex >= bS.AllyUnit.Pokemon.Moves.Count)
                            {
                                yield return dialogueBox.ShowDialogue($"{bS.AllyUnit.Pokemon.Base.Name} did not learn {currMoveToLearn.Name}", hideActions:true);
                            }
                            else
                            {
                                yield return dialogueBox.TypeDialogue($"3...2...1... and poof!");
                                
                                yield return new WaitForSeconds(0.9f);
                                var replacedMove = bS.AllyUnit.Pokemon.Moves[moveIndex].Base.Name;
                                bS.AllyUnit.Pokemon.ReplaceMove(bS.AllyUnit.Pokemon.Moves[moveIndex], new Move(currMoveToLearn));
                                AudioManager.i.PlaySFX(AudioID.LvlUp);
                                yield return dialogueBox.ShowDialogue($"{bS.AllyUnit.Pokemon.Base.Name} forgot {replacedMove} and learned {currMoveToLearn.Name}!", hideActions: true);
                            }
                        }
                        else
                        {
                            yield return dialogueBox.ShowDialogue($"{bS.AllyUnit.Pokemon.Base.Name} did not learn {currMoveToLearn.Name}", hideActions: true); 
                        }
                        
                    }
                    yield return new WaitForSeconds(1f);
                }
                currMoveToLearn = null;
                yield return playerUnit.BattleHUD.SetExpAnimation(true);
            }
            yield return new WaitForSeconds(0.8f);
        }

        yield return faintedUnit.PlayFaintAnimation();

        yield return new WaitForSeconds(1f);

        yield return CheckForBattleOver(faintedUnit);
    }

    
}
