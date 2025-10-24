using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static SoundEffectManager _instance;
    private static AudioSource _audioSource;
    private static SoundEffectLibrary _soundEffectLibrary;
    public Slider sfxSlider;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _audioSource = GetComponent<AudioSource>();
            _soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Play(string soundName)
    {
        Debug.Log(soundName);
        AudioClip audioClip = _soundEffectLibrary.GetRandomClip(soundName);
        if (audioClip != null)
        {
            _audioSource.PlayOneShot(audioClip);
        }
    }

    void Start()
    {

        sfxSlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 100);
        SetVolume(savedVolume);
        sfxSlider.value = savedVolume;
    }

    public static void SetVolume(float volume)
    {
        _audioSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void OnValueChanged()
    {
        SetVolume(sfxSlider.value);   
    }
}
