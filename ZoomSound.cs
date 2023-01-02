using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ZoomSound : MonoBehaviour
{
    [SerializeField] AudioSource playerSource, playerActionSource;
    [SerializeField] AudioClip zoomInClip, zoomOutClip;
    [SerializeField] AudioClip[] crackleClips, darknessClips;
    [SerializeField] Controls controls;
    [SerializeField] AudioMixer audioMixer;
    float adjuster;
    bool hasPlayedExit = true, transitioning = false;
    public bool disabledZoomSound;
    [SerializeField] DarkVsLight darkVsLight;

    void ZoomAdjuster(){
        if (controls.inZoom){ // IF CURRENTLY ZOOMED
            if (adjuster < 1){
                adjuster += 0.04f;              
            } else {
                transitioning = false;
            }
        } else {
            if (adjuster > 0){
                adjuster -= 0.04f;
            } else {
                transitioning = false;
            } 
        }
        audioMixer.SetFloat("OtherSources_Vol", Mathf.Lerp(0, -3, adjuster)); 
        audioMixer.SetFloat("OtherSources_CutOff", Mathf.Lerp(22000, 300, adjuster)); 
    }

    void ZoomIn(){ playerActionSource.PlayOneShot(zoomInClip);}

    void ZoomOut(){ playerActionSource.PlayOneShot(zoomOutClip);}

    IEnumerator CrackleRepeater(){
        playerActionSource.PlayOneShot(crackleClips[Random.Range(0, crackleClips.Length)], 0.15f);
        yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
        if (controls.inZoom){
            StartCoroutine(CrackleRepeater());
        } else {
            yield break;
        }
    }

    IEnumerator DarkAmbiRepeater(){
        playerSource.PlayOneShot(darknessClips[Random.Range(0, darknessClips.Length)], (darkVsLight.playerDarkness * 0.08f));
        yield return new WaitForSeconds(2f);
        if (controls.inZoom){
            StartCoroutine(DarkAmbiRepeater());
        } else {
            yield break;
        }
    }

    void FixedUpdate(){
        if (transitioning) ZoomAdjuster();
    }

    void Update(){
        if (controls.enteredZoom){
            StartCoroutine(CrackleRepeater());
            StartCoroutine(DarkAmbiRepeater());
            transitioning = true;
            if(!disabledZoomSound) ZoomIn();
            hasPlayedExit = false;
        }

        if (!controls.inZoom && !hasPlayedExit){
            hasPlayedExit = true;
            transitioning = true;
            if(!disabledZoomSound) ZoomOut();
        }
    }

    void Start(){ hasPlayedExit = true;}
}
