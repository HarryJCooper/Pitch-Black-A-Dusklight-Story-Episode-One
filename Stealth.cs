using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Stealth : MonoBehaviour
{
    // IF WITHIN A CERTAIN DISTANCE PLAYER IS HEARD
    // IF MOVING TOO FAST PLAYER IS HEARD
    // IF ENEMY IS FACING PLAYER PLAYER HAS TO BE STILL? - would need to use some indicator that you're being looked at, maybe a timer?

    [SerializeField] DarkVsLight darkVsLight;
    AudioMixer enemyMixer;
    Controls controls;
    AudioSource playerSource, enemySource;

    [SerializeField] AudioClip playerFoundClip, enemyFoundPlayerClip, enemyDetectedPlayerClip, beepingClip, playerAssassinatedEnemyClip, enemyAssassinatedByPlayerClip;
    [SerializeField] AudioClip[] playerDetectedCountDownClips, enemyBreathingClips;
    public float currentDetectionDistance, detectionDistance, enemyAngleToPlayer, distanceFromPlayer, detectionTimer, maxDetectionTime, maxAssassinateDistance = 5;
    float lowpassFrequency = 200;
    int playerDetectedClipCounter = 0;
    bool hasTurnedOn, beingDetected, beingDetectedClipPlayed, enemyFacingAway;
    public bool playerSprinting, playerCrouching, turnedOn, hasBeenTurnedOn = false, canAssassinate;

    private void Start(){
        playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
        enemySource = GetComponent<AudioSource>();
        enemyMixer = enemySource.outputAudioMixerGroup.audioMixer;
        controls = GameObject.Find("Controls").GetComponent<Controls>();
    }

    private void Update(){ 
        if (Input.GetKeyDown(KeyCode.X) && !turnedOn && !hasBeenTurnedOn){ // REFACTOR - this is temporary
            turnedOn = true;
            hasBeenTurnedOn = true;
            controls.inStealth = true;
            StartCoroutine(RepeatEnemyBreathing());
            StartCoroutine(DistanceFromEnemySignifier());
            StartCoroutine(GetAngleTowardsPlayer());
        }
        if (turnedOn){
            DetectionDistanceModifier();
            CheckIfDetected();
            CheckIfCanAssassinate();
            CheckIfAssassinate();
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
        enemySource.PlayOneShot(beepingClip);
        if(turnedOn){
            StartCoroutine(DistanceFromEnemySignifier());
        } else {
            lowpassFrequency = 20000;
        }
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
            detectionTimer = 0;
        }
        if (detectionTimer > maxDetectionTime || (distanceFromPlayer < currentDetectionDistance * 0.5f && !enemyFacingAway)) StartCoroutine(PlayerIsFound());
    }

    void PlayerIsHeard(){
        if (!beingDetectedClipPlayed){ 
            enemySource.PlayOneShot(enemyDetectedPlayerClip);
            beingDetectedClipPlayed = true;
        }
        if (beingDetected && !playerSource.isPlaying){
            playerSource.PlayOneShot(playerDetectedCountDownClips[playerDetectedClipCounter]);
            playerDetectedClipCounter += 1; 
        }
    }

    void CheckIfCanAssassinate(){
        if (playerCrouching && distanceFromPlayer < maxAssassinateDistance && enemyFacingAway){
            canAssassinate = true;
        } else {
            canAssassinate = false;
        }
    }

    void CheckIfAssassinate(){
        if (canAssassinate && controls.enter){
            StartCoroutine(EnemyAssassinated());
        }
    }

    IEnumerator PlayerIsFound(){
        lowpassFrequency = 20000;
        enemyMixer.SetFloat("EnemyLowpassFreq", lowpassFrequency);
        turnedOn = false;
        controls.inStealth = false;
        controls.inCombat = true;
        StartCoroutine(this.GetComponent<Combat>().EnemyMoveTowardsOrTaunt());
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
        lowpassFrequency = 20000;
        enemyMixer.SetFloat("EnemyLowpassFreq", lowpassFrequency);
        turnedOn = false;
        controls.inStealth = false;
        playerSource.PlayOneShot(playerAssassinatedEnemyClip); 
        yield return new WaitForSeconds(playerAssassinatedEnemyClip.length);
        enemySource.PlayOneShot(enemyAssassinatedByPlayerClip); 
        yield return new WaitForSeconds(enemyAssassinatedByPlayerClip.length);
        darkVsLight.playerDarkness -= 1;
    }
}
