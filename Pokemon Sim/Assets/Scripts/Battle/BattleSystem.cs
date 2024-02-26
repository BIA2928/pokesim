using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerPoke;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleUnit enemyPoke;
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleDialogue dialogueBox;
    [SerializeField] PartyScreen partyScreen;

    BattleState state;
    int currentAction;
    int currentMove;

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
        playerHUD.SetData(playerPoke.Pokemon);

        enemyPoke.Setup(wildPokemon);
        enemyHUD.SetData(enemyPoke.Pokemon);

        partyScreen.Init();

        dialogueBox.SetMoveNames(playerPoke.Pokemon.Moves);

        yield return dialogueBox.TypeDialogue($"A wild {enemyPoke.Pokemon.Base.Name} appeared!");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogueBox.TypeDialogue($"What will {playerPoke.Pokemon.Base.Name} do?"));
        dialogueBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        partyScreen.SetPartyData(playerParty.PokemonList);
        partyScreen.gameObject.SetActive(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;
        var move = playerPoke.Pokemon.Moves[currentMove];
        --move.PP;
        yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} used {move.Base.Name}.");

        playerPoke.PlayAttackAnimation();
        yield return new WaitForSeconds(0.5f);
        enemyPoke.PlayHitEffect();

        var damageDetails = enemyPoke.Pokemon.TakeDamage(move, playerPoke.Pokemon);
        yield return enemyHUD.UpdateHP();
        yield return TypeDamageDetails(damageDetails, enemyPoke.Pokemon.Base.Name);


        if (damageDetails.DidFaint)
        {
            yield return dialogueBox.TypeDialogue($"The wild {enemyPoke.Pokemon.Base.Name} fainted.");
            enemyPoke.PlayFaintAnimation();

            yield return new WaitForSeconds(1f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;
        var move = enemyPoke.Pokemon.GetRandomMove();
        --move.PP;
        yield return dialogueBox.TypeDialogue($"The wild {enemyPoke.Pokemon.Base.Name} used {move.Base.Name}.");

        enemyPoke.PlayAttackAnimation();
        yield return new WaitForSeconds(0.5f);
        playerPoke.PlayHitEffect();

        var damageDetails = playerPoke.Pokemon.TakeDamage(move, playerPoke.Pokemon);
        yield return playerHUD.UpdateHP();
        yield return TypeDamageDetails(damageDetails, playerPoke.Pokemon.Base.Name);


        if (damageDetails.DidFaint)
        {
            yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} fainted.");
            playerPoke.PlayFaintAnimation();

            yield return new WaitForSeconds(1f);

            var next = playerParty.GetFirstHealthy();
            if (next != null)
            {
                playerPoke.Setup(next);
                playerHUD.SetData(next);

                dialogueBox.SetMoveNames(next.Moves);

                yield return dialogueBox.TypeDialogue($"Go {next.Base.Name}!");

                PlayerAction();
            }
            else
            {
                OnBattleOver(false);
            }
            
        }
        else
        {
            PlayerAction();
        }

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
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
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
                PlayerMove();
            } 
            else if (currentAction == 1)
            {
                // Bag
            }
            else if (currentAction == 2)
            {
                // Pokemon
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
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(PerformPlayerMove());
        }

        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableMoveSelector(true);
            dialogueBox.EnableDialogueText(true);
            PlayerAction();
        }
    }
}
