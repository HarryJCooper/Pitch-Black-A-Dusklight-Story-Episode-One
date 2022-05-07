using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffOnEnter : MonoBehaviour
{
    [SerializeField] AudioClip stopClip, loopClip, startClip;
    [SerializeField] AudioSource audioSource;
    public int hasStopped;
    [SerializeField] SaveAndLoadPumpingStation saveAndLoadPumpingStation;
    bool hasPlayedStartClip;

    void Start(){ audioSource = GetComponent<AudioSource>();}

    void OnTriggerEnter(Collider other){ 
        hasStopped = 1;
        saveAndLoadPumpingStation.SavePumpingStation();
    }

    public IEnumerator Sequence(){
        if (hasStopped == 0){
            if (!hasPlayedStartClip){
                hasPlayedStartClip = true;
                audioSource.PlayOneShot(startClip);
                yield return new WaitForSeconds(startClip.length);
            }
            audioSource.PlayOneShot(loopClip);
            yield return new WaitForSeconds(loopClip.length + 3f);
            StartCoroutine(Sequence());
            yield break;
        } 
        audioSource.PlayOneShot(stopClip);
        yield return new WaitForSeconds(stopClip.length);
        this.gameObject.SetActive(false);
    }
}
