using System;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    private AudioSource _audioSource;

    public AudioClip backgroundMusic;

    public Slider musicSlider;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _audioSource =  GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic(false, backgroundMusic);
        }
        
        musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value);});
    }

    public static void SetVolume(float volume)
    {
        _instance._audioSource.volume = volume;
    }

    public static void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {
        if (audioClip != null)
        {
            _instance._audioSource.clip = audioClip;
        }
        
        if (_instance._audioSource.clip != null)
        {
            if (resetSong)
            {
                _instance._audioSource.Stop();
            }
            _instance._audioSource.Play();
        }
    }

    public static void PauseBackgroundMusic()
    {
        _instance._audioSource.Pause();
    }
}
