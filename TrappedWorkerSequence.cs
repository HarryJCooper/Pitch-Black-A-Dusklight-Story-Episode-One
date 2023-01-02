using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DearVR;

public class TrappedWorkerSequence : MonoBehaviour
{
    public int active = 1, triggered = 0, finished = 0;
    public AudioClip cutsceneEnterClip, cutsceneExitClip, charlieOverlayClip;
    [SerializeField] AudioSource protagSource, protagReverbSource, protagActionSource, trappedWorkerSource;
    DearVRSource protagReverbVRSource, trappedWorkerVRSource;
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
            if (distanceFromPlayer < maxDistanceFromPlayer || ((trappedWorkerSource.transform.position.z - protagSource.transform.position.z) < 5)){
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
        protagReverbSource.volume = 0.4f;
        protagReverbVRSource.DearVRPlayOneShot(clip);
    }

    void Start(){
        active = 1;
        protagReverbVRSource = protagReverbSource.GetComponent<DearVRSource>();
        trappedWorkerVRSource = trappedWorkerSource.GetComponent<DearVRSource>();
        protagReverbVRSource.performanceMode = true;
        trappedWorkerVRSource.performanceMode = true;
    }

    public IEnumerator Sequence(){
        protagReverbVRSource = protagReverbSource.GetComponent<DearVRSource>();
        trappedWorkerVRSource = trappedWorkerSource.GetComponent<DearVRSource>();
        protagReverbVRSource.performanceMode = true;
        trappedWorkerVRSource.performanceMode = true;
        if (!hasStartedRepeater){
            hasStartedRepeater = true;
            StartCoroutine(collapseRepeater.ambienceCoroutine); 
        }
        isRunning = true;
        if (finished == 0 && active == 1){
            if (triggered == 1){
                finished = 1;
                protagSource.transform.LookAt(trappedWorkerSource.transform.position);
                pBFootstepSystem.canRotate = false;
                controls.inCutscene = true;
                protagActionSource.PlayOneShot(cutsceneEnterClip);
                yield return new WaitForSeconds(cutsceneEnterClip.length);
                turnOnGoo.TurnOffGooVolumeController();
                // Protag 
                // Don’t have time, I need to find those docs..
                PlayOneShotWithVerb(protagClips[0]);
                yield return new WaitForSeconds(protagClips[0].length);

                trappedWorkerVRSource.DearVRStop();
                // Factory Worker
                // Please! 
                trappedWorkerVRSource.DearVRPlayOneShot(trappedWorkerClips[0]);
                yield return new WaitForSeconds(trappedWorkerClips[0].length);

                // Protag 
                // Save him, or go straight for the docs… gotta make a choice… and gotta make it quick… 
                PlayOneShotWithVerb(protagClips[1]);
                yield return new WaitForSeconds(protagClips[1].length);

                // Factory Worker
                // Gotta move to me, then we can get out of here.
                protagActionSource.PlayOneShot(charlieOverlayClip);
                trappedWorkerVRSource.DearVRPlayOneShot(trappedWorkerClips[1]);
                decisionPrompt.lightOrDarkDecision = 1;
                StartCoroutine(decisionPrompt.DecisionLoop());
                StartCoroutine(decisionPrompt.DecisionLoopTwo());
                StartCoroutine(CheckDecisionLoop());
            } else {
                if (triggered == 1) yield break;
                if (trappedWorkerSource.gameObject.activeSelf){
                    trappedWorkerLoopClipsInt = RandomNumberGen();
                    if (!trappedWorkerSource.isPlaying){
                        trappedWorkerVRSource.DearVRPlayOneShot(trappedWorkerLoopClips[trappedWorkerLoopClipsInt]);
                    }
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
