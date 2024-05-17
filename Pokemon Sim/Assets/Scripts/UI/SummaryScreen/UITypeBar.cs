using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITypeBar : MonoBehaviour
{
    [SerializeField] Image type1;
    [SerializeField] Image type2;

    public void SetImages(PokeType type1, PokeType type2)
    {
        var sprite1 = TypeImageDB.i.Lookup(type1);
        var sprite2 = TypeImageDB.i.Lookup(type2);
        this.type1.sprite = sprite1;
        this.type2.sprite = sprite2;
    }
}
