using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ChangeAfterFocus : MonoBehaviour
{
    AudioMixer audioMixer;
    AudioSource audioSource;
    [SerializeField] AudioSource playerSource;
    [SerializeField] AudioClip audioClip, reachAudio, hitPOI, unhitPOI;
    public bool hit;
    bool hitPOIPlayed, unhitPOIPlayed;
    [SerializeField] float zoomedVolume;
    float volumeIncreaser;
    string poiString;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] Controls controls;

    void Start(){
        audioSource = GetComponent<AudioSource>();
        audioMixer = audioSource.outputAudioMixerGroup.audioMixer;
        poiString = audioSource.outputAudioMixerGroup.name + "_HitVol";
    }
    
    void Update(){
        if (controls.inZoom && pBFootstepSystem.canMove){
            Zoom();    
            return;
        }
        volumeIncreaser = 0;
        audioMixer.SetFloat(poiString, 0); 
        hit = false;
    }

    void Zoom(){
        // IDEA- MAKE ZOOMEDVOLUME INCREASE DEPENDANT ON PLAYER DARKNESS
        if (hit){
            if (!hitPOIPlayed){
                playerSource.PlayOneShot(hitPOI);
                hitPOIPlayed = true;
                unhitPOIPlayed = false;
            }
            volumeIncreaser += Time.deltaTime;
            audioMixer.SetFloat(poiString, Mathf.Lerp(0, zoomedVolume, volumeIncreaser)); 
            return;
        }
        volumeIncreaser = 0;
        audioMixer.SetFloat(poiString, 0); 
        if (!unhitPOIPlayed && controls.inZoom){
            hitPOIPlayed = false;
            playerSource.PlayOneShot(unhitPOI);
            unhitPOIPlayed = true;
        }
    }
}

