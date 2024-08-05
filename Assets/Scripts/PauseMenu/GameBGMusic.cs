using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBGMusic : MonoBehaviour
{
    public AudioSource gameBGMusic;
    public AudioSource turnbasedBGMusic;


    private float gameMusic = 0.1f;

    private void Update()
    {
        gameBGMusic.volume = gameMusic;
        turnbasedBGMusic.volume = gameMusic;
    }

    public void UpdateGameVolume(float volume)
    {
        gameMusic = volume;
    }
}
