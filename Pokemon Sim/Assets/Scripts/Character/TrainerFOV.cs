using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTrigger(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        GameController.i.OnEnterTrainerFOV(GetComponentInParent<TrainerController>());
    }

    public bool TriggerRepeatedly => false;

    
}
