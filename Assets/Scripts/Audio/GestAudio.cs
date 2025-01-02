using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestAudio : MonoBehaviour
{
    public SOPerso playerData;
    public AudioSource audioSource;
    public AudioClip music;

    void Start()
    {
        audioSource.clip = music;
        audioSource.Play();
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (playerData.newGame)
        {
            audioSource.volume = 0.5f;
        }
        else
        {
            audioSource.volume = 0.7f;
        }
    }
}
