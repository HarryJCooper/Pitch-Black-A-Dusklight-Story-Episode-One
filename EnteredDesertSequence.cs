using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnteredDesertSequence : MonoBehaviour
{
    [SerializeField] AudioController audioController;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] AudioClip[] playerFootstepClips;
    [SerializeField] AmbienceRepeater ambienceRepeater;
    [SerializeField] Controls controls;
    public int finished;
    
    void Start(){
        if (finished == 1) return;
        controls.canZoom = true;
        pBFootstepSystem.footstepClips = playerFootstepClips;
        StartCoroutine(ambienceRepeater.ambienceCoroutine);
        finished = 1;
    }
}
