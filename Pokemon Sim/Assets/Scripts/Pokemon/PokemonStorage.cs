using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonStorage : MonoBehaviour, ISavable
{
    const int BOX_POKE_LIMIT = 36;
    const int BOX_LIMIT = 10;
    Pokemon[,] boxes = new Pokemon[BOX_LIMIT, BOX_POKE_LIMIT];

    private void Awake()
    {
        
    }
    public void AddPokemon(Pokemon pokemon, int boxIndex, int slotIndex)
    {
        pokemon.Heal();
        boxes[boxIndex,slotIndex] = pokemon;
    }

    public void AddCaughtPokemon(Pokemon pokemon)
    {
        pokemon.Heal();
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

    public Pokemon TakePokemon(int boxIndex, int slotIndex)
    {
        Pokemon pokemon = boxes[boxIndex, slotIndex];
        boxes[boxIndex, slotIndex] = null;
        return pokemon;
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

    private void ClearBox()
    {
        for (int i = 0; i < BOX_LIMIT; i++)
        {
            for (int j = 0; j < BOX_POKE_LIMIT; j++)
            {
                boxes[i, j] = null;
            }
        }
    }

    public static PokemonStorage GetPlayerStorageBoxes()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonStorage>();
    }

    public object CaptureState()
    {
        var saveData = new BoxSaveData()
        {
            boxSlots = new List<BoxSlotSaveData>()
        };
        for (int i = 0; i < BOX_LIMIT; i++)
        {
            for (int j = 0; j < BOX_POKE_LIMIT; j++)
            {
                if (boxes[i, j] != null)
                {
                    saveData.boxSlots.Add(
                        new BoxSlotSaveData() { pokemonData = boxes[i, j].GetSaveData(), 
                            boxIndex = i, 
                            slotIndex = j 
                        });
                }
            }
        }
        return saveData;
    }

    public void RestoreState(object state)
    {
        var boxSaveData = state as BoxSaveData;

        // Clear box
        ClearBox();

        foreach(var slot in boxSaveData.boxSlots)
        {
            boxes[slot.boxIndex, slot.slotIndex] = new Pokemon(slot.pokemonData);
        }

    }
}

[System.Serializable]
public class BoxSaveData
{
    public List<BoxSlotSaveData> boxSlots;
}

[System.Serializable]
public class BoxSlotSaveData
{
    public PokemonSaveData pokemonData;
    public int boxIndex;
    public int slotIndex;

}
