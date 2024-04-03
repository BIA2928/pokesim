using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour, Interactive, ISavable
{
    [SerializeField] ItemBase item;

    public bool Used { get; set; } = false;

    

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);

            Used = true;

            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            Dialogue dialogue = new Dialogue();
            dialogue.Lines.Add($"You found a {item.Name}!");
            dialogue.Lines.Add($"{item.Name} was added to the inventory.");
            yield return DialogueManager.Instance.ShowDialogue(dialogue);
        }
        
    }

    public object CaptureState()
    {
        return Used;
    }
    public void RestoreState(object state)
    {
        bool used = (bool)state;
        Used = used;

        if (Used)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
