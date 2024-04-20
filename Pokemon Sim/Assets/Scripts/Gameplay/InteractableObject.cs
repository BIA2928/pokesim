using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class InteractableObject : MonoBehaviour, Interactive
{
    [SerializeField] MoveBase HMToInteract;
    public virtual IEnumerator Interact(Transform initiator)
    {
        var pokemonWithHM = initiator.GetComponent<PokemonParty>().
            PokemonList.FirstOrDefault(p => p.Moves.Any(m => m.Base == HMToInteract));
        if (pokemonWithHM != null)
        {
            int selectedChoice = 0;
            Dialogue d = new Dialogue() { Lines = { $"Would you like to use {HMToInteract.Name}?" }, };
            List<string> choices = new List<string>() { "Yes", "No" };
            yield return DialogueManager.Instance.ShowDialogueChoices(d, choices, (i) => selectedChoice = i, false);

            if (selectedChoice == 0)
            {
                yield return DialogueManager.Instance.ShowDialogue($"{pokemonWithHM.Base.Name} used {HMToInteract.Name}!");
                yield return ApplyHM();
            }
        }
    }

    public virtual IEnumerator ApplyHM()
    {
        var sprite = gameObject.GetComponent<SpriteRenderer>();
        AudioManager.i.PlaySFX(HMToInteract.Sound);
        yield return new WaitForSeconds(HMToInteract.Sound.length);
        yield return sprite.DOFade(0, 0.8f).WaitForCompletion();
        gameObject.SetActive(false);
        sprite.color = Color.white;
    }

}
