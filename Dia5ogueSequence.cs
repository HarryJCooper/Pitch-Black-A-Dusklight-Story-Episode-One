using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dia5ogueSequence : MonoBehaviour
{
    [SerializeField] AudioSource protagSource;
    [SerializeField] AudioClip protagClip, windClip;
    [SerializeField] Controls controls;
    public int finished;

    void OnTriggerEnter(Collider other){ if (other.gameObject.name == "Player") StartCoroutine(Sequence());}

    IEnumerator Sequence(){
        if (finished == 0 && !protagSource.isPlaying && controls.inExplore){
            finished = 1;
            // Gush of wind triggers this dialogue. 
            protagSource.PlayOneShot(windClip);
            // Protag
            // Brrr, cold out here, 
            protagSource.PlayOneShot(protagClip);
            yield return new WaitForSeconds(protagClip.length);
        }
    }
}
