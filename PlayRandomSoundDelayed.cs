using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRandomSoundDelayed : MonoBehaviour
{

    public AudioSource _as;
    public AudioClip[] audioClipArray;
    public float wait_time;
    public float wait_time_min;
    public float wait_time_max;
    void Awake()
    
    {
        _as = GetComponent<AudioSource>();
    }

    void Start()
    {
        _as.clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
        float wait_time = Random.Range(wait_time_min, wait_time_max);
        InvokeRepeating("Inst", wait_time_min, wait_time_max);
    }

    void Inst()
    {
        _as.PlayOneShot(_as.clip);
        _as.clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
        float wait_time = Random.Range(wait_time_min, wait_time_max);
    }
}
           