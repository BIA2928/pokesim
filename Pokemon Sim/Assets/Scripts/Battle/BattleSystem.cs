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
}
