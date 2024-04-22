using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{

    [SerializeField] List<PokemonEncounterData> wildPokemon;
    [SerializeField] List<PokemonEncounterData> waterWildPokemon;
    [HideInInspector][SerializeField] int totalChance;
    [HideInInspector] [SerializeField] int totalChanceWater;

    private void OnValidate()
    {
        CalculateEncounterRates();
    }
    private void Start()
    {
        CalculateEncounterRates();
    }

    void CalculateEncounterRates()
    {
        if (wildPokemon.Count == 0)
            totalChance = -1;
        else
        {
            totalChance = 0;
            foreach (var poke in wildPokemon)
            {
                poke.ChanceLower = totalChance;
                poke.ChanceUpper = totalChance + poke.encounterRate;

                totalChance += poke.encounterRate;
            }
        }

        if (waterWildPokemon.Count == 0)
            totalChanceWater = -1;
        else
        {
            totalChanceWater = 0;
            foreach (var poke in waterWildPokemon)
            {
                poke.ChanceLower = totalChanceWater;
                poke.ChanceUpper = totalChanceWater + poke.encounterRate;

                totalChanceWater += poke.encounterRate;
            }
        }
    }
    public Pokemon GetRandomWildPokemon(BattleEnvironment environment = BattleEnvironment.LongGrass)
    {
        PokemonEncounterData pokemonData;
        if (environment == BattleEnvironment.Water)
        {
            int randValue = Random.Range(1, totalChanceWater + 1);
            pokemonData = waterWildPokemon.First(p => randValue >= p.ChanceLower && randValue <= p.ChanceUpper);
        }
        else
        {
            int randValue = Random.Range(1, totalChance + 1);
            pokemonData = wildPokemon.First(p => randValue >= p.ChanceLower && randValue <= p.ChanceUpper);
        }
        
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