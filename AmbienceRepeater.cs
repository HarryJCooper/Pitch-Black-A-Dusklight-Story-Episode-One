using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceRepeater : MonoBehaviour
{

    private AudioSource audioSource;
    public AudioClip[] ambience;
    public bool hasPlayedSound;
    private float waitTime;
    public float minWait = 30;
    public float maxWait = 60;
    public bool stopAmbience, dontPlayAtStart;

    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (!hasPlayedSound && !stopAmbience && !dontPlayAtStart)
        {
            PlaySound();
        }
    }

    void PlaySound()
    {
        audioSource.PlayOneShot(ambience[Random.Range(0, ambience.Length)]);
        hasPlayedSound = true;
        waitTime = Random.Range(minWait, maxWait);
        StartCoroutine(waitThenPlay());
    }

    IEnumerator waitThenPlay()
    {
        yield return new WaitForSeconds(waitTime);
        hasPlayedSound = false;
    }
}
