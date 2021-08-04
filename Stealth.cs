using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Stealth : MonoBehaviour
{
    // IF WITHIN A CERTAIN DISTANCE PLAYER IS HEARD
    // IF MOVING TOO FAST PLAYER IS HEARD
    // IF ENEMY IS FACING PLAYER PLAYER HAS TO BE STILL? - would need to use some indicator that you're being looked at, maybe a timer?

    AudioMixer enemyMixer;
    Controls controls;
    AudioSource playerSource, enemySource;

    [SerializeField] AudioClip playerFoundClip, enemyFoundPlayerClip, enemyDetectedPlayerClip, beepingClip, playerAssassinatedEnemyClip, enemyAssassinatedByPlayerClip;
    [SerializeField] AudioClip[] playerDetectedCountDownClips, enemyBreathingClips;
    public float currentDetectionDistance, detectionDistance, enemyAngleToPlayer, distanceFromPlayer, detectionTimer, maxDetectionTime, maxAssinateDistance = 2;
    float lowpassFrequency = 200;
    int playerDetectedClipCounter = 0;
    bool hasTurnedOn, beingDetected, beingDetectedClipPlayed, enemyFacingAway, canAssassinate;
    public bool playerSprinting, playerCrouching, turnedOn;

    private void Start(){
        playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
        enemySource = GetComponent<AudioSource>();
        enemyMixer = enemySource.outputAudioMixerGroup.audioMixer;
        controls = GameObject.Find("Controls").GetComponent<Controls>();
    }

    private void Update(){ 
        if (Input.GetKeyDown(KeyCode.X) && !turnedOn){ // REFACTOR - this is temporary
            turnedOn = true;
            controls.inStealth = true;
            StartCoroutine(RepeatEnemyBreathing());
            StartCoroutine(DistanceFromEnemySignifier());
            StartCoroutine(GetAngleTowardsPlayer());
        }
        if (turnedOn){
            DetectionDistanceModifier();
            CheckIfDetected();
            CheckIfCanAssassinate();
        }
    }

    void DetectionDistanceModifier(){
        playerSprinting = controls.sprint;
            playerCrouching = controls.crouching;
            
            if (playerSprinting){
                currentDetectionDistance = detectionDistance * 2;
            } else if (playerCrouching){
                currentDetectionDistance = detectionDistance / 2;
            } else {
                currentDetectionDistance = detectionDistance;
            }

            distanceFromPlayer = Vector3.Distance(playerSource.transform.position, enemySource.transform.position);
    }

    IEnumerator DistanceFromEnemySignifier(){
        yield return new WaitForSeconds(distanceFromPlayer / 30); 
        lowpassFrequency = (float)enemyAngleToPlayer * 10f;
        if (distanceFromPlayer < currentDetectionDistance) beingDetected = true; else beingDetected = false;

        enemyMixer.SetFloat("EnemyLowpassFreq", lowpassFrequency);
        Debug.Log(lowpassFrequency);
        enemySource.PlayOneShot(beepingClip);
        
        StartCoroutine(DistanceFromEnemySignifier());
    }

    IEnumerator GetAngleTowardsPlayer(){
        // Calculate the vector pointing from the enemy to the player
        Vector3 enemyDir = transform.position - playerSource.transform.position;

        // Calculate the angle between the forward vector of the player and the vector pointing to the enemy
        enemyAngleToPlayer = Vector3.Angle(transform.forward, enemyDir);
        if (enemyAngleToPlayer < 100) enemyFacingAway = true;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(GetAngleTowardsPlayer());
    }

    void CheckIfDetected(){
        if (beingDetected){
            detectionTimer += Time.deltaTime;
            PlayerIsHeard();
        } else {
            beingDetectedClipPlayed = false;
            playerDetectedClipCounter = 0;
        } // LOOK AT ME! MAKE IT SO THAT YOU HAVE TO BE CROUCHING TO ASSASSINATE
        if (detectionTimer > maxDetectionTime || (distanceFromPlayer < currentDetectionDistance * 0.5f && !enemyFacingAway)) StartCoroutine(PlayerIsFound());
    }

    void PlayerIsHeard(){
        if (!beingDetectedClipPlayed){ 
            enemySource.PlayOneShot(enemyDetectedPlayerClip);
            beingDetectedClipPlayed = true;
        }
        if (beingDetected && !playerSource.isPlaying){ // REFACTOR - could use a coroutine here.
            playerSource.PlayOneShot(playerDetectedCountDownClips[playerDetectedClipCounter]);
            playerDetectedClipCounter += 1;    
        }
    }

    void CheckIfCanAssassinate(){
        if (playerCrouching && distanceFromPlayer < maxAssinateDistance && enemyFacingAway){
            canAssassinate = true;
        } else {
            canAssassinate = false;
        }
    }

    void CheckIfAssassinate(){
        if (canAssassinate && (controls.doubleTap || Input.GetKeyDown(KeyCode.Space))){
            StartCoroutine(EnemyAssassinated());
        }
    }

    IEnumerator PlayerIsFound(){
        controls.inCombat = true;
        controls.inStealth = false;
        turnedOn = false;
        enemySource.PlayOneShot(enemyFoundPlayerClip);
        yield return new WaitForSeconds(enemyFoundPlayerClip.length);
        playerSource.PlayOneShot(playerFoundClip);
    }

    IEnumerator RepeatEnemyBreathing(){
        enemySource.PlayOneShot(enemyBreathingClips[Random.Range(0, enemyBreathingClips.Length)]);
        yield return new WaitForSeconds(enemyBreathingClips[0].length);
        StartCoroutine(RepeatEnemyBreathing());
    }

    IEnumerator EnemyAssassinated(){
        // REFACTOR - Test this
        playerSource.PlayOneShot(playerAssassinatedEnemyClip); 
        yield return new WaitForSeconds(playerAssassinatedEnemyClip.length);
        enemySource.PlayOneShot(enemyAssassinatedByPlayerClip); 
        yield return new WaitForSeconds(enemyAssassinatedByPlayerClip.length);
    }
}
