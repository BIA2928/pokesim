using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    [SerializeField] Dialogue postHealDialogue;
    public IEnumerator Heal(Transform player, Dialogue dialogue)
    {
        int selectedChoice = 0;
        yield return DialogueManager.Instance.ShowDialogueChoices(dialogue, new List<string>() { "Yes", "No"}, (i) => selectedChoice = i, false);


        if (selectedChoice == 0)
        {
            yield return Fader.instance.FadeIn(0.4f);
            var party = player.GetComponent<PokemonParty>();
            party.PokemonList.ForEach(p => p.Heal());
            party.PartyUpdated();

            yield return Fader.instance.FadeOut(0.4f);

            if (postHealDialogue != null)
                yield return DialogueManager.Instance.ShowDialogue(postHealDialogue);
        }
        else 
        {
            yield return DialogueManager.Instance.ShowDialogue("Come back if you change your mind");
        }
        
    }
}
