using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dia4ogueSequence : MonoBehaviour
{
    [SerializeField] AudioSource protagSource;
    [SerializeField] AudioClip protagClip;
    [SerializeField] Controls controls;
    public int finished;
    [SerializeField] SaveAndLoadPumpingStation saveAndLoadPumpingStation;

    void OnTriggerEnter(Collider other){ if (other.gameObject.name == "Player") StartCoroutine(Sequence());}

    IEnumerator Sequence(){
        if (finished == 0 && !protagSource.isPlaying && controls.inExplore){
            finished = 1;
            // Protag
            // Alright, I’m near, I can hear it. Gotta listen for guards… 
            protagSource.PlayOneShot(protagClip);
            yield return new WaitForSeconds(protagClip.length);
            saveAndLoadPumpingStation.SavePumpingStation();
        }
    }
}
