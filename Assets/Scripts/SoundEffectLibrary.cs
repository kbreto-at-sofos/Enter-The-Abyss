using System.Collections.Generic;
using UnityEngine;

public class SoundEffectLibrary : MonoBehaviour
{
    public SoundEffectGroup[] soundEffectGroups;
    private Dictionary<string, List<AudioClip>> _soundDirectory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        _soundDirectory = new Dictionary<string, List<AudioClip>>();
        foreach (var soundEffectGroup in soundEffectGroups)
        {
            _soundDirectory[soundEffectGroup.name] = soundEffectGroup.audioClips;
        }
    }

    public AudioClip GetRandomClip(string name)
    {
        if (_soundDirectory.ContainsKey(name))
        {
            List<AudioClip> clips = _soundDirectory[name];
            if (clips.Count > 0)
            {
                return clips[Random.Range(0, clips.Count)];
            }
        }

        return null;
    }
}

// to see in the inspector
[System.Serializable]
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClips;
}
