using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ChangeAudioMixer_Simple : MonoBehaviour
{
    public AudioSource footsteps;
    public AudioMixerGroup changetothis;
    public AudioClip[] footstepsArray;
    public GameObject Player;

    void OnTriggerEnter(Collider other)
    {
        New();
        if (other.tag == "Player")
        {
            Player.GetComponent<PBFootstepSystem>().footstepClips = footstepsArray;
        }
    }

    void New()
    {
        footsteps.outputAudioMixerGroup = changetothis;
    }
}
