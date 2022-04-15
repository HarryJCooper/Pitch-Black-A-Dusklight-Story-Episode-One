using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeftWorkerSequence : MonoBehaviour
{
    [SerializeField] AudioSource tannoySource, trappedWorkerSource, protagSource, protagReverbSource, finnSource, musicSource, securityForcesSource, alarmSource, doorSource;
    [SerializeField] AudioClip[] tannoyClips, protagClips, finnClips;
    [SerializeField] AudioClip flashbackClip, trappedWorkerScream, alarmClip, securityForcesClip, doorSlideClip, quadbikePullUpClip, outroThemeClip;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] AudioController audioController;
    [SerializeField] SaveAndLoadPumpingStation saveAndLoadPumpingStation;
    [SerializeField] AmbienceRepeater quadbikeAmbienceRepeater;
    public int finished;

    void PlayOneShotWithVerb(AudioClip clip){
        protagSource.PlayOneShot(clip);
        protagReverbSource.PlayOneShot(clip);
    }

    public IEnumerator Sequence(){
        if (finished == 1) yield break;
        darkVsLight.playerDarkness += 1;
        // As the player chooses to leave Shaun, the overseer’s tannoy can be heard in between the falling station. 
        // Protag
        // No-time, didn’t come here to save the world.
        PlayOneShotWithVerb(protagClips[0]);
        yield return new WaitForSeconds(protagClips[0].length);
        PlayTannoy();
    }

    void PlayTannoy(){
        // Tannoy starts playing
        StartCoroutine(PlayerInOffice());
    }
    
        

    public IEnumerator PlayerInOffice(){
        // Player moves towards the sound
        // Player enters the trigger box in the office
        // Tannoy changes clip and plays
        
        // The player moves into the office, led by the repeated, but somewhat inaudible, tannoy sequence. Maybe there’s a time-based fail state… maybe… 
        // As the player moves into the office, the door closes behind him/her, cutting off the previous sound environment, but rubble can be heard falling (low-passed), 
        // and maybe a Shaun scream. The Tannoy then speaks again. 
        // Tannoy
        // Welcome overseer. Due to evacuation of this facility, all files and vital data have been removed. Please contact security force 9 to retrieve them. 
        tannoySource.PlayOneShot(tannoyClips[0]);
        yield return new WaitForSeconds(tannoyClips[0].length);
        yield return new WaitForSeconds(1f);
        StartCoroutine(SecurityForces());
    }
    
    public IEnumerator SecurityForces(){
        yield return new WaitForSeconds(1f);
        // the sound of security forces in the distance
        securityForcesSource.PlayOneShot(securityForcesClip);
        // combined with the triggering of an alarm can be heard. 
        alarmSource.PlayOneShot(alarmClip);
        // This alarm triggers a side door that leads from the overseer’s office into a small corridor that then leads to the desert wasteland. 
        doorSource.PlayOneShot(doorSlideClip);
        yield return new WaitForSeconds(doorSlideClip.length);
        // The alarm also seals the overseer’s door, meaning the trapped worker remains helpless. 
        
        // As the protag leaves the overseer's office and into the wasteland, Finn pulls up. 
        finnSource.PlayOneShot(quadbikePullUpClip);
        StartCoroutine(quadbikeAmbienceRepeater.ambienceCoroutine);
        // Music is triggered as the sequence of escape begins. 

        // Finn
        // Over here ya eediot! Hurry up; we haven’t got long before those stupid moths turn up! 
        finnSource.PlayOneShot(finnClips[0]);
        yield return new WaitForSeconds(finnClips[0].length);

        // Protag’s consciousness is fading, and so the next line 
        // Protag
        // Finn, I couldn’t find… AGHH, I couldn’t find the docs… 
        protagSource.PlayOneShot(protagClips[1]);
        yield return new WaitForSeconds(protagClips[1].length);
    
        controls.inCutscene = true;
        pBFootstepSystem.canMove = false;
        pBFootstepSystem.canRotate = false;
        StartCoroutine(audioController.ReduceMasterCutOff(8f));
        yield return new WaitForSeconds(8f);
        // ----------------------------------
        // Psychotic episode #1
        // Suddenly, audio denotes anther flashback type sequence, the audio is harder and more unforgiving than the director flashbacks. The protagonist is younger here and appears to be in an argument with one of his brothers. 
        protagSource.PlayOneShot(flashbackClip);
        yield return new WaitForSeconds(flashbackClip.length - 10f);

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
        StartCoroutine(audioController.IncreaseMasterCutOff(8f));
        yield return new WaitForSeconds(8f);
        // Finn 
        // By the night, come on. Let’s get you fixed up. 
        finnSource.PlayOneShot(finnClips[1]);
        yield return new WaitForSeconds(finnClips[1].length);

        StartCoroutine(audioController.ReduceMasterCutOff(8f));
        yield return new WaitForSeconds(8f);
        // Big noise-based swooshes fade into the outro theme, marking the end of the first episode. The outro fade into music should be haunting and dark, maybe some screams layered with swooshes and other natural sounds. 
        // BACK TO MENU!
        musicSource.PlayOneShot(outroThemeClip);
        yield return new WaitForSeconds(outroThemeClip.length);

        finished = 1;
        saveAndLoadPumpingStation.FinishedPumpingStation();
        saveAndLoadPumpingStation.LoadMainMenu();
    }
}
