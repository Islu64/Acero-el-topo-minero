using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicoSound : MonoBehaviour
{
    public AudioClip picoSound; // Som da picareta
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPicoSound()
    {
        if (picoSound != null)
        {
            audioSource.PlayOneShot(picoSound);
        }
    }
}

