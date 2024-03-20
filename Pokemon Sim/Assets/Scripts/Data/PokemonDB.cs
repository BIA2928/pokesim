using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonDB 
{
    static Dictionary<string, PokemonBase> pokemonDict;

    public static void Init()
    {
        pokemonDict = new Dictionary<string, PokemonBase>();
        var pokeArray = Resources.LoadAll<PokemonBase>("");
        foreach (var pokemon in pokeArray)
        {
            if (pokemonDict.ContainsKey(pokemon.Name))
            {
                Debug.Log($"Two instances of {pokemon.Name}");
                continue;
            }
                
            pokemonDict[pokemon.Name] = pokemon;
        }
    }

    public static PokemonBase LookUpByName(string name) 
    { 
        if (!pokemonDict.ContainsKey(name))
        {
            Debug.Log($"Pokemon with name {name} does not exist in the database");
            return null;
        }
        return pokemonDict[name];
    }
}
