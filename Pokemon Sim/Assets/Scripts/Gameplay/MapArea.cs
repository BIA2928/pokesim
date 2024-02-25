using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{

    [SerializeField] List<Pokemon> wildPokemon;
    // Start is called before the first frame update
    public Pokemon GetRandomWildPokemon()
    {
        var wildPoke = wildPokemon[Random.Range(0, wildPokemon.Count)];
        wildPoke.Init();
        return wildPoke;
    }
}
