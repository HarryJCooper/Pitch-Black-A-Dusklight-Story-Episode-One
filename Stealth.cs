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

    public AudioClip playerFoundClip, enemyFoundPlayerClip, enemyDetectedPlayerClip, beepingClip;
    public AudioClip[] playerDetectedCountDownClips;
    public float currentDetectionDistance, detectionDistance, enemyAngleToPlayer, distanceFromPlayer, detectionTimer, maxDetectionTime;
    float lowpassFrequency = 200;
    int playerDetectedClipCounter = 0;
    bool hasTurnedOn, beingDetected, beingDetectedClipPlayed;
    public bool playerSprinting, playerCrouching, turnedOn, closeEnoughToAssassinate;

    private void Start(){
        playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
        enemySource = GetComponent<AudioSource>();
        enemyMixer = enemySource.outputAudioMixerGroup.audioMixer;
        controls = GameObject.Find("Controls").GetComponent<Controls>();
    }

    private void Update(){ 
        if (Input.GetKeyDown(KeyCode.X)){ // REFACTOR - this is temporary
            turnedOn = true;
            controls.inStealth = true;
        }

        if (turnedOn){
            if (!hasTurnedOn){
                StartCoroutine(DistanceFromEnemySignifier());
                StartCoroutine(GetAngleTowardsPlayer());
                hasTurnedOn = true;
            }

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

            CheckIfDetected();
        }
    }

    IEnumerator DistanceFromEnemySignifier(){
        yield return new WaitForSeconds(distanceFromPlayer / 30); 
        if (enemyAngleToPlayer > 135){
            if (lowpassFrequency < 5000){
                lowpassFrequency += 10 * Time.deltaTime;
            } else if (lowpassFrequency > 200) {
            lowpassFrequency -= 10  * Time.deltaTime;
            }
        }
        if (distanceFromPlayer < currentDetectionDistance) beingDetected = true; else beingDetected = false;

        enemyMixer.SetFloat("EnemyBeepLowpassFreq", lowpassFrequency);
        enemySource.PlayOneShot(beepingClip);
        
        StartCoroutine(DistanceFromEnemySignifier());
    }

    IEnumerator GetAngleTowardsPlayer(){
        // Calculate the vector pointing from the enemy to the player
        Vector3 enemyDir = transform.position - playerSource.transform.position;

        // Calculate the angle between the forward vector of the player and the vector pointing to the enemy
        enemyAngleToPlayer = Vector3.Angle(transform.forward, enemyDir);
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
        }
        if (detectionTimer > maxDetectionTime || distanceFromPlayer < currentDetectionDistance * 0.5f) PlayerIsFound();
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

    void PlayerIsFound(){
        controls.inCombat = true;
        controls.inStealth = false;
        turnedOn = false;
        playerSource.PlayOneShot(playerFoundClip);
        enemySource.PlayOneShot(enemyFoundPlayerClip);
    }
}
