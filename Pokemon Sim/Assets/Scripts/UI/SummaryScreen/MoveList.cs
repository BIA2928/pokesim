using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveList : MonoBehaviour
{
    [SerializeField] List<MoveSlot> moves;

    public List<MoveSlot> MoveSlots => moves;
}
