using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] Dialogue openingDialogue;
    [SerializeField] Dialogue closingDialogue;
    public IEnumerator Trade()
    {
        yield return ShopController.instance.StartTrading(this);
    }

    public Dialogue OpeningDialogue => openingDialogue;
    public Dialogue ClosingDialogue => closingDialogue;
}
