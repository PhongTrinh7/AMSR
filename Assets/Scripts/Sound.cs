using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0f, 2f)]
    public float volume;

    [Range(0f, 2f)]
    public float pitch;

    public AudioSource source;

    public bool loop;

    public float originalVolume;
}
