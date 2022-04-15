using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedWorkerSequence : SequenceBase
{
    [SerializeField] AudioSource protagSource, protagReverbSource, factoryWorkerSource, finnSource, musicSource;
    [SerializeField] AudioClip[] protagClips, factoryWorkerClips, finnClips;
    [SerializeField] AudioClip rifleButtClip, rubbleClip, workerCoughingClip, outroThemeClip;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] SaveAndLoadPumpingStation saveAndLoadPumpingStation;
    [SerializeField] AudioController audioController;
    [SerializeField] AmbienceRepeater collapseRepeater;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    public bool hasReachedExit;
    public bool hasStarted;

    void PlayOneShotWithVerb(AudioClip clip){
        protagSource.PlayOneShot(clip);
        protagReverbSource.PlayOneShot(clip);
    }

    void Start(){ active = 0;}

    public override IEnumerator Sequence(){
        if (finished == 1 || active == 0) yield break;
        hasStarted = true;
        darkVsLight.playerDarkness -= 1;
        protagSource.transform.rotation = Quaternion.RotateTowards(protagSource.transform.rotation, factoryWorkerSource.transform.rotation, 360);
        pBFootstepSystem.canRotate = false;
        controls.inCutscene = true;
        // Protag
        // Aghh FINE!
        PlayOneShotWithVerb(protagClips[0]);
        yield return new WaitForSeconds(protagClips[0].length);
        // Player lifts up rubble/clears rubble and free’s Shaun. 
        PlayOneShotWithVerb(rubbleClip);
        yield return new WaitForSeconds(rubbleClip.length);
        pBFootstepSystem.canRotate = true;
        controls.inCutscene = false;
        factoryWorkerSource.PlayOneShot(workerCoughingClip);
        // The pair dash for an exit - CREATE AN EXIT SOUND
        // The pair exit and stand outside the collapsing station entrance. They are both out of breath and panicked somewhat. 
        // Finally, they make it out of station through a nearby door (cutscene – audio MUST be well directed to justify cut scenes) – rubble falling around them, industrial moans, panicked breathe, Shaun in pain etc. 
    }

    public IEnumerator SequenceContinued(){
        // StartCoroutine(audioController.FadeOutReverbAndReflections());
        yield return new WaitForSeconds(5f);
        // The two emerge, falling building sounds behind them. They get up, brush dust and debris off of them, panting.

        // Factory worker speaks the latter half of the line as though he is shouting at them, and they can hear them. 
        // Factory Worker
        // By the Light, I owe you my life. Certainly, more than those clippers who left me here to die. 
        factoryWorkerSource.PlayOneShot(factoryWorkerClips[0]);
        yield return new WaitForSeconds(factoryWorkerClips[0].length);

        // Protag 
        // Yeah, well, you just ruined my day… you owe me… 
        protagSource.PlayOneShot(protagClips[1]);
        yield return new WaitForSeconds(protagClips[1].length);

        // Factory Worker speaks in humorous, desperate hope for the first sentence. 
        // Factory Worker
        // Kindness is the glue that holds us all together? No? 
        factoryWorkerSource.PlayOneShot(factoryWorkerClips[1]);
        yield return new WaitForSeconds(factoryWorkerClips[1].length);

        // Protag
        // No.
        protagSource.PlayOneShot(protagClips[2]);
        yield return new WaitForSeconds(protagClips[2].length);

        // Factory Worker
        // Alright, alright… If you’re heading to Dusklight, and you find yourself in district 5, listen out for Slater, say my name and he’ll hook you up with something… 
        factoryWorkerSource.PlayOneShot(factoryWorkerClips[2]);
        yield return new WaitForSeconds(factoryWorkerClips[2].length);

        // Protag
        // Name? 
        protagSource.PlayOneShot(protagClips[3]);
        yield return new WaitForSeconds(protagClips[3].length);

        // Factory replies in a stumbly manner but speaks with a robotic voice when reading his worker number. 
        // Factory Worker 
        // Oh right, yeah, its worker 059324… but you can call me Shaun. You have that stench of a failed Nightlander rebel about you, and Slater has plenty of dirt on The Church so ask for that! 
        factoryWorkerSource.PlayOneShot(factoryWorkerClips[3]);
        yield return new WaitForSeconds(factoryWorkerClips[3].length);

        // Protag
        // Alri…
        protagSource.PlayOneShot(protagClips[4]);
        yield return new WaitForSeconds(protagClips[4].length);

        // Finns quadbike, after being heard approaching for the last 5 seconds, pulls up sharply. He speaks immediately, at first over the sound of the engine. 
        // Finn 
        // What in the name are you doing, agent? Who the hell is this?! 
        finnSource.PlayOneShot(finnClips[0]);
        yield return new WaitForSeconds(finnClips[0].length);

        // Protag 
        // He’s a worker… 
        protagSource.PlayOneShot(protagClips[5]);
        yield return new WaitForSeconds(protagClips[5].length);

        // Finn 
        // He’s a moth is who he is! You let the docs go because of this?!
        finnSource.PlayOneShot(finnClips[1]);
        yield return new WaitForSeconds(finnClips[1].length);

        // Protag 
        // About that… 
        protagSource.PlayOneShot(protagClips[6]);
        yield return new WaitForSeconds(protagClips[6].length);

        // Shaun 
        // Hey man, I gotta work to put food on my table, Church or no Church! 
        factoryWorkerSource.PlayOneShot(factoryWorkerClips[4]);
        yield return new WaitForSeconds(factoryWorkerClips[4].length);

        // Finn 
        // I know you gotta do what you gotta do, pal… but so do we. 
        finnSource.PlayOneShot(finnClips[2]);
        yield return new WaitForSeconds(finnClips[2].length);

        // Shaun 
        // I don’t wanna get in the midd… 
        factoryWorkerSource.PlayOneShot(factoryWorkerClips[5]);
        yield return new WaitForSeconds(factoryWorkerClips[5].length);
        finnSource.PlayOneShot(rifleButtClip);
        yield return new WaitForSeconds(rifleButtClip.length);
        // Finn 
        // Let’s hope there’s a silver lining here, lets hope we can get something out of him, eh? 
        finnSource.PlayOneShot(finnClips[3]);
        yield return new WaitForSeconds(finnClips[3].length);

        // Protag
        // Why else do you think I saved him? Besides, those docs would have been long gone, that place was evacuated, Finn. 
        protagSource.PlayOneShot(protagClips[7]);
        yield return new WaitForSeconds(protagClips[7].length);

        // Finn let’s out an angry burst, this burst is a horrific fit of anger, revealing the true desperation and viciousness of Finn, breaking any image of a clown/soft character that may have been portrayed prior. 
        // Finn 
        // DAMMIT
        finnSource.PlayOneShot(finnClips[4]);
        yield return new WaitForSeconds(finnClips[4].length);

        // Any heavy wind audio or other prominent sources fade slightly for tension. 
        // Finn 
        // I don’t think you quite understand the importance of what we’re doing here. Do you know what we’ve been through? The Things we’ve had to do, to rid the city of those devils? 
        finnSource.PlayOneShot(finnClips[5]);
        yield return new WaitForSeconds(finnClips[5].length);

        // The protag lunges forward (or some foot work) to denote intense posture change. 
        // Protag 
        // I’m here for the same reason as you. The same reason that any of us are doing any of this - to escape. To stop our pasts from destroying our futures, to make sure we don’t mess up again.
        protagSource.PlayOneShot(protagClips[8]);
        yield return new WaitForSeconds(protagClips[8].length);

        // Finn sounds almost nervous here, certainly a bit shakey from anger. 
        // Finn 
        // Oh yeah?
        finnSource.PlayOneShot(finnClips[6]);
        yield return new WaitForSeconds(finnClips[6].length);

        // Protag
        // Yeah. And trust me, you don’t want be on my list of mistakes.
        protagSource.PlayOneShot(protagClips[9]);
        yield return new WaitForSeconds(protagClips[9].length);

        // Finn 
        // Really?
        finnSource.PlayOneShot(finnClips[7]);
        yield return new WaitForSeconds(finnClips[7].length);

        // Protag
        // *exhale*
        protagSource.PlayOneShot(protagClips[10]);
        yield return new WaitForSeconds(protagClips[10].length);

        // Finn
        // Fine!
        finnSource.PlayOneShot(finnClips[8]);
        yield return new WaitForSeconds(finnClips[8].length);

        // Finn get’s back on the quadbike. Music rises in to end of the utterance. (maybe quad bike starts up as exit music kicks in)
        // Finn
        // I guess we’ll head back, see what’s hiding this one’s lungs, and then… then we go to Dusklight… 
        finnSource.PlayOneShot(finnClips[9]);
        yield return new WaitForSeconds(finnClips[9].length);
        
        StartCoroutine(audioController.ReduceMasterCutOff(8f));
        yield return new WaitForSeconds(8f);   

        musicSource.PlayOneShot(outroThemeClip);
        yield return new WaitForSeconds(outroThemeClip.length);

        finished = 1;
        saveAndLoadPumpingStation.FinishedPumpingStation();
        saveAndLoadPumpingStation.LoadMainMenu();
    }
}
