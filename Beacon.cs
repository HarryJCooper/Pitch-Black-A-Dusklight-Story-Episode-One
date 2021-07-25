using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    bool hasPlayed;
    public bool turnedOn;
    AudioSource audioSource;
    public AudioClip beacon;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (turnedOn)
        {
            if (!hasPlayed)
            {
                hasPlayed = true;
                StartCoroutine(RepeatBeacon());
            }
        }   
    }

    IEnumerator RepeatBeacon()
    {
        yield return new WaitForSeconds(2f);
        audioSource.PlayOneShot(beacon);
        hasPlayed = false;
    }

}
