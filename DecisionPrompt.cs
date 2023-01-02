using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionPrompt : MonoBehaviour
{
    [SerializeField] AudioSource playerActionSource;
    [SerializeField] AudioClip[] loopClips;
    [SerializeField] AudioClip lightDecisionClip, darkDecisionClip, reminderClip, mobileClip, computerClip;
    public short lightOrDarkDecision = 0;
    [SerializeField] Controls controls;
    bool hasPlayed;

    void Start(){
        if (controls.mobile){
            reminderClip = mobileClip;
        } else {
            reminderClip = computerClip;
        }
    }

    void Update(){
        if (lightOrDarkDecision == 1){
            if (controls.swipeRight || controls.doubleTapLeft){
                playerActionSource.Stop();
                playerActionSource.PlayOneShot(lightDecisionClip, 0.1f);
                lightOrDarkDecision = 2;
            }
            if (controls.swipeLeft || controls.doubleTapRight){
                playerActionSource.Stop();
                playerActionSource.PlayOneShot(darkDecisionClip, 0.1f);
                lightOrDarkDecision = 3;
            }
        }
    }

    public IEnumerator DecisionLoop(){
        playerActionSource.PlayOneShot(loopClips[Random.Range(0, loopClips.Length)], 0.02f);
        yield return new WaitForSeconds(loopClips[0].length / 2);
        if (lightOrDarkDecision == 1){
            StartCoroutine(DecisionLoop());
        }
    }

    public IEnumerator DecisionLoopTwo(){
        if(!hasPlayed) yield return new WaitForSeconds(6f);
        hasPlayed = true;
        if (lightOrDarkDecision != 1) yield break;
        playerActionSource.PlayOneShot(reminderClip, 0.2f);
        yield return new WaitForSeconds(reminderClip.length);
        if (lightOrDarkDecision == 1){
            StartCoroutine(DecisionLoopTwo());
        }
    }
}
