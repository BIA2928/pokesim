using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioData> commonAudios;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] AudioSource extraSoundsPlayer;
    [SerializeField][Range(0f,1.5f)] float fadeDuration = 0.8f;

    Dictionary<AudioID, AudioData> sfxLookup;

    public static AudioManager i { get; private set; }

    private void Awake()
    {
        if (i == null)
            i = this;
    }
    private void Start()
    {
        sfxLookup = commonAudios.ToDictionary(x => x.id);
    }
    public void PlaySFX(AudioClip effect)
    {
        if (effect == null) return;

        if (!sfxPlayer.isPlaying)
            sfxPlayer.PlayOneShot(effect);
    }
    public IEnumerator PlayMusicSFX(AudioClip effect)
    {
        float vol = musicPlayer.volume;
        musicPlayer.Pause();
        musicPlayer.volume = 0;
        extraSoundsPlayer.PlayOneShot(effect);
        yield return new WaitForSeconds(effect.length);
        musicPlayer.UnPause();
        yield return musicPlayer.DOFade(vol, fadeDuration).WaitForCompletion();
        // Pause current music and save where youre at in song
        // Play sfx with no fade in or out, WAIT UNTIL FINISHED AND LOCK INPUT IF POSSIBLE
        // Play previous song again with fade in 
    }

    public void PlaySFX(AudioID id)
    {
        AudioData audio;
        if (sfxLookup.ContainsKey(id))
            audio = sfxLookup[id];
        else
        {
            Debug.LogWarning($"Error - AudioID: {id} not found");
            return;
        }

        if (audio.stopsMusic)
            StartCoroutine(PlayMusicSFX(audio.clip));
        else
            PlaySFX(audio.clip);

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

    public AudioSource SFXPlayer => sfxPlayer;
    public AudioSource ExtraAudioPlayer => extraSoundsPlayer;
}

public enum AudioID
{
    UISelect, Hit, HitSprEft, HitNVEft, ExpGain, TMReceived, ItemReceived, KeyItemReceived, LvlUp, Bump, Jump, ObtainedPoke
}



[System.Serializable]
public class AudioData
{
    public AudioID id;
    public AudioClip clip;
    public bool stopsMusic;
}
