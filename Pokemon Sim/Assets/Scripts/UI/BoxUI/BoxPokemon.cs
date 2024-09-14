using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxPokemon : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image pokeImage;
    [SerializeField] Image shadowImage;

    Pokemon pokemon;


    public void Init()
    {
        Clear();
    }

    public void PickupSelect()
    {
        // Selection nin this case mena sgetting picked up
        // Play SFX for clarity
        pokeImage.enabled = false;
    }


    public void Drop()
    {
        shadowImage.enabled = true;
        pokeImage.enabled = true;
    }

    public void SetData(Pokemon pokemon)
    { 
        this.pokemon = pokemon;
        shadowImage.enabled = false;
        if (pokemon == null)
        {
            pokeImage.enabled = false;
        }
        else
        {
            pokeImage.enabled = true;
            pokeImage.sprite = pokemon.Base.BoxSprite;
        }
        
        
    }

    public void Clear()
    {
        this.pokemon = null;
        shadowImage.enabled = false;
        pokeImage.enabled = false;
    }

    public void HoverSelect()
    {
        shadowImage.enabled = true;
    }

    public void HoverDeselect()
    {
        shadowImage.enabled = false;
    }

    public bool IsEmpty()
    {
        return pokemon == null;
    }

    public Pokemon GetPokemon()
    {
        return pokemon;
    }





}
