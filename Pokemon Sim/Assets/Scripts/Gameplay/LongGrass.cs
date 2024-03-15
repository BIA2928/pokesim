using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable 
{
    public void OnPlayerTrigger(PlayerController player)
    {
        if (Random.Range(1, 10) == 9)
        {
            //character.HandleUpdate();
            GameController.i.StartBattle();
        }
    }

     
    
}
