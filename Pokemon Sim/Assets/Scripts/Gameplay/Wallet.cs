using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    
    [SerializeField] int balance;
    public int Balance => balance;

    public event System.Action OnBalanceChange;

    public static Wallet i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    public bool CanAfford(int cost)
    {
        return cost <= balance;
    }

    public void AddMoney(int amount)
    {
        balance += amount;
        OnBalanceChange?.Invoke();
    }

    public void Pay(int amount)
    {
        balance -= amount;
        OnBalanceChange?.Invoke();
        if (balance < 0)
        {
            Debug.LogWarning("Negative balance error.");
        }
    }
}
