using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dia2ogueSequence : MonoBehaviour
{
    [SerializeField] AudioSource protagSource;
    [SerializeField] AudioClip protagClip;
    [SerializeField] Controls controls;
    public int finished;

    void OnTriggerEnter(Collider other){ if (other.gameObject.name == "Player") StartCoroutine(Sequence());}

    IEnumerator Sequence(){
        if (finished == 0 && !protagSource.isPlaying && controls.inExplore){
            finished = 1;
            // Protag
            // Peaceful night. Brother would’ve loved this place.
            protagSource.PlayOneShot(protagClip);
            yield return new WaitForSeconds(protagClip.length);
        }
    }
}
