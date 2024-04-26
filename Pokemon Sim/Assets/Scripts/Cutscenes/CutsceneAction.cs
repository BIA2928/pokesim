using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutsceneAction 
{
    [SerializeField] string name;
    [SerializeField] bool waitToComplete = true;

    public virtual IEnumerator Play()
    {
        yield break;
    }
    public string Name { get => name; set => name = value; }
    public bool WaitToComplete => waitToComplete;
}
