using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FootstepQuieter : MonoBehaviour
{
    public AudioMixerSnapshot Standard_Footsteps;
    public AudioMixerSnapshot Reduced_Footsteps;
    public float timeToReach;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Reduced_Footsteps.TransitionTo(timeToReach);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            Standard_Footsteps.TransitionTo(0.1f);
        }
    }
}
