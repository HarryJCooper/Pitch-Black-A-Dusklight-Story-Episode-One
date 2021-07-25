using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnTriggerEnter : MonoBehaviour
{
    public AudioClip audioClip;
    AudioSource audioSource;
    bool hasPlayed;
    public float delayPlayTime;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter()
    {
        if (!hasPlayed)
        {
            hasPlayed = true;
            audioSource.PlayDelayed(delayPlayTime);
        }
    }
}
