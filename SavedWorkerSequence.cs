using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DearVR;

public class SavedWorkerSequence : SequenceBase
{
    [SerializeField] AudioSource protagSource, protagReverbSource, protagActionSource, factoryWorkerSource, doorSource, desertWindSource, finnSource, musicSource, outOfWorldSource;
    [SerializeField] DearVRSource protagReverbVRSource, factoryWorkerVRSource, doorVRSource, desertWindVRSource, finnVRSource;
    [SerializeField] AudioClip[] protagClips, factoryWorkerClips, finnClips, factoryWorkerLoopClips, sandFootsteps, playerSandFootsteps;
    [SerializeField] AudioClip rifleButtClip, rubbleClip, workerCoughingClip, desertWindClip, suckInClip, desertWindSweetenerClip, savedWorkerExertionClip, 
    explosionClip, quadbikeClip, doorBeepClip, doorOpenClip, outroThemeClip, creditsComputer, creditsMobile, factoryWorkerHitsFloorClip, gottaMoveToMeClip;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] SaveAndLoadPumpingStation saveAndLoadPumpingStation;
    [SerializeField] AudioController audioController;
    [SerializeField] AmbienceRepeater collapseRepeater, pumpingStationRepeater, desertRepeater;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] GameObject desertDoor, desertAudioSourcesObject;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] ReverbManager reverbManager;
    float outOfWorldVol;
    [SerializeField] AudioMixer outOfWorldMixer;
    public bool hasReachedExit;
    public bool hasStarted, moveShaunOutside;
    bool moveShaunToDoor, playerOutside, moveFinnToPlayer, creditsStarted, reduceOutOfWorldVol;    
    int factoryWorkerI;
    [SerializeField] EnemyFootsteps trappedWorkerFootsteps;

    void PlayOneShotWithVerb(AudioClip clip){
        protagSource.PlayOneShot(clip);
        protagReverbSource.volume = 0.4f;
        protagReverbVRSource.DearVRPlayOneShot(clip);
    }

    void Start(){ 
        active = 0;
    }

    public override IEnumerator Sequence(){
        if (finished == 1 || active == 0) yield break;
        hasStarted = true;
        darkVsLight.playerDarkness -= 1;
        protagSource.transform.LookAt(factoryWorkerSource.transform.position);
        pBFootstepSystem.canRotate = false;
        controls.inCutscene = true;
        // Protag
        // Aghh FINE!
        PlayOneShotWithVerb(protagClips[0]);
        yield return new WaitForSeconds(protagClips[0].length);
        // Player lifts up rubble/clears rubble and free’s Shaun. 
        PlayOneShotWithVerb(rubbleClip);
        factoryWorkerVRSource.DearVRPlayOneShot(savedWorkerExertionClip);
        yield return new WaitForSeconds(rubbleClip.length);
        factoryWorkerVRSource.DistanceCorrection = 0.35f;
        pBFootstepSystem.canRotate = true;
        controls.inCutscene = false;
        factoryWorkerVRSource.DearVRPlayOneShot(workerCoughingClip);
        desertAudioSourcesObject.SetActive(true);
        desertWindSource.loop = true;
        desertWindSource.clip = desertWindClip;
        desertWindSource.Play();
        yield return new WaitForSeconds(2f);
        moveShaunToDoor = true;
        finnSource.gameObject.SetActive(true);
        // The pair dash for an exit - CREATE AN EXIT SOUND
        // The pair exit and stand outside the collapsing station entrance. They are both out of breath and panicked somewhat. 
        // Finally, they make it out of station through a nearby door (cutscene – audio MUST be well directed to justify cut scenes) – rubble falling around them, industrial moans, panicked breathe, Shaun in pain etc. 
    }

    void FixedUpdate(){
        if (moveShaunToDoor){
            if (Vector3.Distance(factoryWorkerSource.transform.position, doorSource.transform.position) > 2){
                factoryWorkerSource.transform.position = Vector3.MoveTowards(factoryWorkerSource.transform.position, doorSource.transform.position, 0.1f);
            } else {
                moveShaunToDoor = false;
                StartCoroutine(OpenDoor());
            }
        } else if (moveShaunOutside){
            if (Vector3.Distance(factoryWorkerSource.transform.position, desertWindSource.transform.position) > 2){
                factoryWorkerSource.transform.position = Vector3.MoveTowards(factoryWorkerSource.transform.position, desertWindSource.transform.position, 0.1f);
            } else {
                moveShaunOutside = false;
                StartCoroutine(ShaunOutside());
            }
        }

        if (moveFinnToPlayer){
            if (Vector3.Distance(finnSource.transform.position, protagSource.transform.position) > 4){
                finnSource.transform.position = Vector3.MoveTowards(finnSource.transform.position, protagSource.transform.position, 0.15f);
            } else {
                moveFinnToPlayer = false;
            }
        }

        if (reduceOutOfWorldVol){
            outOfWorldVol -= 4f;
            outOfWorldMixer.SetFloat("OutOfWorldSource_Vol", outOfWorldVol);
        }
    }

    void RemoveReverbAndPSSources(){
        reverbManager.turnedOn = false;
        audioMixer.SetFloat("PumpingStationVerbOne_Vol", -80f);
        audioMixer.SetFloat("PumpingStationVerbThree_Vol", -80f);
        pumpingStationRepeater.StopAllSources();
        desertRepeater.gameObject.SetActive(true);
        StartCoroutine(desertRepeater.ambienceCoroutine);
    }

    public IEnumerator OpenDoor(){
        doorSource.PlayOneShot(doorBeepClip);
        yield return new WaitForSeconds(doorBeepClip.length);
        doorSource.PlayOneShot(doorOpenClip);
        yield return new WaitForSeconds(doorOpenClip.length - 1f);
        factoryWorkerVRSource.DearVRPlayOneShot(gottaMoveToMeClip);
        yield return new WaitForSeconds(gottaMoveToMeClip.length);
        desertDoor.SetActive(false);
        yield return new WaitForSeconds(1f);
        trappedWorkerFootsteps.footstepClips = sandFootsteps;
        factoryWorkerVRSource.ReverbLevel = -80f;
        moveShaunOutside = true;
    }

    IEnumerator ShaunOutside(){
        if (playerOutside) yield break;
        factoryWorkerVRSource.DearVRPlayOneShot(factoryWorkerLoopClips[factoryWorkerI]);
        // if (factoryWorkerI < (factoryWorkerClips.Length - 1)){
        //     factoryWorkerI += 1;
        // } else {
        //     factoryWorkerI = 0;
        // }
        yield return new WaitForSeconds(10f);
        StartCoroutine(ShaunOutside());
    }

    public IEnumerator SequenceContinued(){
        playerOutside = true;
        pBFootstepSystem.footstepClips = playerSandFootsteps;
        protagActionSource.PlayOneShot(suckInClip);
        yield return new WaitForSeconds(suckInClip.length);
        audioController.SetVolumeToNothing("Master_Vol");
        RemoveReverbAndPSSources();
        StartCoroutine(audioController.FadeMusic());
        yield return new WaitForSeconds(0.5f);
        factoryWorkerSource.Stop();
        musicSource.Stop();
        audioController.SetVolumeToZero("Master_Vol");
        audioMixer.SetFloat("Music_Vol", 0f);
        protagActionSource.PlayOneShot(explosionClip);
        protagSource.transform.position += new Vector3(0, 0, 10f);
        controls.inCutscene = true;
        controls.canZoom = false;
        yield return new WaitForSeconds(explosionClip.length);
        factoryWorkerSource.transform.position = new Vector3(protagSource.transform.position.x - 7f, 0.3f, protagSource.transform.position.z);
        finnSource.gameObject.transform.position = new Vector3(protagSource.transform.position.x - 3f, 0.3f, protagSource.transform.position.z + 100f);
        protagActionSource.PlayOneShot(desertWindSweetenerClip);
        yield return new WaitForSeconds(5f);
        // The two emerge, falling building sounds behind them. They get up, brush dust and debris off of them, panting.

        // Factory worker speaks the latter half of the line as though he is shouting at them, and they can hear them. 
        // Factory Worker
        // By the Light, I owe you my life. Certainly, more than those clippers who left me here to die. 
        factoryWorkerVRSource.DearVRPlayOneShot(factoryWorkerClips[0]);
        yield return new WaitForSeconds(factoryWorkerClips[0].length);

        // Protag 
        // Yeah, well, you just ruined my day… you owe me… 
        protagSource.PlayOneShot(protagClips[1]);
        yield return new WaitForSeconds(protagClips[1].length);

        // Factory Worker speaks in humorous, desperate hope for the first sentence. 
        // Factory Worker
        // Kindness is the glue that holds us all together? No? 
        factoryWorkerVRSource.DearVRPlayOneShot(factoryWorkerClips[1]);
        yield return new WaitForSeconds(factoryWorkerClips[1].length);

        // Protag
        // No.
        protagSource.PlayOneShot(protagClips[2]);
        yield return new WaitForSeconds(protagClips[2].length);

        // Factory Worker
        // Alright, alright… If you’re heading to Dusklight, and you find yourself in district 5, listen out for Slater, say my name and he’ll hook you up with something… 
        factoryWorkerVRSource.DearVRPlayOneShot(factoryWorkerClips[2]);
        yield return new WaitForSeconds(factoryWorkerClips[2].length);


        // Protag
        // Name? 
        protagSource.PlayOneShot(protagClips[3]);
        yield return new WaitForSeconds(protagClips[3].length);

// ____________
        moveFinnToPlayer = true;
        finnVRSource.DearVRPlayOneShot(quadbikeClip);
// ____________

        // Factory replies in a stumbly manner but speaks with a robotic voice when reading his worker number. 
        // Factory Worker 
        // Oh right, yeah, its worker 059324… but you can call me Shaun. You have that stench of a failed Nightlander rebel about you, and Slater has plenty of dirt on The Church so ask for that! 
        factoryWorkerVRSource.DearVRPlayOneShot(factoryWorkerClips[3]);
        yield return new WaitForSeconds(factoryWorkerClips[3].length);
        
        // Protag
        // Alri…
        protagSource.PlayOneShot(protagClips[4]);
        yield return new WaitForSeconds(protagClips[4].length);

        // Finns quadbike, after being heard approaching for the last 5 seconds, pulls up sharply. He speaks immediately, at first over the sound of the engine. 
        // Finn 
        // What in the name are you doing, agent? Who the hell is this?! 
        finnVRSource.DearVRPlayOneShot(finnClips[0]);
        yield return new WaitForSeconds(finnClips[0].length);

        // Protag 
        // He’s a worker… 
        protagSource.PlayOneShot(protagClips[5]);
        yield return new WaitForSeconds(protagClips[5].length);

        // Finn 
        // He’s a moth is who he is! You let the docs go because of this?!
        finnVRSource.DearVRPlayOneShot(finnClips[1]);
        yield return new WaitForSeconds(finnClips[1].length);

        // Protag 
        // About that… 
        protagSource.PlayOneShot(protagClips[6]);
        yield return new WaitForSeconds(protagClips[6].length);

        // Shaun 
        // Hey man, I gotta work to put food on my table, Church or no Church! 
        factoryWorkerVRSource.DearVRPlayOneShot(factoryWorkerClips[4]);
        yield return new WaitForSeconds(factoryWorkerClips[4].length);

        // Finn 
        // I know you gotta do what you gotta do, pal… but so do we. 
        finnVRSource.DearVRPlayOneShot(finnClips[2]);
        yield return new WaitForSeconds(finnClips[2].length);

        // Shaun 
        // I don’t wanna get in the midd… 
        factoryWorkerVRSource.DearVRPlayOneShot(factoryWorkerClips[5]);
        yield return new WaitForSeconds(factoryWorkerClips[5].length - 1.2f);
        finnSource.transform.position = factoryWorkerSource.transform.position;
        finnVRSource.DearVRPlayOneShot(rifleButtClip);
        yield return new WaitForSeconds(rifleButtClip.length - 1.0f);
        factoryWorkerVRSource.DearVRPlayOneShot(factoryWorkerHitsFloorClip);
        yield return new WaitForSeconds(factoryWorkerHitsFloorClip.length);
        // Finn 
        // Let’s hope there’s a silver lining here, lets hope we can get something out of him, eh? 
        finnVRSource.DearVRPlayOneShot(finnClips[3]);
        yield return new WaitForSeconds(finnClips[3].length);

        // Protag
        // Why else do you think I saved him? Besides, those docs would have been long gone, that place was evacuated, Finn. 
        protagSource.PlayOneShot(protagClips[7]); // MISSING CLIPS
        yield return new WaitForSeconds(protagClips[7].length - 0.4f);

        // Finn let’s out an angry burst, this burst is a horrific fit of anger, revealing the true desperation and viciousness of Finn, breaking any image of a clown/soft character that may have been portrayed prior. 
        // Finn 
        // DAMMIT
        finnVRSource.DearVRPlayOneShot(finnClips[4]);
        yield return new WaitForSeconds(finnClips[4].length);

        // Any heavy wind audio or other prominent sources fade slightly for tension. 
        // Finn 
        // I don’t think you quite understand the importance of what we’re doing here. Do you know what we’ve been through? The Things we’ve had to do, to rid the city of those devils? 
        finnVRSource.DearVRPlayOneShot(finnClips[5]);
        yield return new WaitForSeconds(finnClips[5].length);

        // The protag lunges forward (or some foot work) to denote intense posture change. 
        // Protag 
        // I’m here for the same reason as you. The same reason that any of us are doing any of this - to escape. To stop our pasts from destroying our futures, to make sure we don’t mess up again.
        protagSource.PlayOneShot(protagClips[8]);
        yield return new WaitForSeconds(protagClips[8].length);

        // Finn sounds almost nervous here, certainly a bit shakey from anger. 
        // Finn 
        // Oh yeah?
        finnVRSource.DearVRPlayOneShot(finnClips[6]);
        yield return new WaitForSeconds(finnClips[6].length);

        // Protag
        // Yeah. And trust me, you don’t want be on my list of mistakes.
        protagSource.PlayOneShot(protagClips[9]);
        yield return new WaitForSeconds(protagClips[9].length);

        // Finn 
        // Really?
        finnVRSource.DearVRPlayOneShot(finnClips[7]);
        yield return new WaitForSeconds(finnClips[7].length);

        // Protag
        // *exhale*
        protagSource.PlayOneShot(protagClips[10]);
        yield return new WaitForSeconds(protagClips[10].length);

        // Finn
        // Fine!
        finnVRSource.DearVRPlayOneShot(finnClips[8]);
        yield return new WaitForSeconds(finnClips[8].length);

        // Finn get’s back on the quadbike. Music rises in to end of the utterance. (maybe quad bike starts up as exit music kicks in)
        // Finn
        // I guess we’ll head back, see what’s hiding this one’s lungs, and then… then we go to Dusklight… 
        finnVRSource.DearVRPlayOneShot(finnClips[9]);
        yield return new WaitForSeconds(finnClips[9].length);
        
        StartCoroutine(audioController.ReduceExposedParameterCutOff("OtherSources_CutOff", 5f));
        
        outOfWorldSource.PlayOneShot(outroThemeClip);
        yield return new WaitForSeconds(outroThemeClip.length);

        creditsStarted = true;

        if (controls.computer){
            outOfWorldSource.PlayOneShot(creditsComputer);
            yield return new WaitForSeconds(creditsComputer.length);
        } else {
            outOfWorldSource.PlayOneShot(creditsMobile);
            yield return new WaitForSeconds(creditsMobile.length);
        }

        StartCoroutine(Finished());
    }

    IEnumerator Finished(){
        reduceOutOfWorldVol = true;
        yield return new WaitForSeconds(3f);
        finished = 1;        
        saveAndLoadPumpingStation.FinishedPumpingStation();
        saveAndLoadPumpingStation.LoadMainMenu();
    }

    void Update(){
        if ((controls.doubleTap||controls.doubleTapSpace) && creditsStarted){
            creditsStarted = false;
            StartCoroutine(Finished());
        }
    }
}
