using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    private AudioSource Aud;
    public AudioClip[] Clips;

    public float PitchMax = 1.25f;
    public float PitchMin = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        Aud = GetComponent<AudioSource>();

        Aud.clip = Clips[Random.Range(0, Clips.Length)];

        Aud.pitch = Random.Range(PitchMin, PitchMax);

        Aud.Play();
    }
}
