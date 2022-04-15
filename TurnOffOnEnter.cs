using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffOnEnter : MonoBehaviour
{
    [SerializeField] AudioClip stopClip, loopClip;
    [SerializeField] AudioSource audioSource;
    public int hasStopped;

    void Start(){ audioSource = GetComponent<AudioSource>();}

    void OnTriggerEnter(Collider other){ hasStopped = 1;}

    public IEnumerator Sequence(){
        if (hasStopped == 0){
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
