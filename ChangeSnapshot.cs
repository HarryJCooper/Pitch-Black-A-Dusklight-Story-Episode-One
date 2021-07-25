using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ChangeSnapshot : MonoBehaviour
{
    public AudioMixerSnapshot newSnapShot;

    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        newSnapShot.TransitionTo(0.4f);
    }

    void Update()
    {
        
    }
}
