using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable 
{
    public void OnPlayerTrigger(PlayerController player)
    {
        if (Random.Range(1, 10) == 9)
        {
            player.Character.Animator.IsMoving = false;
            GameController.i.StartBattle();
        }
    }

    public bool TriggerRepeatedly => true;

}
