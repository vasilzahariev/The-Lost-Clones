using UnityEngine.Audio;
using UnityEngine;

/// <summary>
/// This class represents a Sound
/// </summary>
[System.Serializable]
public class Sound
{
    public string Name; // The name of the sound

    public SoundType Type; // What type of sound it is

    public AudioClip Clip; // The sound itself

    [Range(0f, 1f)]
    public float Volume; // What volume it'll have

    [Range(0.1f, 3f)]
    public float Pitch; // Wath pitch it'll have

    public bool Loop; // Should it loop or not

    [HideInInspector]
    public AudioSource Source; // ...
}
