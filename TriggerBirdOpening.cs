using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBirdOpening : MonoBehaviour

{
    public AudioSource _as;
    public AudioClip[] audioClipArray;
    public bool isPlaying;
    public GameObject ToBeDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        _as.clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
    }

    void OnTriggerEnter()
    {
        _as.Play();
        _as.volume = 1;
        isPlaying = true;
    }

    void OnTriggerExit()
    {
        Destroy(ToBeDestroyed);
    }

}