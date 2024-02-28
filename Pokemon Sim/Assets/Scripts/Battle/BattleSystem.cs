using System;
using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerPoke;
    [SerializeField] BattleUnit enemyPoke;
    [SerializeField] BattleDialogue dialogueBox;
    [SerializeField] PartyScreen partyScreen;

    BattleState state;
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
        OnBattleOver(playerWin);

    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;
        var move = playerPoke.Pokemon.Moves[currentMove];
        yield return RunMove(playerPoke, enemyPoke, move);

        if (state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;
        var move = enemyPoke.Pokemon.GetRandomMove();

        yield return RunMove(enemyPoke, playerPoke, move);

        if (state == BattleState.PerformMove)
            ActionSelection();
        
    }

    IEnumerator RunMove(BattleUnit sourcePoke, BattleUnit targetPoke, Move move)
    {
        --move.PP;
        yield return dialogueBox.TypeDialogue($"{sourcePoke.Pokemon.Base.Name} used {move.Base.Name}.");

        sourcePoke.PlayAttackAnimation();
        yield return new WaitForSeconds(0.5f);
        targetPoke.PlayHitEffect();

        var damageDetails = targetPoke.Pokemon.TakeDamage(move, playerPoke.Pokemon);

        yield return targetPoke.BattleHUD.UpdateHP();

        yield return TypeDamageDetails(damageDetails, targetPoke.Pokemon.Base.Name);


        if (damageDetails.DidFaint)
        {
            yield return dialogueBox.TypeDialogue($"The wild {targetPoke.Pokemon.Base.Name} fainted.");
            targetPoke.PlayFaintAnimation();

            yield return new WaitForSeconds(1f);

            CheckForBattleOver(targetPoke);
            
        }
        
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
            StartCoroutine(PlayerMove());
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
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));

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
        bool stillEnemyMove = playerPoke.Pokemon.HP > 0;
        dialogueBox.HideActions(true);
        if (stillEnemyMove)
        {
            yield return dialogueBox.TypeDialogue(
                $"That's enough for now {playerPoke.Pokemon.Base.Name}, come back!");
            playerPoke.PlayFaintAnimation();
            yield return new WaitForSeconds(0.9f);
        }
        

        playerPoke.Setup(newPokemon);
        dialogueBox.SetMoveNames(newPokemon.Moves);
        yield return dialogueBox.TypeDialogue($"Go {newPokemon.Base.Name}!");

        if (stillEnemyMove)
            StartCoroutine(EnemyMove());
        else
            ActionSelection();
        dialogueBox.HideActions(false);
    }
}
