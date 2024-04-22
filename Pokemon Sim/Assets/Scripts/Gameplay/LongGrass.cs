using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LongGrass : MonoBehaviour, IPlayerTriggerable 
{
    /// <summary>
    /// Encouter rate out of 10. 10 representing 10 in 10 chance of encounter, 1 representing 1 in 10.
    /// </summary>
    [SerializeField] [Range(0, 10)] int encounterRate = 1;
    public void OnPlayerTrigger(PlayerController player)
    {
        if (Random.Range(1, 10) >= (10 - encounterRate))
        {
            player.Character.Animator.IsMoving = false;
            GameController.i.StartBattle(BattleEnvironment.LongGrass);
        }
    }

    public bool TriggerRepeatedly => true;

}
