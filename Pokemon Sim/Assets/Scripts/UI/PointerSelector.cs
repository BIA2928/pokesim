using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointerSelector : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image heldItemImage;
    [SerializeField] Image pointerImage;
    [SerializeField] Sprite blankSprite;
    [SerializeField] Sprite pointingSprite;
    [SerializeField] Sprite holdingSprite;

    const float Y_OFFSET = 43f;
    public void Move(Transform target)
    {
        Vector3 newPosition = new Vector3(target.position.x, target.position.y + Y_OFFSET);
        transform.position = newPosition;
    }

    public void DropPokemon()
    {
        // Add dotween between sprites here
        // Pointerimage sprite should go from holding to empty

        pointerImage.sprite = pointingSprite;
        heldItemImage.sprite = blankSprite;
    }

    public void HoldPokemon(Pokemon pokemon)
    {
        Debug.Log("pokemon is null is " + pokemon == null);
        pointerImage.sprite = holdingSprite;
        heldItemImage.sprite = pokemon.Base.BoxSprite;
    }

}
