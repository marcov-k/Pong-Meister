using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioClip music;
    [SerializeField] float volume = 1;
    [SerializeField] float effectVolume = 1;
    AudioSource myMusicAudio;
    AudioSource myEffectAudio;

    void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        myMusicAudio = sources[0];
        myEffectAudio = sources[1];
    }

    void Start()
    {
        Initialize();
        PlayMusic();
    }

    void Update()
    {
        UpdateVolume();
    }

    void Initialize()
    {
        myMusicAudio.volume = volume * StaticSettings.masterVolume * StaticSettings.musicVolume;
        myEffectAudio.volume = volume * StaticSettings.masterVolume * StaticSettings.effectsVolume;
    }

    void UpdateVolume()
    {
        myMusicAudio.volume = volume * StaticSettings.masterVolume * StaticSettings.musicVolume;
    }

    void PlayMusic()
    {
        myMusicAudio.ignoreListenerPause = true;
        myMusicAudio.Pause();
        myMusicAudio.resource = music;
        myMusicAudio.volume = volume * StaticSettings.masterVolume * StaticSettings.musicVolume;
        myMusicAudio.Play();
    }

    public void PlayEffect(AudioClip effect, float extraEffectVolume)
    {
        myEffectAudio.volume = (effectVolume + extraEffectVolume) * StaticSettings.masterVolume * StaticSettings.effectsVolume;
        myEffectAudio.PlayOneShot(effect);
    }

    public void SetMusic(AudioClip newMusic, float newVolume)
    {
        music = newMusic;
        volume = newVolume;
        PlayMusic();
    }

    public void PauseMusic(bool pause)
    {
        if (pause)
        {
            myMusicAudio.ignoreListenerPause = false;
            myMusicAudio.Pause();
        }
        else
        {
            myMusicAudio.UnPause();
            myMusicAudio.ignoreListenerPause = true;
        }
    }
}
