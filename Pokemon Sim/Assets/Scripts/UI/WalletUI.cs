using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class WalletUI : MonoBehaviour
{
    [SerializeField] Text balanceText;

    private void Start()
    {
        Wallet.i.OnBalanceChange += SetBalance;
    }
    void SetBalance()
    {
        var culture = new CultureInfo("en-US");
        var balanceStringFormatted = Wallet.i.Balance.ToString("c0", culture);

        balanceText.text = balanceStringFormatted;
    }

    public void ShowBalance()
    {
        gameObject.SetActive(true);
        SetBalance();
    }

    public void CloseWallet()
    {
        gameObject.SetActive(false);
    }
}
