using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFootsteps : MonoBehaviour
{
    [SerializeField] AudioClip[] playerFootstepClips;
    [SerializeField] PBFootstepSystem pBFootstepSystem;

    public void ChangePlayerFootsteps(){ pBFootstepSystem.footstepClips = playerFootstepClips;}
    void OnTriggerEnter(Collider other){ ChangePlayerFootsteps(); }
}
