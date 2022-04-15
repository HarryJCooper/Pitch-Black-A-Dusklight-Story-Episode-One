using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionPrompt : MonoBehaviour
{
    [SerializeField] AudioSource playerSource;
    [SerializeField] AudioClip[] loopClips;
    [SerializeField] AudioClip lightDecisionClip, darkDecisionClip;
    public short lightOrDarkDecision = 0;
    [SerializeField] Controls controls;

    void Update(){
        if (lightOrDarkDecision == 1){
            if (controls.swipeRight || controls.doubleTapLeft){
                playerSource.PlayOneShot(lightDecisionClip, 0.1f);
                lightOrDarkDecision = 2;
            }
            if (controls.swipeLeft || controls.doubleTapRight){
                playerSource.PlayOneShot(darkDecisionClip, 0.1f);
                lightOrDarkDecision = 3;
            }
        }
    }

    public IEnumerator DecisionLoop(){
        playerSource.PlayOneShot(loopClips[Random.Range(0, loopClips.Length)], 0.02f);
        yield return new WaitForSeconds(loopClips[0].length / 2);
        if (lightOrDarkDecision == 1){
            StartCoroutine(DecisionLoop());
        }
    }
}
