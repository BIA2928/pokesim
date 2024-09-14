using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonStorage : MonoBehaviour
{
    const int BOX_POKE_LIMIT = 36;
    const int BOX_LIMIT = 10;
    Pokemon[,] boxes = new Pokemon[BOX_LIMIT, BOX_POKE_LIMIT];

    private void Awake()
    {
        
    }
    public void AddPokemon(Pokemon pokemon, int boxIndex, int slotIndex)
    {
        boxes[boxIndex,slotIndex] = pokemon;
    }

    public void AddCaughtPokemon(Pokemon pokemon)
    {
        for (int i = 0; i < BOX_LIMIT; i++)
        {
            for (int j = 0; j < BOX_POKE_LIMIT; j++)
            {
                if (boxes[i,j] == null)
                {
                    boxes[i, j] = pokemon;
                    return;
                }
            }
        } 
    }

    public void ReleasePokemon(int boxIndex, int slotIndex)
    {
        boxes[boxIndex, slotIndex] = null;
    }

    public void SwapPokemon(int boxIndex1, int slotIndex1, int boxIndex2, int slotIndex2)
    {
        Pokemon original = boxes[boxIndex1, slotIndex1];
        Pokemon swapper = boxes[boxIndex2, slotIndex2];
        boxes[boxIndex1, slotIndex1] = original;
        boxes[boxIndex2, slotIndex2] = swapper;
    }

    public Pokemon GetPokemon(int boxIndex, int slotIndex)
    {
        return boxes[boxIndex, slotIndex];
    }

    public List<Pokemon> GetBoxByIndex(int boxIndex)
    {
        if (boxIndex >= BOX_LIMIT)
        {
            Debug.LogError("Box index not valid.");
        }
        List<Pokemon> pokemonBox = new List<Pokemon>();

        for (int i = 0; i < BOX_POKE_LIMIT ; i++)
        {
            pokemonBox.Add(boxes[boxIndex, i]);
        }

        return pokemonBox;
    }

    public static PokemonStorage GetPlayerStorageBoxes()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonStorage>();
    }
}
