using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public float Volume;
    public bool alreadyPlayed = false;
    public AudioSource _as;
    public AudioClip[] audioClipArray;

    void Awake()
    {
        _as = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _as.clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
    }

    void OnTriggerEnter()
    {
        if (!alreadyPlayed)
        {
            _as.PlayOneShot(audioClipArray[Random.Range(0, audioClipArray.Length)], Volume);
            alreadyPlayed = true;
        }
    }
}