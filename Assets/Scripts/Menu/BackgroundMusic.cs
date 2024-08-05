using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource AudioSource;

    private float musicVolume = 0.1f;

    private void Update()
    {
        AudioSource.volume = musicVolume;
    }

    public void UpdateVolume(float volume)
    {
        musicVolume = volume;
    }
}
