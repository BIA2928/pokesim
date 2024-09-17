using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PointerSelector : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image heldItemImage;
    [SerializeField] Image pointerImage;
    [SerializeField] Sprite blankSprite;
    [SerializeField] Sprite pointingSprite;
    [SerializeField] Sprite droppingSprite;
    [SerializeField] Sprite holdingSprite;

    [SerializeField] BoxUI boxUI;

    Vector3 origHeldPokemonPos;

    private void Start()
    {
        origHeldPokemonPos = heldItemImage.transform.localPosition;
    }
    const float Y_OFFSET = 43f;
    public void Move(Transform target)
    {
        Vector3 newPosition = new Vector3(target.position.x, target.position.y + Y_OFFSET);
        transform.DOMove(newPosition, 0.1f);
        //transform.position = newPosition;
    }

    public IEnumerator DropPokemon()
    {
        Vector3 hpp = heldItemImage.transform.position;
        var sequence = DOTween.Sequence();
        // Make the pointer look as if it is dropping
        pointerImage.sprite = droppingSprite;
        // Make the pokemon fall
        sequence.Append(heldItemImage.transform.DOMoveY(-Y_OFFSET, 0.15f));
        // Fill the box spot
        boxUI.FillCurrentSpot();
        // Hide held item
        heldItemImage.sprite = blankSprite;
        // Move held item back 
        heldItemImage.transform.position = hpp;
        yield return sequence.WaitForCompletion();
        AudioManager.i.PlaySFX(AudioID.ChangePocket);
        pointerImage.sprite = pointingSprite;
    }

    public void DropAndGrabPokemon(Pokemon grabbedPokemon)
    {

    }

    public IEnumerator HoldPokemon(Pokemon pokemon)
    {
        // Place pokemon to hold underneath pointer and then make it visible
        heldItemImage.transform.position = new Vector3(transform.position.x, transform.position.y - Y_OFFSET);
        heldItemImage.sprite = pokemon.Base.BoxSprite;
        // Move it up to pointer and change pointer to grabbing
        boxUI.EmptyCurrentSpot();
        yield return heldItemImage.transform.DOLocalMove(origHeldPokemonPos, 0.21f).WaitForCompletion();
        
        pointerImage.sprite = holdingSprite;
    }

}
