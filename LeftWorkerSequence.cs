using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using DearVR;

public class LeftWorkerSequence : MonoBehaviour
{
    [SerializeField] AudioSource tannoySource, trappedWorkerSource, protagSource, protagActionSource, protagReverbSource, finnSource, musicSource, alarmSource, doorSource, doorToDesertSource;
    [SerializeField] DearVRSource tannoyVRSource, doorVRSource, trappedWorkerVRSource, protagReverbVRSource, finnVRSource, doorToDesertVRSource, alarmVRSource;
    [SerializeField] AudioSource[] computerSources;
    [SerializeField] AudioClip[] tannoyClips, protagClips, finnClips;
    [SerializeField] AudioClip flashbackClip, trappedWorkerScream, alarmClip, doorSlideClip, overseersOfficeExitClip, quadbikePullUpClip, outroThemeClip, windSweetener, creditsComputer, creditsMobile;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] AudioController audioController;
    [SerializeField] SaveAndLoadPumpingStation saveAndLoadPumpingStation;
    [SerializeField] AmbienceRepeater quadbikeAmbienceRepeater, desertAmbienceRepeater, pumpingStationAmbienceRepeater;
    [SerializeField] GameObject officeDoorObject, doorBlocker;
    [SerializeField] ReverbManager reverbManager;
    [SerializeField] AudioMixer outOfWorldMixer;
    public int finished;
    float outOfWorldVol;
    bool moveDoor, moveOverseerOfficeDoor, creditsStarted, reduceOutOfWorldVol;
    [SerializeField] CheckPSPosition checkPSPosition;

    void FixedUpdate(){
        if (moveDoor) doorSource.gameObject.transform.position += new Vector3(0, 0, 0.04f);
        if (moveOverseerOfficeDoor) officeDoorObject.transform.position += new Vector3(0, 0, 0.08f);
        if (reduceOutOfWorldVol){
            outOfWorldVol -= 4f;
            outOfWorldMixer.SetFloat("OutOfWorldSource_Vol", outOfWorldVol);
        }

    }

    void PlayOneShotWithVerb(AudioClip clip){
        protagSource.PlayOneShot(clip);
        protagReverbVRSource.DearVRPlayOneShot(clip);
    }

    public IEnumerator Sequence(){
        if (finished == 1) yield break;
        foreach(AudioSource computerSource in computerSources){
            computerSource.gameObject.SetActive(true);
        } 
        darkVsLight.playerDarkness += 1;
        // As the player chooses to leave Shaun, the overseer’s tannoy can be heard in between the falling station. 
        // Protag
        // No-time, didn’t come here to save the world.
        PlayOneShotWithVerb(protagClips[0]);
        yield return new WaitForSeconds(protagClips[0].length);
        doorSource.gameObject.SetActive(true);
        tannoySource.gameObject.SetActive(true);
        tannoySource.clip = tannoyClips[0];
        tannoySource.loop = true;
        tannoyVRSource.DearVRPlay();
        doorVRSource.DearVRPlayOneShot(doorSlideClip);
        moveDoor = true;
        yield return new WaitForSeconds(doorSlideClip.length);
        moveDoor = false;
        doorSource.gameObject.SetActive(false);
    }

    public IEnumerator PlayerInOffice(){
        trappedWorkerVRSource.DearVRPlayOneShot(trappedWorkerScream);
        yield return new WaitForSeconds(trappedWorkerScream.length);
        audioController.SetParameter("PumpingStationAmbience_Vol", -15f);
        // Player moves towards the sound
        // Player enters the trigger box in the office
        // Tannoy changes clip and plays
        reverbManager.inOffice = true;
        tannoyVRSource.DearVRStop();
        tannoyVRSource.DearVRPlayOneShot(tannoyClips[1]);
        yield return new WaitForSeconds(tannoyClips[1].length);
        // The player moves into the office, led by the repeated, but somewhat inaudible, tannoy sequence. Maybe there’s a time-based fail state… maybe… 
        // As the player moves into the office, the door closes behind him/her, cutting off the previous sound environment, but rubble can be heard falling (low-passed), 
        // and maybe a Shaun scream. The Tannoy then speaks again. 
        // Tannoy
        // Welcome overseer. Due to evacuation of this facility, all files and vital data have been removed. Please contact security force 9 to retrieve them. 
        finnSource.gameObject.SetActive(true);
        finnSource.gameObject.transform.position = new Vector3(140.3f, 0, 973.6f);
        // The alarm also seals the overseer’s door, meaning the trapped worker remains helpless. 
        // As the protag leaves the overseer's office and into the wasteland, Finn pulls up. 
        yield return new WaitForSeconds(3f);
        finnVRSource.DearVRPlayOneShot(quadbikePullUpClip);
        quadbikeAmbienceRepeater.turnedOn = true;
        StartCoroutine(quadbikeAmbienceRepeater.ambienceCoroutine);
        // Music is triggered as the sequence of escape begins. 
        // Finn
        // Over here ya eediot! Hurry up; we haven’t got long before those stupid moths turn up! 
        finnVRSource.DearVRPlayOneShot(finnClips[0]);
        yield return new WaitForSeconds(finnClips[0].length);
    }

    public IEnumerator ProtagGoesOutside(){
        doorToDesertSource.gameObject.SetActive(true);
        doorToDesertVRSource.DearVRPlayOneShot(overseersOfficeExitClip);
        yield return new WaitForSeconds(overseersOfficeExitClip.length / 2);
        protagActionSource.PlayOneShot(windSweetener);
        pumpingStationAmbienceRepeater.StopAllSources();
        pumpingStationAmbienceRepeater.gameObject.SetActive(false);
        desertAmbienceRepeater.gameObject.SetActive(true);
        desertAmbienceRepeater.PlayAllSources();
        protagSource.transform.position += new Vector3(4f, 0, 0);
        yield return new WaitForSeconds(doorSlideClip.length / 2);
        foreach(AudioSource computerSource in computerSources)computerSource.Stop();
        moveOverseerOfficeDoor = false;
        doorToDesertSource.gameObject.SetActive(false);
        // Protag’s consciousness is fading, and so the next line 
        // Protag
        // Finn, I couldn’t find… AGHH, I couldn’t find the docs… 
        protagSource.PlayOneShot(protagClips[1]);
        controls.inCutscene = true;
        controls.canZoom = false;
        pBFootstepSystem.canMove = false;
        pBFootstepSystem.canRotate = false;
        StartCoroutine(audioController.ReduceExposedParameterCutOff("OtherSources_CutOff", 5f));
        yield return new WaitForSeconds(3f);
        // ----------------------------------
        // Psychotic episode #1
        // Suddenly, audio denotes anther flashback type sequence, the audio is harder and more unforgiving than the director flashbacks. The protagonist is younger here and appears to be in an argument with one of his brothers. 
        protagActionSource.PlayOneShot(flashbackClip);
        yield return new WaitForSeconds(2f);
        checkPSPosition.turnedOn = false;
        checkPSPosition.musicSource.Stop();
        alarmVRSource.DearVRStop();

        quadbikeAmbienceRepeater.StopAllSources();
        yield return new WaitForSeconds(flashbackClip.length - 4f);
        // Charlie
        // I knew it, I knew it was you! 

        // Protag 
        // Charlie, no it wasn’t, I didn’t do anything I swear! 

        // Charlie 
        // How could you do this to us?! You killed us… you killed us… little brother.

        // Protag 
        // No Charlie! Not through there, its not safe! 

        // Charlie jumps onto a metal beam, the beam collapses and Charlie falls down in a flurry of metal clanging He screams, his scream, fades into the audio fading back to normality.  

        // Charlie 
        // Aghhh 

        // Protag 
        // Charlieeee

        // Audio reverses back to normality. The protag is in a haze and somewhat unconscious, Finn peers over him and remarks on the events he saw. 
        // ----------------------------------
        StartCoroutine(audioController.IncreaseExposedParameterCutOff("OtherSources_CutOff", 5f));
        audioController.SetParameter("DesertAmbience_Vol", 0f);
        yield return new WaitForSeconds(8f);
        // Finn 
        // By the night, come on. Let’s get you fixed up. 
        finnVRSource.DearVRPlayOneShot(finnClips[1]);
        yield return new WaitForSeconds(finnClips[1].length);

        StartCoroutine(audioController.ReduceExposedParameterCutOff("OtherSources_CutOff", 5f));
        StartCoroutine(audioController.FadeMusic());
        // Big noise-based swooshes fade into the outro theme, marking the end of the first episode. The outro fade into music should be haunting and dark, maybe some screams layered with swooshes and other natural sounds. 
        // BACK TO MENU!
        musicSource.PlayOneShot(outroThemeClip);
        yield return new WaitForSeconds(outroThemeClip.length);

        creditsStarted = true;

        if (controls.computer){
            musicSource.PlayOneShot(creditsComputer);
            yield return new WaitForSeconds(creditsComputer.length);
        } else {
            musicSource.PlayOneShot(creditsMobile);
            yield return new WaitForSeconds(creditsMobile.length);
        }

        StartCoroutine(Finished());
    }

    IEnumerator Finished(){
        reduceOutOfWorldVol = true;
        yield return new WaitForSeconds(3f);
        finished = 1;
        saveAndLoadPumpingStation.FinishedPumpingStation();
        yield return new WaitForSeconds(1f);
        saveAndLoadPumpingStation.LoadMainMenu();
    }

    void Update(){
        if ((controls.doubleTap||controls.doubleTapSpace) && creditsStarted){
            creditsStarted = false;
            StartCoroutine(Finished());
        }
    }
}
