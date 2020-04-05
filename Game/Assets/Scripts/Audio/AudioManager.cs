using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    #region Properties

    public Sound[] Sounds;

    public static AudioManager Instance;

    #endregion

    #region MonoMethods

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);

            return;
        }

        DontDestroyOnLoad(this.gameObject);

        foreach (Sound sound in this.Sounds)
        {
            sound.Source = this.gameObject.AddComponent<AudioSource>();
            sound.Source.clip = sound.Clip;

            sound.Source.volume = sound.Volume;
            sound.Source.pitch = sound.Pitch;
            sound.Source.loop = sound.Loop;
        }
    }

    #endregion

    #region Methods

    public void Play(string name)
    {
        Sound sound = Array.Find(this.Sounds, s => s.Name == name);

        if (sound == null)
        {
            Debug.LogWarning($"Sound with name: {name} was not found!");

            return;
        }

        sound.Source.Play();
    }

    public void Stop(string name)
    {
        Sound sound = Array.Find(this.Sounds, s => s.Name == name);

        if (sound == null)
        {
            Debug.LogWarning($"Sound with name: {name} was not found!");

            return;
        }

        sound.Source.Stop();
    }

    public bool IsPlaying(string name)
    {
        Sound sound = Array.Find(this.Sounds, s => s.Name == name);

        if (sound == null)
        {
            Debug.LogWarning($"Sound with name: {name} was not found!");

            return false;
        }

        return sound.Source.isPlaying;
    }

    #endregion
}
