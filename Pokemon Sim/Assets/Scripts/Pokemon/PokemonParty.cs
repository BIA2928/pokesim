using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{

    [SerializeField] List<Pokemon> mons;
    // Start is called before the first frame update
    void Start()
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
}
