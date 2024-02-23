using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerPoke;
    [SerializeField] BattleHUD playerHUD;
    [SerializeField] BattleUnit enemyPoke;
    [SerializeField] BattleHUD enemyHUD;



    private void Start()
    {
        SetupBattle();
    }

    public void SetupBattle()
    {
        playerPoke.Setup();
        playerHUD.SetData(playerPoke.Pokemon);
        enemyPoke.Setup();
        enemyHUD.SetData(enemyPoke.Pokemon);
    }
}
