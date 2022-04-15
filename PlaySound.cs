using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public bool hasTriggered;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;

    void OnTriggerEnter(Collider other){
        if (hasTriggered || other.gameObject.name != "Player") return;
        audioSource.PlayOneShot(audioClip);
        hasTriggered = true;
    }
}
