using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDB<T> : MonoBehaviour where T: ScriptableObject
{
    static Dictionary<string, T> objs;

    public static void Init()
    {
        objs = new Dictionary<string, T>();
        var objArray = Resources.LoadAll<T>("");
        foreach (var obj in objArray)
        {
            if (objs.ContainsKey(obj.name))
            {
                Debug.LogError($"Duplicate in db: {obj.name}");
                continue;
            }
            objs[obj.name] = obj;
        }
    }

    public static T LookupByName(string name)
    {
        if (!objs.ContainsKey(name))
        {
            Debug.LogError($"Object with name {name} not found in database");
            return null;
        }
        return objs[name];
    }
}
