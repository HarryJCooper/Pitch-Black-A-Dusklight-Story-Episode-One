using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TutorialWalkingSequence : SequenceBase
{
    bool successClipHasPlayed, hasPlayedSqueakyToy, hasPlayedCar, moveToyCar, hasStarted;
    [SerializeField] bool skip;
    [SerializeField] AudioClip eerieDroneClip, eerieDroneClipShort, nurseryRhyme, forwardMobileClip, forwardComputerClip, forwardMobileClip1, forwardComputerClip1, 
    rotateMobileClip0, rotateComputerClip0, rotateMobileClip1, rotateComputerClip1, successClip, squeakyToyClip, toyCarClip;
    [SerializeField] private AudioClip[] motherClips, rattleClips, playerClips, playerFootstepClips;
    [SerializeField] AudioSource playerSource, motherSource, protagReverbSource, toySource;
    [SerializeField] AudioSource[] radioSources, rumbleSources;
    bool sequenceFinished, motherTalking = true, walk;
    [SerializeField] int i = 0;
    [SerializeField] float maxDistanceFromMother, walkTime, walkSpeedX = 0, walkSpeedZ, roomMaxVol;
    [SerializeField] Transform player;
    [SerializeField] TutorialStealthSequence tutorialStealthSequence;
    [SerializeField] Controls controls;
    AudioMixer audioMixer;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] AudioController audioController;
    [SerializeField] AmbienceRepeater ambienceRepeater;
    [SerializeField] GameObject walkingTutorialWalls, stealthTutorialWalls, combatTutorialWalls;
    [SerializeField] float waitTime;

    //     INT. HOUSE – DAY 
    // Player is subject to a series of tutorials that revolve around the protagonist’s early life. The first tutorial is a simple walking tutorial whereby the protagonist is a baby being directed by his mother. The nursery rhyme fades in for 3 seconds ish. 
    // The protagonist’s mother is talking to the protag in a usual motherly tone. At first, the baby protag is in the cot; rattle sounds and mobile movement, and a nursery theme tune (dark) can be heard. However, every so often, the mother exhibits signs of instability and mental fracture (has to be discreet, can’t be too scary) 
    // *All tutorials finish with the sound of children laughing in distance*
    // *take all whimper lines with a normal version too*

    void Awake(){
        active = 0;
    }

    void Update(){
        if (!hasStarted) return;
        float distanceFromMother = Vector3.Distance(motherSource.transform.position, player.transform.position);
        if (distanceFromMother > 65){
            motherSource.transform.position = Vector3.MoveTowards(motherSource.transform.position, 
                                playerSource.transform.position, 
                                Vector3.Distance(motherSource.transform.position, playerSource.transform.position) - 65); 
        }
        if (distanceFromMother < maxDistanceFromMother && !successClipHasPlayed && !motherTalking){
            successClipHasPlayed = true;
            StartCoroutine(PlaySuccessClip());
        }
        if (player.position.z > 38 && !hasPlayedSqueakyToy){
            hasPlayedSqueakyToy = true;
            StartCoroutine(PlaySqueakyToyClip());
        }
        if (player.position.z > 83 && !hasPlayedCar){
            hasPlayedCar = true;
            StartCoroutine(PlayToyCarClip());
        }
    }

    IEnumerator PlaySuccessClip(){
        playerSource.PlayOneShot(successClip);
        controls.inCutscene = true;
        yield return new WaitForSeconds(successClip.length);
        controls.inCutscene = false;
        successClipHasPlayed = false;
    }

    IEnumerator PlaySqueakyToyClip(){
        toySource.transform.position = player.position -= new Vector3(0, -0.5f, 0);
        toySource.PlayOneShot(squeakyToyClip);
        yield return new WaitForSeconds(squeakyToyClip.length);
        if (!playerSource.isPlaying) PlayOneShotWithVerb(playerClips[Random.Range(0, playerClips.Length)]);
    }

    IEnumerator PlayToyCarClip(){
        toySource.transform.position = player.position -= new Vector3(0, -0.5f, 0);
        toySource.PlayOneShot(toyCarClip);
        moveToyCar = true;
        yield return new WaitForSeconds(toyCarClip.length);
        moveToyCar = false;
        if (!playerSource.isPlaying) PlayOneShotWithVerb(playerClips[Random.Range(0, playerClips.Length)]);
    }

    public IEnumerator StartTrainRumble(){
        yield return new WaitForSeconds(waitTime);
        if (finished == 1) yield break; 
        foreach (AudioSource rumbleSource in rumbleSources) rumbleSource.Play();
        yield return new WaitForSeconds(3f);
        if (!playerSource.isPlaying) PlayOneShotWithVerb(playerClips[Random.Range(0, playerClips.Length)]);
        StartCoroutine(StartTrainRumble());
    }

    void PlayOneShotWithVerb(AudioClip clip){
        playerSource.PlayOneShot(clip);
        protagReverbSource.PlayOneShot(clip);
    }

    void Setup(){
        hasStarted = true;
        controls.inCutscene = true;
        walkingTutorialWalls.SetActive(true);
        stealthTutorialWalls.SetActive(false);
        pBFootstepSystem.isBaby = true;
        audioMixer = motherSource.outputAudioMixerGroup.audioMixer;
        audioMixer.SetFloat("WalkingTutorialReverb_Vol", 0f);
        audioMixer.SetFloat("StealthTutorialReverb_Vol", -80f);
        motherSource.transform.position = new Vector3(0, 0.3f, 5); // sets Mum in front of player
        playerSource.transform.position = new Vector3(0, 0.3f, 0); // resets player position
        StartCoroutine(StartRadioSources());
        StartCoroutine(StartTrainRumble());
        pBFootstepSystem.footstepClips = playerFootstepClips;
        if (controls.mobile){
            motherClips[1] = forwardMobileClip;
            motherClips[4] = forwardMobileClip1;
            motherClips[5] = rotateMobileClip0;
            motherClips[6] = rotateMobileClip1;
            return;
        }
    }

    IEnumerator StartRadioSources(){
        foreach (AudioSource source in radioSources){
            source.loop = true;
            source.clip = nurseryRhyme;
            if(!source.isPlaying) source.Play();
            yield return new WaitForSeconds(0.005f);
        }
        yield break;
    }

    public override IEnumerator Sequence(){
        if (skip && Application.isEditor){
            SkipToNextSequence();
            yield break;
        }
        Setup();
        StartCoroutine(ambienceRepeater.ambienceCoroutine);
        ambienceRepeater.gameObject.transform.position = new Vector3(-54.8f, 0, -30.8f);
        StartCoroutine(audioController.IncreaseMasterCutOff(10f));
        yield return new WaitForSeconds(12f);
        // Mother 
        // Wow, look how big you’re getting. You’re gonna grow up to be something special one day. Maybe, just maybe, you’ll leave this wicked city. *gasp* maybe… just maybe, you’ll fix our minds… 
        motherSource.PlayOneShot(motherClips[0]);
        yield return new WaitForSeconds(motherClips[0].length - 3f);
        playerSource.PlayOneShot(eerieDroneClipShort, 0.2f);
        yield return new WaitForSeconds(3f);
        if (!playerSource.isPlaying) PlayOneShotWithVerb(playerClips[3]);
        yield return new WaitForSeconds(playerClips[3].length);
        StartCoroutine(SequenceOne());
    }

    IEnumerator SequenceOne(){
        controls.inCutscene = false;
        // The nursery tune dips in pitch ever so slightly, like the slowing down of a record, before returning to normal. The mother then quickly picks up baby protag, puts him on the floor and runs to other side of the room. The source of the music becomes more localisable. The mother taps her thigh throughout. 
        // Mother
        // Heeey, come on little boy. press forward.
        motherSource.PlayOneShot(motherClips[1]);
        yield return new WaitForSeconds(motherClips[1].length);
        StartCoroutine(WalkOne());
    }

    IEnumerator WalkOne(){
        walk = true;
        walkSpeedZ = Random.Range(0.07f, 0.1f);
        walkTime = Random.Range(4, 7);
        yield return new WaitForSeconds(walkTime);
        motherTalking = false;
        StartCoroutine(LoopOne());
    }

    IEnumerator LoopOne(){
        walk = false;
        i = Random.Range(0, rattleClips.Length);
        motherSource.PlayOneShot(rattleClips[i]);
        yield return new WaitForSeconds(rattleClips[i].length);
        if (Vector3.Distance(motherSource.transform.position, player.transform.position) < maxDistanceFromMother){
            StartCoroutine(SequenceTwo());
        } else {
            StartCoroutine(LoopOne());
        }
    }

    IEnumerator SequenceTwo(){
        motherTalking = true;
        // Mother
        // This way sweety, walk forward. Really think about it. You know how to walk forward; you’ve done it a thousand times already.
        motherSource.PlayOneShot(motherClips[2]);
        yield return new WaitForSeconds(motherClips[2].length);
        PlayOneShotWithVerb(playerClips[4]);
        StartCoroutine(WalkTwo());
    }

    IEnumerator WalkTwo(){
        walk = true;
        walkSpeedZ = Random.Range(0.07f, 0.1f);
        walkTime = Random.Range(4, 7);
        yield return new WaitForSeconds(walkTime);
        motherTalking = false;
        StartCoroutine(LoopTwo());
    }

    IEnumerator LoopTwo(){
        walk = false;
        i = Random.Range(0, rattleClips.Length);
        motherSource.PlayOneShot(rattleClips[i]);
        yield return new WaitForSeconds(rattleClips[i].length);
        if (Vector3.Distance(motherSource.transform.position, player.transform.position) < maxDistanceFromMother){
            StartCoroutine(SequenceThree());
        } else {
            StartCoroutine(LoopTwo());
        }
    }

    IEnumerator SequenceThree(){
        motherTalking = true;
         // Mother 
        // *Slight whimper* Come on baby. 
        motherSource.PlayOneShot(motherClips[3]);
        yield return new WaitForSeconds(motherClips[3].length);
        StartCoroutine(WalkThree());
    }

    IEnumerator WalkThree(){
        PlayOneShotWithVerb(playerClips[0]); // Added in baby giggle for fun.
        walk = true;
        walkSpeedZ = Random.Range(0.07f, 0.1f);
        walkTime = Random.Range(4, 7);
        yield return new WaitForSeconds(walkTime);
        motherTalking = false;
        StartCoroutine(LoopThree());
    }

    IEnumerator LoopThree(){
        walk = false;
        i = Random.Range(0, rattleClips.Length);
        motherSource.PlayOneShot(rattleClips[i]);
        yield return new WaitForSeconds(rattleClips[i].length);
        if (Vector3.Distance(motherSource.transform.position, player.transform.position) < maxDistanceFromMother){
            StartCoroutine(SequenceFour());
        } else {
            StartCoroutine(LoopThree());
        }
    }

    IEnumerator SequenceFour(){
        motherTalking = true;
        // Mother then speaks once more but with a stark outburst of frustration that transitions back to sweet and motherly. As if she is trying to remain calm and sweet. 
        // Mother
        // COME ON, Walk towards me, it’s easy… 
        playerSource.PlayOneShot(eerieDroneClipShort, 0.2f);
        motherSource.PlayOneShot(motherClips[4]);
        yield return new WaitForSeconds(motherClips[4].length);
        StartCoroutine(WalkFour());
    }

    IEnumerator WalkFour(){
        if (!playerSource.isPlaying) PlayOneShotWithVerb(playerClips[1]);
        walk = true;
        walkSpeedZ = Random.Range(0.07f, 0.1f);
        walkTime = Random.Range(4, 7);
        yield return new WaitForSeconds(walkTime);
        motherTalking = false;
        StartCoroutine(LoopFour());
    }

    IEnumerator LoopFour(){
        walk = false;
        i = Random.Range(0, rattleClips.Length);
        motherSource.PlayOneShot(rattleClips[i]);
        yield return new WaitForSeconds(rattleClips[i].length);
        if (Vector3.Distance(motherSource.transform.position, player.transform.position) < maxDistanceFromMother){
            StartCoroutine(SequenceSix());
        } else {
            StartCoroutine(LoopFour());
        }
    }
        
    IEnumerator SequenceSix(){
        motherTalking = true;
        // The mother now walks around the house. The route is simple and mother explains that left and right rotate the protag. 
        // Mother
        // Here sweety, look, you can press left to turn left. And now the other way… thaaaat’s it. 
        motherSource.PlayOneShot(motherClips[5]);
        yield return new WaitForSeconds(motherClips[5].length);
        StartCoroutine(WalkSix());
    }

    IEnumerator WalkSix(){
        if (!playerSource.isPlaying) PlayOneShotWithVerb(playerClips[2]);
        walk = true;
        walkSpeedX = -0.1f;
        walkSpeedZ = Random.Range(0.07f, 0.1f);
        walkTime = Random.Range(4, 7);
        yield return new WaitForSeconds(walkTime);
        motherTalking = false;
        StartCoroutine(LoopSix());
    }

    IEnumerator LoopSix(){
        walk = false;
        i = Random.Range(0, rattleClips.Length);
        motherSource.PlayOneShot(rattleClips[i]);
        yield return new WaitForSeconds(rattleClips[i].length);
        if (Vector3.Distance(motherSource.transform.position, player.transform.position) < maxDistanceFromMother){
            StartCoroutine(SequenceSeven());
        } else {
            StartCoroutine(LoopSix());
        }
    }

    IEnumerator SequenceSeven(){
        motherTalking = true;
        // Mother
        // Remember how to rotate? We’ve been through this, press left or right to turn and center my voice and then walk straight towards me.  
        // The player then walks towards mother and she picks up the baby protag, congratulating him. 
        motherSource.PlayOneShot(motherClips[6]);
        yield return new WaitForSeconds(motherClips[6].length);
        PlayOneShotWithVerb(playerClips[6]);
        StartCoroutine(WalkSeven());
    }

    IEnumerator WalkSeven(){
        walk = true;
        walkSpeedX = 0.1f;
        walkSpeedZ = Random.Range(0.07f, 0.1f);
        walkTime = Random.Range(4, 7);
        yield return new WaitForSeconds(walkTime);
        motherTalking = false;
        StartCoroutine(LoopSeven());
    }

    IEnumerator LoopSeven(){
        walk = false;
        i = Random.Range(0, rattleClips.Length);
        motherSource.PlayOneShot(rattleClips[i]);
        yield return new WaitForSeconds(rattleClips[i].length);
        if (Vector3.Distance(motherSource.transform.position, player.transform.position) < maxDistanceFromMother){
            StartCoroutine(SequenceEight());
        } else {
            StartCoroutine(LoopSeven());
        }
    }

    IEnumerator SequenceEight(){
        motherTalking = true;
        // Mother
        // Well done honey, remember walking isn’t always as easy as you might think, just be patient and use those little ears of yours, you’ll be fine. 
        motherSource.PlayOneShot(motherClips[7]);
        yield return new WaitForSeconds(motherClips[7].length);

        // The mother says her closing line by leaning into a cry or whimper. The whole line is said with uncertainty and sorrow. A subtle, eerie drone note foreshadows the dialogue. 
        // Mother
        // We’ll all be okay.   
        playerSource.PlayOneShot(eerieDroneClip);
        motherSource.PlayOneShot(motherClips[8]);
        yield return new WaitForSeconds(motherClips[8].length);
        StartCoroutine(Outro());
    }

        // Transition to the next scene contains swoosh fx and a ticking, speeding and then slowing clock. 


    IEnumerator Outro(){
        StartCoroutine(audioController.ReduceMasterCutOff(7.5f));
        yield return new WaitForSeconds(7.5f);
        audioController.SetCutOffToZero();
        ambienceRepeater.StopAllSources();
        foreach (AudioSource source in radioSources) source.Stop();
        finished = 1;
        tutorialStealthSequence.RestartSequence();
        yield return new WaitForSeconds(0.1f);
        pBFootstepSystem.isBaby = false;
        walkingTutorialWalls.SetActive(false);
    }

    void SkipToNextSequence(){
        finished = 1;
        tutorialStealthSequence.RestartSequence();
    }

    void Walk(float x, float z){ motherSource.transform.position += new Vector3(x, 0, z);}

    void FixedUpdate(){ 
        if (walk) Walk(walkSpeedX, walkSpeedZ);
        if (moveToyCar) toySource.transform.position += new Vector3(0.01f, 0, 0.05f);
    }
}
