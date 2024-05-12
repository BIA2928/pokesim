using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Utils.StateMachine;

public class EvolutionManager : State<GameController>
{
    [SerializeField] GameObject evoUI;
    [SerializeField] Image oldPokemonImage;
    [SerializeField] Image newPokemonImage;
    [SerializeField] Image backgroundImage;
    [SerializeField] AudioClip evolutionMusic;

    public static EvolutionManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution) 
    {
        GameController.i.StateMachine.Push(this);
        evoUI.SetActive(true);
        AudioManager.i.PlayMusic(evolutionMusic);

        //Get OldPokemon Data
        oldPokemonImage.sprite = pokemon.Base.FrontSprite;
        string prevName = pokemon.Base.Name;
        // Get new pokemon data, evolve
        pokemon.Evolve(evolution);
        newPokemonImage.sprite = pokemon.Base.FrontSprite;
        string newName = pokemon.Base.Name;

        // Play start of routine
        yield return DialogueManager.Instance.ShowPreEvolutionDialogue(prevName);

        // Play animation
        yield return HandleEvolutionAnimation();

        // Play end of routine
        AudioManager.i.PlaySFX(AudioID.Congratulations);
        yield return DialogueManager.Instance.ShowPostEvolutionDialogue(prevName, pokemon.Base.Name);

        // Turn off 
        ResetImages();
        evoUI.SetActive(false);
        GameController.i.StateMachine.Pop();
    }

    public override void ExitState()
    {
        GameController.i.PartyScreen.SetPartyData();
        AudioManager.i.PlayMusic(GameController.i.CurrentScene.SceneMusic);
    }

    public void ResetImages()
    {
        oldPokemonImage.color = Color.white;
        oldPokemonImage.rectTransform.localScale = Vector3.one;
        newPokemonImage.rectTransform.localScale = Vector3.one;
        newPokemonImage.color = new Color(1, 1, 1, 0);
    }

    public IEnumerator HandleEvolutionAnimation()
    {
        // Flash background twice super fast
        var flashSequence = DOTween.Sequence();
        flashSequence.Append(backgroundImage.DOFade(0, 0.05f));
        flashSequence.Append(backgroundImage.DOFade(1f, 0.05f));
        flashSequence.Append(backgroundImage.DOFade(0, 0.05f));
        flashSequence.Append(backgroundImage.DOFade(1f, 0.05f));
        // Make old sprite fade to black
        flashSequence.Append(oldPokemonImage.DOColor(Color.black, 0.25f));
        yield return flashSequence.WaitForCompletion();

        // Make old sprite fade to black


        // Repeat the following a few times
        // Bring old sprite alpha to zero
        // As that's going, make new sprite alpha = 255
        // Do the opposite
        int j = 1;
        var morphSequence = DOTween.Sequence();
        for (int i = 0; i < 11; i++)
        {
            
            if (i % 2 == 0)
            {
                // Shrink and fade old sprite, then grow and unfade new sprite
                morphSequence.Append(oldPokemonImage.DOFade(0f, 1f / j));
                morphSequence.Join(oldPokemonImage.transform.DOScale(Vector3.zero, 1f / j));
                morphSequence.Append(newPokemonImage.DOFade(1f, 1f / j));
                morphSequence.Join(newPokemonImage.transform.DOScale(Vector3.one, 1f / j));
            }
            else
            {
                // Shrink and fade new sprite, then grow and unfade old sprite
                morphSequence.Append(newPokemonImage.DOFade(0f, 1f / j));
                morphSequence.Join(newPokemonImage.transform.DOScale(Vector3.zero, 1f / j));
                morphSequence.Append(oldPokemonImage.DOFade(1f, 1f / j));
                morphSequence.Join(oldPokemonImage.transform.DOScale(Vector3.one, 1f / j));
                j++;
            }
            
        }

        yield return morphSequence.WaitForCompletion();

        // Fade new sprite to 0 alpha
        // Shrink bg to 0 scale x and y
        // Bring them both back to normal in a flash together

        var finalSequence = DOTween.Sequence();
        finalSequence.Append(backgroundImage.transform.DOScale(Vector3.zero, 0.5f));
        finalSequence.Join(newPokemonImage.transform.DOScale(Vector3.zero, 0.5f));
        finalSequence.Append(backgroundImage.transform.DOScale(Vector3.one, 0.5f));
        finalSequence.Join(newPokemonImage.transform.DOScale(Vector3.one, 0.5f));
        finalSequence.Join(newPokemonImage.DOColor(Color.white, 0.1f));
        yield return finalSequence.WaitForCompletion();

    }
}
