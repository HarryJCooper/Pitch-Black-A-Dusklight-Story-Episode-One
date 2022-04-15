using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DearVR;

public class Explosion : MonoBehaviour
{
    [SerializeField] AudioSource playerSource, pumpingStationSource, tannoy1Source, malfunctioningDoorSource;
    [SerializeField] AmbienceRepeater desertAmbienceRepeater, pumpingStationAmbienceRepeater;
    [SerializeField] AudioClip playerClip, stopClip, explosionClip;
    [SerializeField] AudioClip[] playerFootstepClips;
    [SerializeField] GuardCombatSequence guardCombatSequence;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] AudioController audioController;
    [SerializeField] TurnOffOnEnter tannoyOne, tannoyTwo, tannoyThree;
    [SerializeField] GameObject blockPumpingStationEntranceObject;
    [SerializeField] DisableFromArray disableFromArray;
    [SerializeField] EnableFromArray enableFromArray;
    [SerializeField] ReverbManager reverbManager;
    public int finished;

    void OnTriggerEnter(Collider other){
        if (finished == 1) return;
        finished = 1;
        StartCoroutine(Sequence());
    }

    void Setup(){
        disableFromArray.DisableAllInArray();
        enableFromArray.EnableAllInArray();
        playerSource.transform.position = new Vector3(-39.7f, 0.4f, 461.6f);
        reverbManager.turnedOn = true;
    }

    IEnumerator Sequence(){
        // INT. PUMPING STATION – NIGHT
        // HARRY NOTE - Player walks through door and gets blasted by explosion (probz behind), then wakes up. Says something like: 
        playerSource.PlayOneShot(explosionClip);
        tannoy1Source.Stop();
        malfunctioningDoorSource.Stop();
        StopCoroutine(guardCombatSequence.enemyCoroutine);
        guardCombatSequence.enabled = false;
        controls.inCombat = false;
        StartCoroutine(audioController.FadeMusic());
        darkVsLight.playerDarkness -= 1;
        controls.inCutscene = true;
        pBFootstepSystem.canRotate = false;
        pBFootstepSystem.footstepClips = playerFootstepClips;
        yield return new WaitForSeconds(explosionClip.length);
        StartCoroutine(audioController.ReduceMasterCutOff(10f));
        blockPumpingStationEntranceObject.SetActive(true);
        yield return new WaitForSeconds(15f);
        Setup();
        StartCoroutine(audioController.IncreaseMasterCutOff(10f));
        yield return new WaitForSeconds(12f);
        // Protag
        // Aggh that was rough! Still gotta find those docs, and follow the tannoy to get there. Alright, gotta find the overseer’s office, straight down these stairs. 
        playerSource.PlayOneShot(playerClip);
        yield return new WaitForSeconds(playerClip.length);
        StartCoroutine(tannoyOne.Sequence());
        StartCoroutine(tannoyTwo.Sequence());
        StartCoroutine(tannoyThree.Sequence());
        controls.inCutscene = false;
        controls.canZoom = true;
        pBFootstepSystem.canRotate = true;
        pBFootstepSystem.canMove = true;
    }


}
