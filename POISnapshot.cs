using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class POISnapshot : MonoBehaviour
{
    public bool disableFocus;
    public AudioMixerSnapshot Standard;
    public AudioMixerSnapshot Focused;
    public float timeToReach;
    public float timeToReturn;

    void Start()
    {
        disableFocus = false;
    }

    void Update()
    {
        if (!disableFocus)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Focused.TransitionTo(timeToReach);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                Standard.TransitionTo(timeToReturn);
            }
        }
    }
}

