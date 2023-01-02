using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DearVR;

public class AmbienceRepeater : MonoBehaviour
{
    public AudioSource[] audioSources;
    [SerializeField] AudioClip[] ambienceClips;
    public IEnumerator ambienceCoroutine;
    [SerializeField] bool optimize;
    public bool stopped = false;
    public bool turnedOn;
    public float volume;
    [SerializeField] float maxDist;
    Transform playerTransform;

    IEnumerator WaitThenPlay(){
        yield return new WaitForSeconds(2f);
        if (turnedOn) StartCoroutine(ambienceCoroutine);
    }

    void Awake(){
        playerTransform = GameObject.Find("Player").transform;
        ambienceCoroutine = PlayAllAmbiences();
        foreach (AudioSource audioSource in audioSources){
            if (audioSource.gameObject.activeInHierarchy){
                DearVRSource audioVRSource = audioSource.GetComponent<DearVRSource>();
                audioVRSource.performanceMode = true;
            }
        }
    }

    void Start(){
        StartCoroutine(WaitThenPlay());
        if (volume == 0) volume = 1;
    }

    public void PlayAllSources(){
        stopped = false;
        StartCoroutine(PlayAllAmbiences());
    }

    public void StopAllSources(){
        stopped = true;
        foreach (AudioSource audioSource in audioSources){
            DearVRSource audioVRSource = audioSource.GetComponent<DearVRSource>();
            if (audioSource.isPlaying) audioVRSource.DearVRStop();
        } 
    }

    IEnumerator PlayAllAmbiences(){
        foreach (AudioSource audioSource in audioSources){
            if (audioSource.gameObject.activeInHierarchy){
                audioSource.volume = volume;
                DearVRSource audioVRSource = audioSource.GetComponent<DearVRSource>();
                audioVRSource.performanceMode = true;
                if (optimize && (Vector3.Distance(audioSource.transform.position, playerTransform.position) < maxDist)){
                    audioVRSource.DearVRPlayOneShot(ambienceClips[Random.Range(0, ambienceClips.Length)]);
                } else if (!optimize){
                    audioVRSource.DearVRPlayOneShot(ambienceClips[Random.Range(0, ambienceClips.Length)]);
                }
            }
        }
        yield return new WaitForSeconds(ambienceClips[0].length/2 + Random.Range(-2f, 2f));    
        ambienceCoroutine = PlayAllAmbiences();
        if(!stopped && this.gameObject.activeInHierarchy) StartCoroutine(ambienceCoroutine);
    }
}
