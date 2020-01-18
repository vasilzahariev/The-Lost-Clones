using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    void Awake()
    {
        foreach (Sound sound in this.Sounds)
        {
            sound.Source = this.gameObject.AddComponent<AudioSource>();
            sound.Source.clip = sound.Clip;

            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;
            sound.Source.loop = sound.Loop;
        }
    }

    public void Play(string name)
    {
        Sound neededSound = Array.Find(this.Sounds, sound => sound.Name == name);

        if (neededSound == null || (neededSound.PlayOverSame == false && neededSound.Source.isPlaying == true))
        {
            return;
        }

        neededSound.Source.Play();
    }

    public void Stop(string name)
    {
        Sound neededSound = Array.Find(this.Sounds, sound => sound.Name == name);

        if (neededSound == null)
        {
            return;
        }

        neededSound.Source.Stop();
    }

    public void PlayAfter(string name, string afterName)
    {
        Sound neededSound = Array.Find(this.Sounds, sound => sound.Name == name);
        Sound afterSound = Array.Find(this.Sounds, sound => sound.Name == afterName);

        if (neededSound == null || afterSound == null || (neededSound.PlayOverSame == false && neededSound.Source.isPlaying == true))
        {
            return;
        }

        neededSound.Source.PlayDelayed(1f);
    }
}
