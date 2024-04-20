using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{

    [SerializeField] List<PokemonEncounterData> wildPokemon;
    [HideInInspector][SerializeField] int totalChance;

    private void OnValidate()
    {
        totalChance = 0;
        foreach (var poke in wildPokemon)
        {
            poke.ChanceLower = totalChance;
            poke.ChanceUpper = totalChance + poke.encounterRate;

            totalChance += poke.encounterRate;
        }
    }
    private void Start()
    {
    }
    public Pokemon GetRandomWildPokemon()
    {
        int randValue = Random.Range(1, totalChance + 1);
        var pokemonData = wildPokemon.First(p => randValue >= p.ChanceLower && randValue <= p.ChanceUpper);
        var range = pokemonData.levelRange;
        int level = range.max == 0 ? range.min : Random.Range(range.min, range.max + 1);
        var wildPoke = new Pokemon(pokemonData.pokemon, level);
        return wildPoke;
    }
}


[System.Serializable]
public class PokemonEncounterData 
{
    public PokemonBase pokemon;
    public Range levelRange;
    public int encounterRate;

    public int ChanceLower { get; set; }
    public int ChanceUpper { get; set; }
}


[System.Serializable]
public class Range
{
    public int min;
    public int max;
}