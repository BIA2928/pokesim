using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{

    [SerializeField] List<Pokemon> mons;

    public event Action OnPartyUpdate;

    public List<Pokemon> PokemonList
    {
        get { return mons;  }
        set 
        { 
            mons = value;
            OnPartyUpdate?.Invoke();
        }
    }
    void Awake()
    {
        foreach (var pokemon in mons)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetFirstHealthy()
    {
        return mons.Where(x => x.HP > 0).FirstOrDefault();
    }


    public bool AddPokemon(Pokemon pokemon)
    {
        if (mons.Count >= 6)
        {
            //AddPokemonToBox(pokemon);
            PokemonStorage.GetPlayerStorageBoxes().AddCaughtPokemon(pokemon);
            return false;
        }
        else
        {
            mons.Add(pokemon);
            OnPartyUpdate?.Invoke();
            return true;
        }
    }


    public void PartyUpdated()
    {
        OnPartyUpdate?.Invoke();
    }

    public bool CheckForEvolution()
    {
        return PokemonList.Any(p => p.CheckForEvolution() != null);
    }
    public IEnumerator RunEvolutions()
    {
        foreach(var pokemon in PokemonList)
        {
            var evo = pokemon.CheckForEvolution();
            if (evo != null)
            {
                float hpPercentage = ((float)pokemon.HP) / pokemon.MaxHP;
                yield return EvolutionManager.i.Evolve(pokemon, evo);
                pokemon.SetHpByPercentage(hpPercentage);
            }
        }
        // No need to handle updating party screen, that is done in gamecontroller

    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }
}
