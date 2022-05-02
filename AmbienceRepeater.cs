using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceRepeater : MonoBehaviour
{
    [SerializeField] AudioSource[] audioSources;
    [SerializeField] AudioClip[] ambienceClips;
    public IEnumerator ambienceCoroutine;
    public bool stopped = false;
    public bool turnedOn;

    IEnumerator WaitThenPlay(){
        yield return new WaitForSeconds(2f);
        if (turnedOn) StartCoroutine(ambienceCoroutine);
    }

    void Awake(){
        ambienceCoroutine = PlayAllAmbiences();
    }

    void Start(){
        StartCoroutine(WaitThenPlay());
    }

    public void PlayAllSources(){
        stopped = false;
        StartCoroutine(PlayAllAmbiences());
    }

    public void StopAllSources(){
        Debug.Log("stopped");
        stopped = true;
        foreach (AudioSource audioSource in audioSources) audioSource.Stop();
    }

    IEnumerator PlayAllAmbiences(){
        foreach (AudioSource audioSource in audioSources){
            if (audioSource.gameObject.activeInHierarchy) audioSource.PlayOneShot(ambienceClips[Random.Range(0, ambienceClips.Length)]);
        }
        yield return new WaitForSeconds(ambienceClips[0].length/2 + Random.Range(-2f, 2f));    
        ambienceCoroutine = PlayAllAmbiences();
        if(!stopped && this.gameObject.activeSelf) StartCoroutine(ambienceCoroutine);
    }
}
