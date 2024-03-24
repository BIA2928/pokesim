using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{

    [SerializeField] List<Pokemon> mons;
    // Start is called before the first frame update

    public event Action OnPartyUpdate;

    public List<Pokemon> PokemonList
    {
        get { return mons;  }
        set { mons = value; }
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool AddPokemon(Pokemon pokemon)
    {
        if (mons.Count >= 6)
        {
            //AddPokemonToBox(pokemon);
            return false;
        }
        else
        {
            mons.Add(pokemon);
            OnPartyUpdate?.Invoke();
            return true;
        }
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }
}
