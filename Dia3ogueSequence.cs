using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dia3ogueSequence : MonoBehaviour
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
            // That’s the tannoy from the pumping station. Finn said they mine gold for the Church’s basilicas, wonder what else in dusklight’s covered in gold… yeah. 
            protagSource.PlayOneShot(protagClip);
            yield return new WaitForSeconds(protagClip.length);
        }
    }
}
