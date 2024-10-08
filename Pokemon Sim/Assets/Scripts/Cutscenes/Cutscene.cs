using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cutscene : MonoBehaviour, IPlayerTriggerable
{
    [SerializeReference]
    [SerializeField] List<CutsceneAction> actions;

    public bool TriggerRepeatedly => false;

    public void AddAction(CutsceneAction action)
    {

#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this, "add action to cutscene");
#endif
        action.Name = action.GetType().ToString();
        actions.Add(action);
    }

    public void OnPlayerTrigger(PlayerController player)
    {
        Debug.Log("triggered CS");
        player.StopPlayerMovement();
        StartCoroutine(PlayCutscene());
    }

    public IEnumerator PlayCutscene()
    {
        GameController.i.StateMachine.Push(CutsceneState.i);
        foreach (var item in actions)
        {
            if (item.WaitToComplete)
                yield return item.Play();
            else
                StartCoroutine(item.Play());
        }
        GameController.i.StateMachine.Pop();
    }
}
