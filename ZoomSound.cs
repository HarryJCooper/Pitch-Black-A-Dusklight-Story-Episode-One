using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomSound : MonoBehaviour{
    public AudioSource player;
    public AudioClip zoomIn, zoomOut;
    public AudioClip[] crackle;
    private bool crackling, hasCrackled;
    public Controls controls;

    void Update(){
        if(controls.canZoom){
            if (Input.GetKeyDown(KeyCode.Space)){
                ZoomIn();
                crackling = true;
            }

            if (Input.GetKeyUp(KeyCode.Space)){
                ZoomOut();
                crackling = false;
            }

            if (crackling){
                RepeatCrackle();
            }
        }
    }

    void ZoomIn(){
        player.PlayOneShot(zoomIn);
    }

    void ZoomOut(){
        player.PlayOneShot(zoomOut);
    }

    void RepeatCrackle(){
        if (!hasCrackled){
            StartCoroutine(CrackleRepeater());
            hasCrackled = true;
            player.PlayOneShot(crackle[Random.Range(0, crackle.Length)], 0.3f);
        }
    }

    IEnumerator CrackleRepeater(){
        yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
        hasCrackled = false;
    }
}
