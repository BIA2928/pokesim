using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [SerializeField] List<string> lines;


    public List<string> Lines
    {
        get { return lines; }
        set { lines = value; }
    }

    public void AddLine(string line)
    {
        lines.Add(line);
    }

    public void AddLines(List<string> lines)
    {
        foreach (string line in lines)
        {
            lines.Add(line);
        }
    }
    public Dialogue()
    {
        lines = new List<string>();
    }
}
