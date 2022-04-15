using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappedWorkerSequence : SequenceBase
{
    [SerializeField] AudioSource protagSource, protagReverbSource, trappedWorkerSource;
    [SerializeField] AudioClip[] protagClips, trappedWorkerClips, trappedWorkerLoopClips;
    [SerializeField] DecisionPrompt decisionPrompt;
    [SerializeField] SavedWorkerSequence savedWorkerSequence;
    [SerializeField] LeftWorkerSequence leftWorkerSequence;
    [SerializeField] AmbienceRepeater collapseRepeater;
    [SerializeField] TurnOnGoo turnOnGoo;

    void PlayOneShotWithVerb(AudioClip clip){
        protagSource.PlayOneShot(clip);
        protagReverbSource.PlayOneShot(clip);
    }

    void Awake(){active = 1;}

    public override IEnumerator Sequence(){
        if (finished == 0 && active == 1){
            if (triggered == 1){
                turnOnGoo.TurnOffGooVolumeController();
                StartCoroutine(collapseRepeater.ambienceCoroutine); 
                // Protag 
                // Don’t have time, I need to find those docs..
                PlayOneShotWithVerb(protagClips[0]);
                yield return new WaitForSeconds(protagClips[0].length);

                // Factory Worker
                // Please! 
                trappedWorkerSource.PlayOneShot(trappedWorkerClips[0]);
                yield return new WaitForSeconds(trappedWorkerClips[0].length);

                // Protag 
                // Save him, or go straight for the docs… gotta make a choice… and gotta make it quick… 
                PlayOneShotWithVerb(protagClips[1]);
                yield return new WaitForSeconds(protagClips[1].length);

                // Factory Worker
                // Gotta move to me, then we can get out of here.
                trappedWorkerSource.PlayOneShot(trappedWorkerClips[1]);
                yield return new WaitForSeconds(trappedWorkerClips[1].length);
                decisionPrompt.lightOrDarkDecision = 1;
                StartCoroutine(decisionPrompt.DecisionLoop());
                StartCoroutine(CheckDecisionLoop());
                Debug.Log("checking decision loop");
            } else {
                if (trappedWorkerSource.gameObject.activeSelf){
                    trappedWorkerSource.PlayOneShot(trappedWorkerLoopClips[Random.Range(0, trappedWorkerLoopClips.Length)]);
                    yield return new WaitForSeconds(15f); 
                    StartCoroutine(Sequence());
                    yield break;
                } 
                yield return new WaitForSeconds(5f); 
                StartCoroutine(Sequence());
            }
        }
    }

    IEnumerator CheckDecisionLoop(){
        if (decisionPrompt.lightOrDarkDecision == 3){
            StartCoroutine(leftWorkerSequence.Sequence());
        } else if (decisionPrompt.lightOrDarkDecision == 2) {
            StartCoroutine(savedWorkerSequence.Sequence());
        } else if (decisionPrompt.lightOrDarkDecision == 1){
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(CheckDecisionLoop());
        }
    }

    IEnumerator Finished(){
        finished = 1;
        yield break;
    }
}
