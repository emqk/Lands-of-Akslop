using UnityEngine;
using System.Collections.Generic;

public enum MusicType
{
    worldMusic, battleMusic
}

public class AudioManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource soundsSource;

    [Header("Music")]
    [SerializeField] List<AudioClip> worldMusic = new List<AudioClip>();
    [SerializeField] AudioClip battleMusic;

    [Header("Sounds")]
    [SerializeField] AudioClip buttonClick;
    [SerializeField] AudioClip takeDamage;

    public static AudioManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (!instance)
            instance = this;
    }

    private void Start()
    {
        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (!musicSource.isPlaying)
        {
            SetMusicToPlay(RandomizeWorldMusic());
            musicSource.Play();
        }
    }
    AudioClip RandomizeWorldMusic()
    {
        return worldMusic[Random.Range(0, worldMusic.Count)];
    }

    public void PlayMusic(MusicType musicType)
    {
        switch (musicType)
        {
            case MusicType.worldMusic:
                SetMusicToPlay(RandomizeWorldMusic());
                break;
            case MusicType.battleMusic:
                SetMusicToPlay(battleMusic);
                break;
            default:
                break;
        }
    }

    void SetMusicToPlay(AudioClip audioClip)
    {
        musicSource.clip = audioClip;
        musicSource.Play();
    }

    void PlaySound()
    {
        soundsSource.Play();
    }

    public void ClickButton()
    {
        soundsSource.clip = buttonClick;
        PlaySound();
    }
    
    public void TakeDamage()
    {
        soundsSource.clip = takeDamage;
        PlaySound();
    }
}
