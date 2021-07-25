using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Footstep_Snapshot_Control : MonoBehaviour
{
    public AudioMixerSnapshot Standard_Footsteps;
    public AudioMixerSnapshot Reduced_Footsteps;
    public AudioMixerSnapshot Tiptoe_Footsteps;
    public float timeToReach;

    void OnTriggerEnter(Collider other)
    {
        New();
    }
    
    void New()
    {
        Standard_Footsteps.TransitionTo(0.1f);
        StartCoroutine(Wait());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Reduced_Footsteps.TransitionTo(timeToReach);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            Standard_Footsteps.TransitionTo(0.5f);
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        Reduced_Footsteps.TransitionTo(timeToReach);
    }
}
