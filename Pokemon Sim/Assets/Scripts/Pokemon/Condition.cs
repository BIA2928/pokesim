using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Condition
{
    public string Name { get; set; }
    public string Description { get; set; }

    public string StartMessage { get; set; }

    public Func<Pokemon, bool> OnStartOfTurn { get; set; }

    public Action<Pokemon> OnEndOfTurn { get; set; }

    public Action<Pokemon> OnCndStart { get; set; }

    public ConditionType CndType { get; set; }
    

}
