using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHUD battleHUD;
    [SerializeField] GameObject pokeballSprite;

    public BattleHUD BattleHUD
    {
        get { return battleHUD;  }
    }

    public Pokemon Pokemon { get; set; }
    public bool IsPlayerUnit { get { return isPlayerUnit; } }

    Image image;
    Vector3 originalPos;
    Vector3 origScale = new Vector3(1f, 1f, 1f);
    Color origColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = transform.localPosition;
        origColor = image.color;
    }

    public void EnableImage (bool enable)
    {
        image.enabled = enable;
    }

    public void Setup(Pokemon poke)
    {
        transform.localScale = origScale; 
        Pokemon = poke;
        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        } 
        else
        {
            image.sprite = Pokemon.Base.FrontSprite;
            ScalePokemon(Pokemon.Base.PokeSize);         
        }

        battleHUD.SetData(poke);
        battleHUD.gameObject.SetActive(true);
        EnableImage(false);
    }

    public void Clear()
    {
        battleHUD.gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets the pokeImage after the pokemon has fainted.
    /// </summary>
    public void ResetAfterFaint()
    {
        Debug.Log("Resetting image");
        image.transform.localPosition = originalPos;
        image.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// Scales the pokemon to ensure larger pokemon sprites actually appear larger than smaller counterparts
    /// </summary>
    /// <param name="size">The PokeSize enum of the qualitative size of the pokemon.</param>
    private void ScalePokemon(PokeSize size)
    {
        Transform rT = image.transform;
        switch (size)
        {
            case PokeSize.S:
                break;
            case PokeSize.M:
                rT.localScale = new Vector3(SizeScales.MediumScale, SizeScales.MediumScale, 1.0f);
                rT.position += new Vector3(0f, SizeScales.MediumYTranslation, 0f);
                break;
            case PokeSize.L:
                rT.localScale = new Vector3(SizeScales.LargeScale, SizeScales.LargeScale, 1.0f);
                rT.position += new Vector3(0f, SizeScales.LargeYTranslation, 0f);
                break;
            case PokeSize.XL:
                rT.localScale = new Vector3(SizeScales.XLScale, SizeScales.XLScale, 1.0f);
                rT.position += new Vector3(0f, SizeScales.XLYTranslation, 0f);
                break;
            default:
                break;
        }
    }

    // Animations here

    public void PlayAttackAnimation()
    {
        float animationDistance = 45f;
        // Animationtime = total time / 2
        float animationTime = 0.15f;
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + animationDistance, animationTime));
        } 
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - animationDistance, animationTime));
        }

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, animationTime));

    }

    

    public void PlayHitEffect()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.05f));
        sequence.Append(image.DOColor(origColor, 0.05f));
    }

    public IEnumerator PlayFaintAnimation() 
    {
        Vector3 startPos = transform.localPosition;
        if (Pokemon.Base.Cry != null)
        {
            if (Pokemon.HP <= 0)
            {
                yield return AudioManager.i.PlaySFXLowPitch(Pokemon.Base.Cry);
                AudioManager.i.PlaySFX(AudioID.PokemonReturn);
            }
            else
                // Eventually add retreat into pokeball noise
                yield return AudioManager.i.PlaySFXAsync(AudioID.PokemonReturn);
        }

            
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 120f, 0.3f));
        sequence.Join(image.DOFade(0f, 0.3f));
        yield return sequence.WaitForCompletion();
        EnableImage(false);
        ResetAfterFaint();
    }

    

    public void PlayEnterAnimation()
    {
        // Ensure in the correct position

        image.transform.localPosition = originalPos;
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        }
        else
        {
            image.transform.localPosition = new Vector3(500f, originalPos.y);
        }
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.8f));
        sequence.Join(image.DOFade(origColor.a, 0.1f));
        //yield return sequence.WaitForCompletion();
    }

    public IEnumerator PlayEnterAnimation2(bool isWild=false)
    {
        // Ensure in the correct position
        Vector3 buffer = new Vector3(0f, 1.5f, 0f);
        image.transform.localPosition = originalPos;
        
        if (isPlayerUnit)
        {
            
            var ballSequence = DOTween.Sequence();
            image.transform.localScale = Vector3.zero;
            EnableImage(true);

            // Have pokeball thrown in
            float rotationMagnitude = -1080f;
            var pokeballObj = Instantiate(pokeballSprite, image.transform.position - new Vector3(3.1f, -1f), Quaternion.identity);
            var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
            AudioManager.i.PlaySFX(AudioID.PokeballThrow);
            ballSequence.Append(pokeball.transform.DOJump(image.transform.position + buffer, 2f, 1, 1f));
            ballSequence.Join(pokeball.transform.DORotate(new Vector3(0, 0, rotationMagnitude), 1f, RotateMode.FastBeyond360));
            ballSequence.Append(image.transform.DOScale(Vector3.one, 0.1f));
            ballSequence.Join(pokeball.DOFade(0f, 0.05f));
            yield return ballSequence.WaitForCompletion();
            Destroy(pokeballObj);
            AudioManager.i.PlaySFX(AudioID.PokemonOut);
        }
        else
        {
            if (isWild)
            {
                var sequence = DOTween.Sequence();
                // Have pokemon fade in from the side
                image.transform.localPosition = new Vector3(500f, originalPos.y);
                EnableImage(true);
                sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.8f));
                sequence.Join(image.DOFade(origColor.a, 0.1f));
                yield return sequence.WaitForCompletion();
            }
            else
            {
                var ballSequence = DOTween.Sequence();
                image.transform.localScale = Vector3.zero;
                EnableImage(true);

                // Have pokeball thrown in
                float rotationMagnitude = 1080f;
                var pokeballObj = Instantiate(pokeballSprite, image.transform.position + new Vector3(3.75f, 1.75f), Quaternion.identity);
                var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
                AudioManager.i.PlaySFX(AudioID.PokeballThrow);
                ballSequence.Append(pokeball.transform.DOJump(image.transform.position + buffer, 2f, 1, 1f));
                ballSequence.Join(pokeball.transform.DORotate(new Vector3(0, 0, rotationMagnitude), 1f, RotateMode.FastBeyond360));
                ballSequence.Append(image.transform.DOScale(Vector3.one, 0.1f));
                ballSequence.Join(pokeball.DOFade(0f, 0.05f));
                yield return ballSequence.WaitForCompletion();
                Destroy(pokeballObj);
                AudioManager.i.PlaySFX(AudioID.PokemonOut);
            }    
        }

        if (Pokemon.Base.Cry != null)
        {
            yield return new WaitForSeconds(0.4f);
            AudioManager.i.PlaySFX(Pokemon.Base.Cry);
            yield return new WaitForSeconds(Pokemon.Base.Cry.length);
        }
        
    }


    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0f, 0.4f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 40f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.4f, 0.4f ,1f), 0.5f));

        yield return sequence.WaitForCompletion();

    }

    public IEnumerator PlayThrownOutAnimation()
    {
        yield break;
    }

    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1f, 0.3f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, 0.4f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.4f));

        yield return sequence.WaitForCompletion();

    }


}






static class SizeScales
{
    public const float MediumScale = 1.05f;
    public const float MediumYTranslation = 0.1f;
    public const float LargeScale = 1.14f;
    public const float LargeYTranslation = 0.3f;
    public const float XLScale = 1.2f;
    public const float XLYTranslation = 0.5f;
}



