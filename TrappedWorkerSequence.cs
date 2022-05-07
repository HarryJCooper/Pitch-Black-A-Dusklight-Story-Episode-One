using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappedWorkerSequence : MonoBehaviour
{
    public int active = 1, triggered = 0, finished = 0;
    public AudioClip cutsceneEnterClip, cutsceneExitClip;
    [SerializeField] AudioSource protagSource, protagReverbSource, trappedWorkerSource;
    [SerializeField] AudioClip[] protagClips, trappedWorkerClips, trappedWorkerLoopClips;
    [SerializeField] DecisionPrompt decisionPrompt;
    [SerializeField] SavedWorkerSequence savedWorkerSequence;
    [SerializeField] LeftWorkerSequence leftWorkerSequence;
    [SerializeField] AmbienceRepeater collapseRepeater;
    [SerializeField] TurnOnGoo turnOnGoo;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    float distanceFromPlayer;
    [SerializeField] float maxDistanceFromPlayer;
    int trappedWorkerLoopClipsInt, randomInt;
    public bool isRunning;
    bool hasStartedRepeater;

    public void Update(){
        if (isRunning && triggered == 0){
            distanceFromPlayer = Vector3.Distance(protagSource.transform.position, trappedWorkerSource.transform.position);
            if (distanceFromPlayer < maxDistanceFromPlayer){
                triggered = 1;
                StartCoroutine(Sequence());
            } 
        } 
    }

    int RandomNumberGen(){
        randomInt = Random.Range(0, trappedWorkerLoopClips.Length);
        if (randomInt == trappedWorkerLoopClipsInt) randomInt = RandomNumberGen();
        return randomInt;
    }

    void PlayOneShotWithVerb(AudioClip clip){
        protagSource.PlayOneShot(clip);
        protagReverbSource.PlayOneShot(clip, 0.4f);
    }

    void Awake(){
        active = 1;
    }

    public IEnumerator Sequence(){
        if (!hasStartedRepeater){
            hasStartedRepeater = true;
            StartCoroutine(collapseRepeater.ambienceCoroutine); 
        }
        isRunning = true;
        if (finished == 0 && active == 1){
            if (triggered == 1){
                finished = 1;
                pBFootstepSystem.canRotate = false;
                controls.inCutscene = true;
                turnOnGoo.TurnOffGooVolumeController();
                // Protag 
                // Don’t have time, I need to find those docs..
                PlayOneShotWithVerb(protagClips[0]);
                yield return new WaitForSeconds(protagClips[0].length);

                trappedWorkerSource.Stop();
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
            } else {
                if (triggered == 1) yield break;
                if (trappedWorkerSource.gameObject.activeSelf){
                    trappedWorkerLoopClipsInt = RandomNumberGen();
                    trappedWorkerSource.PlayOneShot(trappedWorkerLoopClips[trappedWorkerLoopClipsInt]);
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
            pBFootstepSystem.canRotate = true;
            controls.inCutscene = false;
            leftWorkerSequence.enabled = true;
            StartCoroutine(leftWorkerSequence.Sequence());
        } else if (decisionPrompt.lightOrDarkDecision == 2){
            pBFootstepSystem.canRotate = true;
            controls.inCutscene = false;
            savedWorkerSequence.enabled = true;
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
