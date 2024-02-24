using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerPoke;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleUnit enemyPoke;
    [SerializeField] BattleHUD enemyHUD;
    [SerializeField] BattleDialogue dialogueBox;

    BattleState state;
    int currentAction;
    int currentMove;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerPoke.Setup();
        playerHUD.SetData(playerPoke.Pokemon);
        enemyPoke.Setup();
        enemyHUD.SetData(enemyPoke.Pokemon);

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
        yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} used {move.Base.Name}.");

        var damageDetails = enemyPoke.Pokemon.TakeDamage(move, playerPoke.Pokemon);
        yield return enemyHUD.UpdateHP();
        yield return TypeDamageDetails(damageDetails, enemyPoke.Pokemon.Base.Name);


        if (damageDetails.DidFaint)
        {
            yield return dialogueBox.TypeDialogue($"The wild {enemyPoke.Pokemon.Base.Name} fainted.");
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
        yield return dialogueBox.TypeDialogue($"The wild {enemyPoke.Pokemon.Base.Name} used {move.Base.Name}.");

        var damageDetails = playerPoke.Pokemon.TakeDamage(move, playerPoke.Pokemon);
        yield return playerHUD.UpdateHP();
        yield return TypeDamageDetails(damageDetails, playerPoke.Pokemon.Base.Name);


        if (damageDetails.DidFaint)
        {
            yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} fainted.");
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

    private void Update()
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
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
            {
                ++currentAction;
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                --currentAction;
            }
        }

        dialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fihgt
                PlayerMove();
            } 
            else
            {
                // Run
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerPoke.Pokemon.Moves.Count - 1)
                ++currentMove;
        } 
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)){
            if (currentMove < playerPoke.Pokemon.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogueBox.UpdateMoveSelection(currentMove, playerPoke.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}
