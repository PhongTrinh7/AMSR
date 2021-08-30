using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : Manager<AudioManager>
{
    public Sound[] sounds;
    public DialogueBox db;

    public override void Awake()
    {
        base.Awake();
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.originalVolume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public AudioSource Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Play();
            return s.source;
        } 
        else
        {
            return null;
        }
    }

    public AudioSource PlayOneShot(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.PlayOneShot(s.source.clip);
            return s.source;
        }
        else
        {
            return null;
        }
    }

    public void PlayDelayed(string name, float delay)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        s.source.PlayDelayed(delay);

        //yield return null;
    }

    public void Stop(string name, float time)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            StartCoroutine(FadeOut(s.source, time));
        }
    }

    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public void ChangeVolume(float n)
    {
        foreach (Sound s in AudioManager.Instance.sounds)
        {
            s.source.volume = s.originalVolume * n;
        }

        db.audioSource.volume = n * .1f;
    }
}
