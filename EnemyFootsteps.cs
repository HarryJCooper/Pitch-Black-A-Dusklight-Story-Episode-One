using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip[] footstepClips;
    float walkDist;
    [SerializeField] float maxDist;
    Vector3 recentPosition;

    void Start(){
        audioSource = this.GetComponent<AudioSource>();
        recentPosition = this.transform.position;
    }

    void Update(){
        walkDist = Vector3.Distance(recentPosition, this.transform.position);
        if (walkDist > maxDist){
            audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)], Random.Range(0.8f, 1f));
            walkDist = 0;
            recentPosition = this.transform.position;
        }
    }
}
