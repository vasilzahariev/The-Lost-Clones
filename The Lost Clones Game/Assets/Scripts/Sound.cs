﻿using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string Name;

    public AudioClip Clip;

    [Range(0f, 1f)]
    public float Volume;

    [Range(0.1f, 3f)]
    public float Pitch;

    public bool Loop;
    public bool PlayOverSame;

    [HideInInspector]
    public AudioSource Source;
}
