using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Items/Create new medicine item")]
public class MedicineItem : ItemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHP;

    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;

    [Header("Status Conditions")]
    [SerializeField] ConditionType status;
    [SerializeField] bool recoverAllStatuses;

    [Header("Revives")]
    [SerializeField] bool revive;
    [SerializeField] bool maxRevive;


}
