using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour, ISavable
{
    [SerializeField] ItemBase itemBase;
    [SerializeField] int count=1;
    [SerializeField] Dialogue dialogue;

    bool hasGiven = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogueManager.Instance.ShowDialogue(dialogue);
        player.GetComponent<Inventory>().AddItem(itemBase, count);
        hasGiven = true;

        yield return DialogueManager.Instance.ShowItemReceivedDialogue(itemBase, count);
    }

    public bool CanGive()
    {
        return itemBase != null && !hasGiven;
    }

    public object CaptureState()
    {
        return hasGiven;
    }

    public void RestoreState(object state)
    {
        hasGiven = (bool)state;
    }
}
