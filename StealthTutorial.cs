using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthTutorial : MonoBehaviour
{
    Controls controls;
    AudioSource playerSource, enemySource;
    [SerializeField] AudioSource charlieSource;
    public IEnumerator loopCoroutine;
    [SerializeField] AudioClip[] playerDetectedCountDownClips, enemyLoopClips, enemyFoundPlayerClips;
    [SerializeField] AudioClip enemyFoundPlayerClangClip, enemyFoundPlayDistortionClip;
    [SerializeField] float currentDetectionDistance, detectionDistance, distanceFromPlayer;
    [SerializeField] bool playerCrouching, hasBeenTurnedOn = false;
    [SerializeField] TutorialStealthSequence tutorialStealthSequence;
    [SerializeField] AudioController audioController;
    bool hasTurnedOn, beingDetected, beenCaughtAlready, playingMusic;
    public bool beenCaught;
    public bool turnedOn;
    
    private void Start(){
        playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
        enemySource = GetComponent<AudioSource>();
        controls = GameObject.Find("Controls").GetComponent<Controls>();
        loopCoroutine = RepeatLoopClips();
    }

    void Update(){ 
        if (!turnedOn) return;
        if (!hasBeenTurnedOn){
            hasBeenTurnedOn = true;
            controls.inStealth = true;
        }
        DetectionDistanceModifier();
        CheckIfDetected();
    }

    void DetectionDistanceModifier(){
        playerCrouching = controls.crouching;
        if (playerCrouching){
            currentDetectionDistance = detectionDistance / 2;
            if (!playingMusic){
                audioController.PlayMusic("stealth");
                playingMusic = true;
            }
        } else {
            currentDetectionDistance = detectionDistance;
            if (playingMusic){
                StartCoroutine(audioController.FadeMusic());
                playingMusic = false;
            }
        }
        distanceFromPlayer = Vector3.Distance(playerSource.transform.position, enemySource.transform.position);
    }

    void CheckIfDetected(){if (distanceFromPlayer < currentDetectionDistance) StartCoroutine(PlayerIsFound());}

    IEnumerator PlayerIsFound(){
        controls.inCutscene = true;
        beenCaught = true;
        charlieSource.Stop();
        turnedOn = false;
        controls.inStealth = false;
        enemySource.Stop();
        enemySource.transform.position = new Vector3(-1.5f, 0.3f, 14);
        // Mother
        // I knew it! You think you can sneak past me you little horror!
        // Mother
        // Ha! Caught you, scoundrels, disrespectful little jerks!
        if (!beenCaughtAlready){
            playerSource.PlayOneShot(enemyFoundPlayerClangClip);
            enemySource.PlayOneShot(enemyFoundPlayerClips[0]);
            playerSource.PlayOneShot(enemyFoundPlayDistortionClip);
            beenCaughtAlready = true;
        } else {
            playerSource.PlayOneShot(enemyFoundPlayerClangClip);
            playerSource.PlayOneShot(enemyFoundPlayDistortionClip);
            enemySource.PlayOneShot(enemyFoundPlayerClips[1]);
        }
        enemySource = GetComponent<AudioSource>();
        yield return new WaitForSeconds(enemyFoundPlayDistortionClip.length);
        audioController.SetCutOffToZero();
        tutorialStealthSequence.RestartSequence();
        beenCaught = false;
    }

    public IEnumerator RepeatLoopClips(){
        enemySource.PlayOneShot(enemyLoopClips[Random.Range(0, enemyLoopClips.Length)]);
        yield return new WaitForSeconds(enemyLoopClips[0].length);
        if (turnedOn && enemySource){
            StartCoroutine(loopCoroutine);
        }
    }
}
