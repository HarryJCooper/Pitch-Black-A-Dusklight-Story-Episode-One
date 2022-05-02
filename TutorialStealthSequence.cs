using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TutorialStealthSequence : SequenceBase
{
    [SerializeField] bool skip;
    [SerializeField] AudioClip[] charlieClips, charlieLoopClips, playerClips, playerFootstepClips;
    [SerializeField] AudioClip crouchClipComp, crouchClipMob, swooshSFXClip, doorClip;
    [SerializeField] AudioSource charlieSource, motherSource, playerSource, protagReverbSource, doorSource;
    [SerializeField] AudioMixer audioMixer;
    bool sequenceFinished, walk;
    [SerializeField] float walkSpeedZ, maxDistanceFromCharlie;
    [SerializeField] Controls controls;
    [SerializeField] StealthTutorial stealthTutorial;
    [SerializeField] GameObject walkingTutorialWalls, stealthTutorialWalls, doorObject;
    [SerializeField] AudioController audioController;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] AmbienceRepeater ambienceRepeater;
    IEnumerator stealthTutorialCoroutine;

    void PlayOneShotWithVerb(AudioClip clip){
        playerSource.PlayOneShot(clip);
        protagReverbSource.PlayOneShot(clip);
    }

    // REMOVE ON BUILD
    void Awake(){active = 0;}

    void FixedUpdate(){ if (walk) Walk(charlieSource.transform, walkSpeedZ);}

    void Setup(){
        pBFootstepSystem.footstepClips = playerFootstepClips;
        pBFootstepSystem.canRotate = false;
        controls.inCutscene = true;
        audioMixer = motherSource.outputAudioMixerGroup.audioMixer;
        audioMixer.SetFloat("WalkingTutorialReverb_Vol", -80f);
        audioMixer.SetFloat("StealthTutorialReverb_Vol", 0f);
        audioMixer.SetFloat("Mother_InitialVol", 10f);
        audioMixer.SetFloat("OutsideDoor_Vol", 0f);
        doorObject.SetActive(true);
        motherSource.transform.position = new Vector3(-5, 0.3f, 10); // sets Mum behind the wall
        playerSource.transform.position = new Vector3(7.1f, 0.3f, -5.4f); // resets player position
        playerSource.gameObject.transform.localRotation = Quaternion.identity; //
        charlieSource.transform.position = new Vector3(10.2f, 0.3f, 0); // sets Charlie position
        if (controls.mobile){
            charlieClips[2] = crouchClipMob;
        } else {
            charlieClips[2] = crouchClipComp;
        }
        
        walkingTutorialWalls.SetActive(false);
        stealthTutorialWalls.SetActive(true);
        
        if (stealthTutorial.loopCoroutine != null){
            StopCoroutine(stealthTutorial.loopCoroutine);
        }
        StartCoroutine(ambienceRepeater.ambienceCoroutine);
        ambienceRepeater.stopped = false;
    }

    public void RestartSequence(){
        if (stealthTutorialCoroutine != null) StopCoroutine(stealthTutorialCoroutine);
        stealthTutorialCoroutine = Sequence();
        StartCoroutine(stealthTutorialCoroutine);
    }

    public override IEnumerator Sequence(){
        if (skip && Application.isEditor){
            StartCoroutine(Outro());
            yield break;
        }
        motherSource.transform.position = new Vector3(-5, 0.3f, 10); // sets Mum behind the wall
        Setup();
        motherSource.Play();
        StartCoroutine(audioController.IncreaseMasterCutOff(10f)); 
        yield return new WaitForSeconds(8f);
        stealthTutorial.turnedOn = true;
        
        // INT. HOUSE – DAY.
        // The next tutorial is a stealth tutorial. The protag is older now, sevenish. Him and his brothers are trying to sneak out of the house. 
        // All characters are whispering for this sequence. Besides mother, who’s bitterly muttering. 

        // Charlie 
        // Come on big brother, let’s get out of here, we just gotta sneak past mom, and then we’re free!
        charlieSource.PlayOneShot(charlieClips[0]);
        yield return new WaitForSeconds(charlieClips[0].length);

        // Mother can be heard clanging in the kitchen, frustrated and showing signs of instability. 

        // Protag 
        // Okay, you first.
        PlayOneShotWithVerb(playerClips[0]);
        yield return new WaitForSeconds(playerClips[0].length);

        // Charlie 
        // Alright, just gotta stay low and move slowly. You wait here.
        charlieSource.PlayOneShot(charlieClips[1]);
        yield return new WaitForSeconds(charlieClips[1].length);

        // The brother moves past the kitchen doorway. 
        walk = true;
        yield return new WaitForSeconds(7f);
        walk = false;

        // Mother
        // All day, dishes and pots, those stupid kids. How can they do this to me, I work all day and night for them, and they repay me with this. I can’t do it anymore, I can’t. And that good for nothing father huh where is he, where’s he sneaking around at night?! One day they’ll realise, little JERKS. I’ll show them, I’ll show them what they’re worth to me. Trying to sneak around and go behind my back, gotta keep watching them, one eye open. Only got a few more years then tick-tick, tick-tock. My times strikes out, and I’m free, free from that cure, free from those kids, free from this hole of a city. 
        stealthTutorial.loopCoroutine = stealthTutorial.RepeatLoopClips();
        StartCoroutine(stealthTutorial.loopCoroutine);

        // Charlie 
        // Come on you next. Tap control to sneak and then move forward/swipe down to sneak and then move forward.
        charlieSource.PlayOneShot(charlieClips[2]);
        yield return new WaitForSeconds(charlieClips[2].length);
        doorSource.PlayOneShot(doorClip);
        yield return new WaitForSeconds(0.2f);
        doorObject.SetActive(false);
        yield return new WaitForSeconds(doorClip.length - 0.2f);

        // Protag
        // Alright, crouch and move slow. If my brother can do it, so can I.
        PlayOneShotWithVerb(playerClips[1]);
        yield return new WaitForSeconds(playerClips[1].length);
        controls.inCutscene = false;
        pBFootstepSystem.canRotate = true;
        controls.inStealth = true;

        yield return new WaitForSeconds(10f);
        if (Vector3.Distance(charlieSource.transform.position, playerSource.transform.position) > maxDistanceFromCharlie && !stealthTutorial.beenCaught){
            // Charlie 
            // Over here, come on, brother. This way. Gotta move to me, then we can get out of here.
            charlieSource.PlayOneShot(charlieClips[3]);
            StartCoroutine(CheckIfComplete());
            StartCoroutine(LoopClips());
        } else {
            StartCoroutine(CheckIfComplete());
        }
        // If the player gets caught the mother lashes out and shouts at the boys, grabbing the protag and starting the sequence again.         
    }

    void Walk(Transform transform, float z){
        transform.position += new Vector3(0, 0, z);
    }

    IEnumerator LoopClips(){
        yield return new WaitForSeconds(12f);
        if (controls.inCutscene) yield break;
        Debug.Log("made it past yielding break");
        if (!charlieSource.isPlaying){
            Debug.Log("said thingy");
            charlieSource.PlayOneShot(charlieLoopClips[Random.Range(0, charlieLoopClips.Length)]);
            StartCoroutine(LoopClips());
        } else {
            Debug.Log("didn't say thingy");
            StartCoroutine(LoopClips());
        }

    }

    IEnumerator CheckIfComplete(){
        if ((Vector3.Distance(charlieSource.transform.position, playerSource.transform.position) < maxDistanceFromCharlie)){
            charlieSource.Stop();
            StartCoroutine(Outro());
            yield break;
        } else {
            yield return new WaitForSeconds(0f);
            StartCoroutine(CheckIfComplete());
        }
    }

    IEnumerator Outro(){
        controls.inCutscene = true;
        // Charlie
        // Yes! We did it little brother. Free at last, we’ll never let anyone hold us down… ever.	
        finished = 1;
        charlieSource.PlayOneShot(charlieClips[4]);
        stealthTutorial.turnedOn = false;
        controls.inStealth = false;
        ambienceRepeater.stopped = true;
        audioMixer.SetFloat("Mother_InitialVol", -80f);
        motherSource.Stop();
        yield return new WaitForSeconds(charlieClips[4].length);
        playerSource.PlayOneShot(swooshSFXClip);
        StartCoroutine(audioController.ReduceMasterCutOff(10f));
        StartCoroutine(audioController.FadeMusic());
        yield return new WaitForSeconds(12f);
        finished = 1;
        PlayerPrefs.SetInt("tutorial", 1);
        SceneManager.LoadScene("BunkerAndEncampment");
    }
}
