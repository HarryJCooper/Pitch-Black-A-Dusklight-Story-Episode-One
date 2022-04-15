using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ZoomSound : MonoBehaviour
{
    [SerializeField] AudioSource playerSource;
    [SerializeField] AudioClip zoomInClip, zoomOutClip;
    [SerializeField] AudioClip[] crackleClips, darknessClips;
    [SerializeField] Controls controls;
    [SerializeField] AudioMixer audioMixer;
    float adjuster;
    bool hasPlayedExit = true, transitioning = false;
    [SerializeField] DarkVsLight darkVsLight;

    void ZoomAdjuster(){
        if (controls.inZoom){ // IF CURRENTLY ZOOMED
            if (adjuster < 1){
                adjuster += 0.02f;              
            } else {
                transitioning = false;
            }
        } else {
            if (adjuster > 0){
                adjuster -= 0.02f;              
            } else {
                transitioning = false;
            } 
        }
        audioMixer.SetFloat("OtherSources_Vol", Mathf.Lerp(0, -3, adjuster)); 
        audioMixer.SetFloat("OtherSources_CutOff", Mathf.Lerp(22000, 300, adjuster)); 
    }

    void ZoomIn(){ playerSource.PlayOneShot(zoomInClip);}

    void ZoomOut(){ playerSource.PlayOneShot(zoomOutClip);}

    IEnumerator CrackleRepeater(){
        playerSource.PlayOneShot(crackleClips[Random.Range(0, crackleClips.Length)], 0.3f);
        yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
        if (controls.inZoom){
            StartCoroutine(CrackleRepeater());
        } else {
            yield break;
        }
    }

    IEnumerator DarkAmbiRepeater(){
        playerSource.PlayOneShot(darknessClips[Random.Range(0, darknessClips.Length)], (darkVsLight.playerDarkness * 0.05f));
        yield return new WaitForSeconds(2);
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
            ZoomIn();
            hasPlayedExit = false;
        }

        if (!controls.inZoom && !hasPlayedExit){
            hasPlayedExit = true;
            transitioning = true;
            ZoomOut();
        }
    }

    void Start(){ hasPlayedExit = true;}
}
