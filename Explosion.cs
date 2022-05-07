using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DearVR;

public class Explosion : MonoBehaviour
{
    [SerializeField] AudioSource playerSource, playerActionSource, pumpingStationSource, tannoy1Source, malfunctioningDoorSource, playerReverbSource;
    [SerializeField] AmbienceRepeater desertAmbienceRepeater, pumpingStationAmbienceRepeater;
    [SerializeField] AudioClip playerClip, stopClip, tanoyDieingClip, malfunctioningDoorDieing, explosionClip, postExplosionSweetener;
    [SerializeField] AudioClip[] playerFootstepClips;
    [SerializeField] GuardCombatSequence guardCombatSequence;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] AudioController audioController;
    [SerializeField] TurnOffOnEnter tannoyOne, tannoyTwo, tannoyThree;
    [SerializeField] GameObject blockPumpingStationEntranceObject, changeFootstepsObject;
    [SerializeField] DisableFromArray disableFromArray;
    [SerializeField] EnableFromArray afterExplosionEnableFromArray;
    [SerializeField] ReverbManager reverbManager;
    [SerializeField] DesertBoundary desertBoundary;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] SaveAndLoadPumpingStation saveAndLoadPumpingStation;
    public int finished;

    void PlayOneShotWithVerb(AudioClip clip){
        playerSource.PlayOneShot(clip);
        playerReverbSource.PlayOneShot(clip, 0.4f);
    }

    void OnTriggerEnter(Collider other){
        if (finished == 1) return;
        finished = 1;
        StartCoroutine(Sequence());
    }

    void Setup(){
        disableFromArray.DisableAllInArray();
        afterExplosionEnableFromArray.EnableAllInArray();
        playerSource.transform.position = new Vector3(-38.0f, 0.3f, 477f);
        reverbManager.turnedOn = true;
    }

    IEnumerator Sequence(){
        controls.inCombat = false;
        controls.inStealth = false;
        StopCoroutine(guardCombatSequence.enemyCoroutine);
        guardCombatSequence.enabled = false;
        StartCoroutine(audioController.FadeMusic());
        yield return new WaitForSeconds(2f);
        malfunctioningDoorSource.Stop();
        desertBoundary.turnedOn = false;
        desertBoundary.gameObject.SetActive(false);
        // INT. PUMPING STATION – NIGHT
        // HARRY NOTE - Player walks through door and gets blasted by explosion (probz behind), then wakes up. Says something like: 
        tannoy1Source.Stop();
        tannoy1Source.PlayOneShot(tanoyDieingClip);
        malfunctioningDoorSource.Stop();
        malfunctioningDoorSource.PlayOneShot(malfunctioningDoorDieing);
        playerActionSource.PlayOneShot(explosionClip);
        darkVsLight.playerDarkness -= 1;
        pBFootstepSystem.canRotate = false;
        controls.inCutscene = true;
        pBFootstepSystem.footstepClips = playerFootstepClips;
        yield return new WaitForSeconds(explosionClip.length - 18f);
        pumpingStationSource.Stop();
        malfunctioningDoorSource.Stop();
        audioMixer.SetFloat("DesertAmbience_Vol", -80f);
        yield return new WaitForSeconds(16f);
        StartCoroutine(audioController.ReduceMasterCutOff(2f));
        blockPumpingStationEntranceObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        Setup();
        StartCoroutine(audioController.IncreaseMasterCutOff(10f));
        yield return new WaitForSeconds(2f);
        PlayOneShotWithVerb(postExplosionSweetener);
        changeFootstepsObject.SetActive(false);
        yield return new WaitForSeconds(10f);
        // Protag
        // Aggh that was rough! Still gotta find those docs, and follow the tannoy to get there. Alright, gotta find the overseer’s office, straight down these stairs. 
        PlayOneShotWithVerb(playerClip);
        yield return new WaitForSeconds(playerClip.length);
        saveAndLoadPumpingStation.SavePumpingStation();
        StartCoroutine(tannoyOne.Sequence());
        StartCoroutine(tannoyTwo.Sequence());
        StartCoroutine(tannoyThree.Sequence());
        audioMixer.SetFloat("Footsteps_Vol", 0f);
        controls.inCutscene = false;
        controls.canZoom = true;
        pBFootstepSystem.canRotate = true;
        pBFootstepSystem.canMove = true;
    }
}
