using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using DearVR;

public class QuadbikeFinishedSequence : SequenceBase
{
    public AudioSourceContainer audioSourceContainer;
    [SerializeField] AudioClip[] protagClips, donnieClips, donnieLoopClips, loopClips, quadbikeLoopClips;
    [SerializeField] AudioClip getOnQuadbikeClip, outroMusicAndClip;
    [SerializeField] AudioController audioController;
    [SerializeField] SaveAndLoadEncampment saveAndLoadEncampment;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] AudioSource outsideRadioSource;
    [SerializeField] AudioMixer audioMixer;
    bool startedLoopRevs;
    int i = 0;

    public void StartSequence(){
        audioSourceContainer.donnieVRSource.DearVRStop();
        StartCoroutine(TriggeredSequence());
    }

    IEnumerator LoopRevs(){
        if (finished == 1) yield break;
        startedLoopRevs = true;
        audioSourceContainer.donnieVRSource.DearVRPlayOneShot(quadbikeLoopClips[Random.Range(0, quadbikeLoopClips.Length)]);
        yield return new WaitForSeconds(quadbikeLoopClips[0].length/2);
        StartCoroutine(LoopRevs());
    }

    public IEnumerator SequenceLoop(){
        if (active == 0) yield break;
        yield return new WaitForSeconds(0.5f);
        audioSourceContainer.donnieVRSource.DearVRStop();
        if (!startedLoopRevs) StartCoroutine(LoopRevs());
        audioSourceContainer.donnieVRSource.DearVRPlayOneShot(donnieLoopClips[i]);
        yield return new WaitForSeconds(15f);
        if (i < 2) i += 1; else i = 0;
        StartCoroutine(SequenceLoop());
    }

    IEnumerator TriggeredSequence(){
        active = 0;
        audioSourceContainer.protagActionSource.PlayOneShot(cutsceneEnterClip);
        yield return new WaitForSeconds(cutsceneEnterClip.length);
        // Player walks to Donnie to get the Quadbike 
        controls.inCutscene = true;
        pBFootstepSystem.canRotate = false;
        // Donnie 
        // Hey loudmouth! Quadbike’s ready; she’s fitted with a nav device, it’ll point you in the right direction just do what she says, know that might be hard for you. 
        audioSourceContainer.donnieVRSource.DearVRPlayOneShot(donnieClips[0]);
        yield return new WaitForSeconds(donnieClips[0].length);

        // Protag mutters under their breath. 
        // Protag 
        // Idiot.
        audioSourceContainer.protagSource.PlayOneShot(protagClips[0]);
        yield return new WaitForSeconds(protagClips[0].length);

        // Donnie replies in an awkward tone. 
        // Donnie 
        // Yeah, I heard that, again… I heard that again. Dammit. 
        audioSourceContainer.donnieVRSource.DearVRPlayOneShot(donnieClips[1]);
        yield return new WaitForSeconds(donnieClips[1].length);

        // Player then follows the sound of the nearby engine, maybe a car unlocked beep triggers too, and interacts to get on it. They then drive to the objective using the sat nav audio prompts. The player reaches a point, and the protag speaks to denote a fade. 
        // Protag 
        // Alright, got a while till the pumping station, then I gotta follow the rest of his trail. This place better be worth it… 
        audioSourceContainer.protagActionSource.PlayOneShot(getOnQuadbikeClip, 0.3f);
        yield return new WaitForSeconds(getOnQuadbikeClip.length);
        audioSourceContainer.protagActionSource.PlayOneShot(outroMusicAndClip, 0.4f);
        yield return new WaitForSeconds(12f);
        audioMixer.SetFloat("OtherSources_Vol", -80f);
        yield return new WaitForSeconds(outroMusicAndClip.length - 24f);
        outsideRadioSource.Stop();
        yield return new WaitForSeconds(6f);
        StartCoroutine(audioController.ReduceMasterCutOff(6f));
        yield return new WaitForSeconds(6f);
        audioController.SetCutOffToZero();
        Finished();
    }

    void Finished(){
        finished = 1;
        saveAndLoadEncampment.SaveEncampment();
        saveAndLoadEncampment.FinishedEncampment();
        StartCoroutine(LoadNextLevel());
    }

    IEnumerator LoadNextLevel(){
        LoadingData.sceneToLoad = "ThePumpingStation";
        LoadingData.hasLoaded = false;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Loading");
        while (!asyncLoad.isDone){
            yield return null;
        }
    }
}
