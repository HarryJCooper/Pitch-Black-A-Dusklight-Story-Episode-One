using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOnEnvironmentEnter : MonoBehaviour

{
    public AudioSource _as;
    public AudioClip[] audioClipArray;
    public bool isPlaying;

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
            _as.Play();
        _as.volume = 0.04f;
        isPlaying = true;
    }

    void OnTriggerExit()
    {
        _as.volume = 0.01f;
    }

}