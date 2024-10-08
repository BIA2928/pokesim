using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioData> commonAudios;
    [SerializeField] AudioClip surfMusic;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] AudioSource extraSoundsPlayer;
    [SerializeField][Range(0f,1.5f)] float fadeDuration = 0.8f;

    AudioClip musicBeforeSurfing;
    AudioClip musicBeforeBattle;
    Dictionary<AudioID, AudioData> sfxLookup;

    public static AudioManager i { get; private set; }

    float originalPitch;
    private void Awake()
    {
        if (i == null)
            i = this;
    }
    private void Start()
    {
        sfxLookup = commonAudios.ToDictionary(x => x.id);
        originalPitch = sfxPlayer.pitch;
    }
    public void PlaySFX(AudioClip effect)
    {
        if (effect == null) return;


        if (!sfxPlayer.isPlaying || sfxPlayer.clip != effect)
        {
            sfxPlayer.clip = effect;
            sfxPlayer.PlayOneShot(effect);
        }

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

    public IEnumerator PlaySFXAsync(AudioID id)
    {
        AudioData audio;
        if (sfxLookup.ContainsKey(id))
            audio = sfxLookup[id];
        else
        {
            Debug.LogWarning($"Error - AudioID: {id} not found");
            yield break;
        }

        if (audio.stopsMusic)
            yield return PlayMusicSFX(audio.clip);
        else
        {

            if (!sfxPlayer.isPlaying || sfxPlayer.clip != audio.clip)
            {
                sfxPlayer.clip = audio.clip;
                sfxPlayer.PlayOneShot(audio.clip);
            }
            yield return new WaitUntil(() => sfxPlayer.isPlaying == false);
        }
    }

    public IEnumerator PlaySFXLowPitch(AudioClip clip)
    {
        if (clip == null) yield break;

        if (!sfxPlayer.isPlaying || sfxPlayer.clip != clip)
        {
            sfxPlayer.clip = clip;
            sfxPlayer.pitch = sfxPlayer.pitch * 0.8f;
            SFXPlayer.PlayOneShot(clip);
        }
        yield return new WaitUntil(() => SFXPlayer.isPlaying == false);
        sfxPlayer.pitch = originalPitch;
    }

    

    public void PlayMusic(AudioClip clip, bool loop=true, bool fade=true)
    {
        if (clip != null && (clip != musicPlayer.clip || musicPlayer.isPlaying == false))
        {
            StartCoroutine(PlayMusicAsync(clip, loop, fade));
        }
        
    }

    public void PlayBattleMusic(AudioClip clip, bool loop=true, bool fade = true)
    {
        if (clip != null)
        {
            musicBeforeBattle = musicPlayer.clip;
            StartCoroutine(PlayMusicAsync(clip, loop, fade));
        }
    }

    public void StopBattleMusic(bool fade = true)
    {
        if (!PlayerController.i.IsSurfing)
            StartCoroutine(PlayMusicAsync(GameController.i.CurrentScene.SceneMusic, true, fade));
        else
            StartCoroutine(PlayMusicAsync(surfMusic, true, fade));
        musicBeforeBattle = null;
    }

    public void PlaySurfMusic()
    {
        musicBeforeSurfing = musicPlayer.clip;
        StartCoroutine(PlayMusicAsync(surfMusic, true, true));
    }

    public void StopSurfMusic()
    {
        StartCoroutine(PlayMusicAsync(musicBeforeSurfing, true, true));
        musicBeforeSurfing = null;
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

    public AudioClip GetAudioClip(AudioID audioID)
    {
        if (sfxLookup.ContainsKey(audioID))
            return sfxLookup[audioID].clip;
        return null;
    }

    public AudioSource SFXPlayer => sfxPlayer;
    public AudioSource ExtraAudioPlayer => extraSoundsPlayer;
}

public enum AudioID
{
    UISelect, UISwitchSelection, MenuOpen, MenuClose, Hit, HitSprEft, HitNVEft, ExpGain, TMReceived, 
    ItemReceived, KeyItemReceived, LvlUp, Bump, Jump, ObtainedPoke, StatUp, StatDown,
    PokemonOut, PokeballThrow, ChangePocket, EnterArea, PokeballShake, PokeballClick, PokeballBounce,
    Congratulations, PokemonReturn
}



[System.Serializable]
public class AudioData
{
    public AudioID id;
    public AudioClip clip;
    public bool stopsMusic;
}
