using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonGiver : MonoBehaviour, ISavable
{
    [SerializeField] Pokemon pokemon;
    [SerializeField] Dialogue dialogue;

    bool hasGiven = false;

    public IEnumerator GivePokemon(PlayerController player)
    {
        yield return DialogueManager.Instance.ShowDialogue(dialogue);
         
        pokemon.Init();
        bool partyFull = player.GetComponent<PokemonParty>().AddPokemon(pokemon);

        hasGiven = true;
        yield return DialogueManager.Instance.ShowPokemonReceivedDialogue(pokemon.Base, partyFull);
    }

    public bool CanGive()
    {
        return pokemon != null && !hasGiven;
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
