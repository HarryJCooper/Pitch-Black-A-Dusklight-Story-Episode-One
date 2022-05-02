using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFootsteps : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] footstepClips;
    float walkDist, newMaxDist;
    [SerializeField] float maxDist;
    Vector3 recentPosition;
    int footstepInt;

    void Start(){
        audioSource = this.GetComponent<AudioSource>();
        recentPosition = this.transform.position;
        newMaxDist = maxDist*Random.Range(0.96f, 1.04f);
    }

    int RandomNumberGen(){
        int randomInt = Random.Range(0, footstepClips.Length);
        if (randomInt == footstepInt) randomInt = RandomNumberGen();
        return randomInt;
    }

    void Update(){
        walkDist = Vector3.Distance(recentPosition, this.transform.position);
        if (walkDist > 4){
            walkDist = 0;
            recentPosition = this.transform.position;
            newMaxDist = maxDist*Random.Range(0.9f, 1.1f);
        } 
        if (walkDist > newMaxDist && walkDist < 4){
            footstepInt = RandomNumberGen(); 
            audioSource.PlayOneShot(footstepClips[footstepInt], Random.Range(0.8f, 1f));
            walkDist = 0;
            recentPosition = this.transform.position;
            newMaxDist = maxDist*Random.Range(0.9f, 1.1f);
        }
    }
}
