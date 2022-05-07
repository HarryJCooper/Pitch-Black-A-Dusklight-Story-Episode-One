using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dia6ogueSequence : MonoBehaviour
{
    [SerializeField] AudioSource protagSource;
    [SerializeField] AudioClip protagClip;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    public int finished;

    void OnTriggerEnter(Collider other){ if (other.gameObject.name == "Player") StartCoroutine(Sequence());}

    IEnumerator Sequence(){
        if (finished == 0 && !protagSource.isPlaying && controls.inExplore){
            finished = 1;
            // For if the player finds a small body of water, shallow. The dynamic terrain system hits. Protag remarks. 
            // Protag
            // Water. Second to the cure. People’d go weeks without water if it meant they could have the cure.
            protagSource.PlayOneShot(protagClip);
            yield return new WaitForSeconds(protagClip.length);
        }
    }
}
