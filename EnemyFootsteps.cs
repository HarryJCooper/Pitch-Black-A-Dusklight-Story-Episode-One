using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DearVR;

public class EnemyFootsteps : MonoBehaviour
{
    AudioSource audioSource;
    DearVRSource audioVRSource;
    public AudioClip[] footstepClips;
    float walkDist, newMaxDist;
    public float maxDist;
    Vector3 recentPosition;
    int footstepInt;
    public bool isLimping;
    [SerializeField] float stopFootstepDist = 4;
    bool limp;

    void Start(){
        recentPosition = this.transform.position;
        newMaxDist = maxDist*Random.Range(0.96f, 1.04f);
        audioSource = this.GetComponent<AudioSource>();
        audioVRSource = audioSource.GetComponent<DearVRSource>();
        audioVRSource.performanceMode = true;
    }

    int RandomNumberGen(){
        int randomInt = Random.Range(0, footstepClips.Length);
        if (randomInt == footstepInt) randomInt = RandomNumberGen();
        return randomInt;
    }

    void Update(){
        walkDist = Vector3.Distance(recentPosition, this.transform.position);
        if (walkDist > stopFootstepDist){
            walkDist = 0;
            recentPosition = this.transform.position;
            newMaxDist = maxDist*Random.Range(0.9f, 1.1f);
        } 
        if (walkDist > newMaxDist && walkDist < stopFootstepDist){
            footstepInt = RandomNumberGen(); 
            float footstepVolFloat = Random.Range(0.8f, 1f);
            audioSource.volume = footstepVolFloat;
            audioVRSource.DearVRPlayOneShot(footstepClips[footstepInt]);
            walkDist = 0;
            recentPosition = this.transform.position;
            if (isLimping){
                limp = !limp;
                if (limp){
                    newMaxDist = maxDist*Random.Range(0.6f, 0.7f);
                } else {
                    newMaxDist = maxDist*Random.Range(1.2f, 1.3f);
                }
                return;
            }
            newMaxDist = maxDist*Random.Range(0.9f, 1.1f);
        }
    }
}
