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
        yield return new WaitForSeconds(1.1f);

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
        yield return new WaitForSeconds(0.8f);

        bool hasFainted = enemyPoke.Pokemon.TakeDamage(move, playerPoke.Pokemon);
        yield return enemyHUD.UpdateHP();

        if (hasFainted)
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
        yield return new WaitForSeconds(0.75f);

        bool hasFainted = playerPoke.Pokemon.TakeDamage(move, playerPoke.Pokemon);
        yield return playerHUD.UpdateHP();

        if (hasFainted)
        {
            yield return dialogueBox.TypeDialogue($"{playerPoke.Pokemon.Base.Name} fainted.");
        }
        else
        {
            PlayerAction();
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
