using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDelayed : MonoBehaviour
{
    
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayDelayed(61.0f);       
    }
}
