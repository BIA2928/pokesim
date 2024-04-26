using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeReference]
    [SerializeField] List<CutsceneAction> actions;

    public void AddAction(CutsceneAction action)
    {
        action.Name = action.GetType().ToString();
        actions.Add(action);
    }

    public IEnumerator PlayCutscene()
    {
        GameController.i.StartCutscene();
        foreach (var item in actions)
        {
            yield return item.Play();
        }
        GameController.i.EnterFreeRoam();
    }
}
