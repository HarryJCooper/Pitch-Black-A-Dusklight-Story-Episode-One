using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DearVR;

public class TurnOffOnEnter : MonoBehaviour
{
    [SerializeField] AudioClip stopClip, loopClip, startClip;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Transform playerTransform;
    DearVRSource audioVRSource;
    [SerializeField] float maxDistance = 150;
    public int hasStopped;
    [SerializeField] SaveAndLoadPumpingStation saveAndLoadPumpingStation;
    bool hasPlayedStartClip;
    [SerializeField] bool rightSide;

    void Start(){ 
        audioSource = GetComponent<AudioSource>();
        audioVRSource = audioSource.GetComponent<DearVRSource>();
        audioVRSource.performanceMode = true;
        hasStopped = PlayerPrefs.GetInt(this.gameObject.name);
        if (hasStopped == 0) StartCoroutine(Sequence());
    }

    void OnTriggerEnter(Collider other){ 
        hasStopped = 1;
        saveAndLoadPumpingStation.SavePumpingStation();
    }

    public IEnumerator Sequence(){
        if (!audioSource.gameObject.activeInHierarchy) yield break;
        if (playerTransform.position.z > (this.gameObject.transform.position.z + 10f)) yield break;
        audioSource = GetComponent<AudioSource>();
        audioVRSource = audioSource.GetComponent<DearVRSource>();
        audioVRSource.performanceMode = true;
        if (hasStopped == 0){
            if ((rightSide && playerTransform.position.x > 0) || (!rightSide && playerTransform.position.x < 0)){
                if (Vector3.Distance(this.gameObject.transform.position, playerTransform.position) < maxDistance){
                    if (!hasPlayedStartClip){
                        hasPlayedStartClip = true;
                        audioVRSource.DearVRPlayOneShot(startClip);
                        yield return new WaitForSeconds(startClip.length);
                    }
                    audioVRSource.DearVRPlayOneShot(loopClip);
                    yield return new WaitForSeconds(loopClip.length);
                }
            }
            yield return new WaitForSeconds(3f);
            StartCoroutine(Sequence());
            yield break;
        } 
        audioVRSource.DearVRPlayOneShot(stopClip);
        yield return new WaitForSeconds(stopClip.length);
        PlayerPrefs.SetInt(this.gameObject.name, hasStopped);
    }
}
