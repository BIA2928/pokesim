using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField][Range(0f,1.5f)] float fadeDuration = 0.8f;

    public static AudioManager i { get; private set; }

    private void Awake()
    {
        if (i == null)
            i = this;
    }
    public void PlayMusic(AudioClip clip, bool loop=true, bool fade=true)
    {
        if (clip != null)
        {
            StartCoroutine(PlayMusicAsync(clip, loop, fade));
        }
        
    }

    IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade)
    {
        float prevVolume = musicPlayer.volume;
        if (fade)
        {
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();
        }
        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();
        if (fade)
            yield return musicPlayer.DOFade(prevVolume, fadeDuration).WaitForCompletion();
    }

    public void PauseMusic()
    {
        musicPlayer.Pause();
    }

    public void UnpauseMusic()
    {
        musicPlayer.UnPause();
    }
}
