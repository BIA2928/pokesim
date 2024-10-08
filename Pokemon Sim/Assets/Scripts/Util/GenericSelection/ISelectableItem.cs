using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableItem 
{
    public void Init();
    public void SetSelected(bool selected);
    public void Clear();
}
