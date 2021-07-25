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

    public AudioClip playerDetectedClip, enemyDetectedPlayerClip, enemyDetectedPlayerWarningClip, beepingClip;
    public float currentDetectionDistance, detectionDistance, enemyAngleToPlayer;
    float distanceFromPlayer, detectionTimer;
    bool hasTurnedOn, beingDetected;
    public bool playerSprinting, playerCrouching, turnedOn;

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
                StartCoroutine(DistanceFromEnemyBeep());
                StartCoroutine(GetAngleTowardsPlayer());
                hasTurnedOn = true;
            }

            playerSprinting = controls.sprint;
            
            if (playerSprinting){
                currentDetectionDistance = detectionDistance * 2;
            } else if (playerCrouching){
                currentDetectionDistance = detectionDistance / 2;
            } else {
                currentDetectionDistance = detectionDistance;
            }

            distanceFromPlayer = Vector3.Distance(playerSource.transform.position, enemySource.transform.position);

            if (distanceFromPlayer < detectionDistance){
                PlayerIsHeard();
                turnedOn = false;
            }

            ChangeEnemyAudio();
        }
    }

    IEnumerator DistanceFromEnemyBeep()
    {
        yield return new WaitForSeconds(distanceFromPlayer / 10);
        float lowpassFrequency = 200;

        if (enemyAngleToPlayer > 135){
            if (lowpassFrequency < 5000){
                lowpassFrequency += 10 * Time.deltaTime;
            } else if (lowpassFrequency > 200) {
            lowpassFrequency -= 10  * Time.deltaTime;
            }
        }
        
        enemyMixer.SetFloat("EnemyBeepLowpassFreq", lowpassFrequency);
        enemySource.PlayOneShot(beepingClip);
        
        StartCoroutine(DistanceFromEnemyBeep());
    }

    IEnumerator GetAngleTowardsPlayer()
    {
        // Calculate the vector pointing from the enemy to the player
        Vector3 enemyDir = transform.position - playerSource.transform.position;

        // Calculate the angle between the forward vector of the player and the vector pointing to the enemy
        enemyAngleToPlayer = Vector3.Angle(transform.forward, enemyDir);
        yield return new WaitForSeconds(0.1f);
        if (enemyAngleToPlayer > 135){
            beingDetected = true;
            enemySource.PlayOneShot(enemyDetectedPlayerWarningClip);
        }
        StartCoroutine(GetAngleTowardsPlayer());
    }

    void ChangeEnemyAudio(){
        if (beingDetected) detectionTimer += Time.deltaTime;
    }

    void PlayerIsHeard(){
        controls.inCombat = true;
        controls.inStealth = false;
        turnedOn = false;
        playerSource.PlayOneShot(playerDetectedClip);
        enemySource.PlayOneShot(enemyDetectedPlayerClip);
    }
}
