using UnityEngine.Audio;
using UnityEngine;
using System;

/// <summary>
/// Controls the SFX, Music and Voice
/// </summary>

public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Every public variable
    /// </summary>
    #region Properties

    public static AudioManager Instance; // Contains the instance of the current audio manager as there cannot be two of them

    public Sound[] Sounds; // An array of sounds that could be played

    #endregion

    /// <summary>
    /// Every method that is in the MonoBehavior class
    /// </summary>
    #region MonoMethods

    void Awake()
    {
        this.InstanceChecker();
    }

    #endregion

    /// <summary>
    /// All methods
    /// </summary>
    #region Methods

    /// <summary>
    /// This method checks if there's an already existing instance of the audio manager.
    /// If there's an instance from a previous scene, it deletes the current one.
    /// If there's no instance from a previous scene, it sets this one as the instance, makes this object undestructable when
    /// loading a new scene and fills it with all the sounds that are given in the Unity Engine.
    /// </summary>
    private void InstanceChecker()
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

    /// <summary>
    /// This Method is called when you want to play a sound.
    /// If there's no song with the given name, the manager puts out a warning in the Debug Console.
    /// If a sound with the give name is found, the manager plays it.
    /// </summary>
    /// <param name="name">The name of the sound</param>
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


    /// <summary>
    /// This method is called when you want to stop a sound.
    /// If there's no song with the given name, the manager puts out a warning in the Debug Console.
    /// If a sound with the give name is found, the manager stops it.
    /// </summary>
    /// <param name="name">The name of the sound</param>
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

    /// <summary>
    /// This method is called when you want to check if a song is currently playing
    /// If there's no song with the given name, the manager puts out a warning in the Debug Console.
    /// If a sound with the give name is found, the method returns whether or not it's playing.
    /// </summary>
    /// <param name="name">The name of the sound</param>
    /// <returns>Whether or not the sound is playing</returns>
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
