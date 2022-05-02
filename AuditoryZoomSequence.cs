using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AuditoryZoomSequence : SequenceBase
{
    [SerializeField] AudioSourceContainer audioSourceContainer;
    [SerializeField] AudioClip[] finnClips, finnClipsMob, finnLoopClips, finnFootstepClips, protagClips, auditoryZoomClips;
    [SerializeField] AudioClip addedToInventoryClip, thereYouGoClip, dusklightStereoClip, trainStereoClip, pumpingStationStereoClip, finnReleaseSpaceBarClip, finnReleaseMobClip;
    bool hasPassed, moveTrain, moveTowardsOne, moveTowardsTwo, checkingForKeys, hasPressedKey, moveFinnToEdgeOfCliff, moveFinnLeft, moveFinnRight, fadeDusklightAfterHit;
    [SerializeField] GameObject edgeOfCliff, distantDusklightObject, trainObject, pumpingStationObject;
    [SerializeField] Controls controls;
    [SerializeField] ChangeAfterFocus dusklightFocus, trainFocus, pumpingStationFocus;
    [SerializeField] EncampmentCombatSequence encampmentCombatSequence;
    [SerializeField] MechanicSequence mechanicSequence;
    [SerializeField] AroundTableSequence aroundTableSequence;
    [SerializeField] QuadbikeFinishedSequence quadbikeFinishedSequence;
    [SerializeField] SaveAndLoadEncampment saveAndLoadEncampment;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] EnemyFootsteps finnFootsteps;
    [SerializeField] ZoomSound zoomSound;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    float fadeDusklightVol;
    int loopInt;

    void Awake(){ active = 0;}

    public void CheckIfShouldStart(){
        if (encampmentCombatSequence.finished == 1
        && mechanicSequence.finished == 1
        && aroundTableSequence.finished == 1){
            StartCoroutine(Sequence());
        }
    }

    bool NotInRange(){
        if (Vector3.Distance(audioSourceContainer.protagSource.transform.position, audioSourceContainer.finnSource.transform.position) < 8){
            return false;
        }
        return true;
    }

    void MoveTrain(){
        if (Vector3.Distance(trainFocus.transform.position, dusklightFocus.transform.position) > 3){
            dusklightFocus.transform.position = Vector3.MoveTowards(trainFocus.transform.position, dusklightFocus.transform.position, 0.1f); 
            return;
        }
        moveTrain = false;
    }

    void MoveFinnRight() => audioSourceContainer.finnSource.transform.position += new Vector3(0.04f, 0, 0);
    
    void MoveFinnLeft() => audioSourceContainer.finnSource.transform.position += new Vector3(-0.04f, 0, 0);

    void MoveFinnToEdgeOfCliff(){
        if (Vector3.Distance(audioSourceContainer.finnSource.transform.position, edgeOfCliff.transform.position) > 3){
            audioSourceContainer.finnSource.transform.position = Vector3.MoveTowards(audioSourceContainer.finnSource.transform.position, edgeOfCliff.transform.position, 0.1f); 
        } else {
            moveFinnToEdgeOfCliff = false;
        }
    }

    void Setup(){
        audioSourceContainer.finnSource.transform.position = new Vector3(
            audioSourceContainer.protagSource.transform.position.x + 40, 
            0.3f,
            audioSourceContainer.protagSource.transform.position.z + 40);
        if (controls.mobile){
            finnClips[9] = finnClipsMob[0];
            finnClips[10] = finnClipsMob[1];
            finnClips[11] = finnClipsMob[2];
            finnClips[16] = finnClipsMob[3];
        }
        trainObject.SetActive(true); 
        pumpingStationObject.SetActive(true);
        distantDusklightObject.SetActive(true);
        audioSourceContainer.trainSource.Play();
        audioSourceContainer.dusklightSource.Play();
        audioSourceContainer.pumpingStationSource.Play();
        audioMixer.SetFloat("Finn_Vol", -5f);
        finnFootsteps.footstepClips = finnFootstepClips;
    }

    void MoveTowards(float maxDistance){
        if (Vector3.Distance(audioSourceContainer.protagSource.transform.position, audioSourceContainer.finnSource.transform.position) > maxDistance)
        audioSourceContainer.finnSource.transform.position = Vector3.MoveTowards(audioSourceContainer.finnSource.transform.position, audioSourceContainer.protagSource.transform.position, 0.1f); 
    }

    void Update(){
        if (checkingForKeys){
            if (Input.anyKeyDown || Input.touchCount > 0){
                hasPressedKey = true;
            }
        }
    }

    void FixedUpdate(){
        if (fadeDusklightAfterHit){
            fadeDusklightVol -= 0.2f;
            audioMixer.SetFloat("Dusklight_FadeAfterHitVol", fadeDusklightVol);
        }
        if (moveTowardsOne) MoveTowards(10);
        if (moveTowardsTwo) MoveTowards(3);
        if (moveFinnToEdgeOfCliff) MoveFinnToEdgeOfCliff();
        if (moveTrain) MoveTrain();
        if (moveFinnLeft) MoveFinnLeft();
        if (moveFinnRight) MoveFinnRight();
    }

    public override IEnumerator Sequence(){
        if (finished == 1) yield break;
        // After the player finishes with each scene (any order), Finn approaches the player. 
        Setup();
        moveTowardsOne = true;
        yield return new WaitForSeconds(8f);
        moveTowardsOne = false;
        audioSourceContainer.protagActionSource.PlayOneShot(cutsceneEnterClip);
        yield return new WaitForSeconds(cutsceneEnterClip.length);
        // Finn
        // Agent, come here!
        audioSourceContainer.finnSource.PlayOneShot(finnClips[0]);
        yield return new WaitForSeconds(finnClips[0].length);

        yield return new WaitForSeconds(7f);
        // If player stays still or doesn’t find Finn then Finn moves to player. In which case, he replies in a very sarcastic tone.
        if (NotInRange()){
            // Finn
            // Or I could come to you… either way’s fine. 
            moveTowardsTwo = true;
            audioSourceContainer.finnSource.PlayOneShot(finnClips[1]);
            yield return new WaitForSeconds(finnClips[1].length + 2f);
            moveTowardsTwo = false;
        }
        // Finn
        // Guessing you spoke to everyone? Either way, if you’re ready then so am I. Tech buffs in the bunker have left us a new toy. Yes, it’s a prototype, but not the kind that blows up
        audioSourceContainer.finnSource.PlayOneShot(finnClips[2]);
        yield return new WaitForSeconds(finnClips[2].length);
        // Finn speaks in a tone that suggests he’s kind of kidding but also wouldn’t be sad if the device did kill the protag. Maybe Finn winks. 

        // Finn
        // So, don’t worry. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[3]);
        yield return new WaitForSeconds(finnClips[3].length);

        // Protag replies in a sarcastic tone. 
        // Protag
        // Thank ‘em for me. 
        audioSourceContainer.protagSource.PlayOneShot(protagClips[0]);
        yield return new WaitForSeconds(protagClips[0].length);

        // Finn
        // It’s called the auditory zoom. Lets you hear for miles, it automatically highlights points of interest. Follow me, we’ll go to edge of the cliff here, and try it out. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[4]);
        yield return new WaitForSeconds(finnClips[4].length);

        moveFinnToEdgeOfCliff = true;
        // Lines for if player fails to follow. 
        yield return new WaitForSeconds(10f);
        StartCoroutine(InRangeLoop());
    }

    int RandomNumberGen(){
        int randomInt = Random.Range(0, finnLoopClips.Length);
        if (randomInt == loopInt) randomInt = RandomNumberGen();
        return randomInt;
    }

    IEnumerator InRangeLoop(){
        if (NotInRange()){
            StartCoroutine(MoveTimer(8));
            // Finn
            // Come on agent / Over-here / 
            audioSourceContainer.finnSource.PlayOneShot(finnClips[5]);
            yield return new WaitForSeconds(8f);
            if (NotInRange()){
                StartCoroutine(MoveTimer(8));
                audioSourceContainer.finnSource.PlayOneShot(finnLoopClips[RandomNumberGen()]);
                yield return new WaitForSeconds(8f);
                if (NotInRange()){
                    StartCoroutine(InRangeLoop());    
                } else {
                    StartCoroutine(SequenceTwo());
                }
            } else {
                StartCoroutine(InRangeLoop());
            }
        } else {
            StartCoroutine(SequenceTwo());
        }
    }

    IEnumerator MoveTimer(float time){
        moveFinnRight = true;
        yield return new WaitForSeconds(time / 4);
        moveFinnRight = false;
        moveFinnLeft = true;
        yield return new WaitForSeconds(time / 2);
        moveFinnLeft = false;
        moveFinnRight = true;
        yield return new WaitForSeconds(time / 4);
        moveFinnRight = false;
    }

    IEnumerator SequenceTwo(){
        // Finn
        // There you go
        audioSourceContainer.finnSource.PlayOneShot(thereYouGoClip);
        yield return new WaitForSeconds(thereYouGoClip.length);
        // Finn stops and begins to speak in an energetic, inspired leader like tone, but leans into a sad and hopeless cadence.
        // Finn
        // Dusklight. Listen to it. The light of a million civilians, shrouded in the shadow of the moths. The Church, Lambton Energy, they don’t hear what we hear… they don't understand.  
        audioSourceContainer.finnSource.PlayOneShot(finnClips[8]);
        yield return new WaitForSeconds(finnClips[8].length + 4f);
        // Finn puts the device on the player and an appropriate foley sound triggers denoting that the player now has it on. 
        // Finn
        // Right, on the device, ok hold on, on the device press 1.
        audioSourceContainer.finnSource.PlayOneShot(finnClips[9]);
        yield return new WaitForSeconds(finnClips[9].length);
        hasPressedKey = false;
        checkingForKeys = true;
        StartCoroutine(CheckIfPressedLoopOne());
    }

    IEnumerator CheckIfPressedLoopOne(){
        // A ‘wrong input’ sound triggers, this may be comical or serious. Finn tries again. 
        if (hasPressedKey){
            checkingForKeys = false;
            hasPressedKey = false;
            audioSourceContainer.protagActionSource.PlayOneShot(auditoryZoomClips[0]);
            yield return new WaitForSeconds(auditoryZoomClips[0].length);
            StartCoroutine(SequenceThree());
            yield break;
        } else {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(CheckIfPressedLoopOne());
        }
    }

    IEnumerator SequenceThree(){
        // Finn
        // Wait… that can’t be it, no, press the I button.  
        audioSourceContainer.finnSource.PlayOneShot(finnClips[10]);
        yield return new WaitForSeconds(finnClips[10].length);
        checkingForKeys = true;
        StartCoroutine(CheckIfPressedLoopTwo());
    }

    IEnumerator CheckIfPressedLoopTwo(){
        // This time, something falls off the device and clunks on the floor, maybe a short burst of steam triggers. 
        if (hasPressedKey){
            checkingForKeys = false;
            hasPressedKey = false;
            audioSourceContainer.protagActionSource.PlayOneShot(auditoryZoomClips[1]);
            yield return new WaitForSeconds(auditoryZoomClips[1].length);
            StartCoroutine(SequenceFour());
            yield break;
        } else {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(CheckIfPressedLoopTwo());
        }
    }

    IEnumerator SequenceFour(){
        // Finn
        // By the night, stupid thing! Okay, no I remember, it’s the space bar button/double tap that centre button there.
        audioSourceContainer.finnSource.PlayOneShot(finnClips[11]);
        yield return new WaitForSeconds(finnClips[11].length);
        controls.canZoom = true;
        StartCoroutine(CheckIfPlayerZoomedAndHitDusklight());
    }

    IEnumerator CheckIfZoomed(){
        // There you go, fix your ears on Dusklight’s districts. 
        if(controls.inZoom){
			audioSourceContainer.finnSource.PlayOneShot(finnClips[20]); // this is quite lazy and I apologise sincerely.
			yield return new WaitForSeconds(finnClips[20].length);
            StartCoroutine(CheckIfPlayerZoomedAndHitDusklight());
            yield break;
        }
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(CheckIfZoomed());
    }

    IEnumerator CheckIfPlayerZoomedAndHitDusklight(){
        pBFootstepSystem.canRotate = false;
        audioSourceContainer.protagSource.transform.rotation = Quaternion.identity;
        if(controls.inZoom && dusklightFocus.hit){
            Debug.Log("hit dusklight");
            controls.lockZoom = true;
            zoomSound.disabledZoomSound = true;
            StartCoroutine(SequenceFive());
            yield break;
        }
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(CheckIfPlayerZoomedAndHitDusklight());
    }

    IEnumerator SequenceFive(){
        audioSourceContainer.protagActionSource.PlayOneShot(dusklightStereoClip, 0.5f);
        yield return new WaitForSeconds(dusklightStereoClip.length - 5f);
        fadeDusklightAfterHit = true;
        // Finn
        // That sound… that’s why. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[12]);
        yield return new WaitForSeconds(finnClips[12].length);
        yield return new WaitForSeconds(6f);
        // Finn switches to the next line quickly. And speaks dismissively when mentioning Lambton. 
        fadeDusklightAfterHit = false;
        controls.lockZoom = false;
        zoomSound.disabledZoomSound = false;
        pBFootstepSystem.canRotate = true;
        // Finn
        // Aaannd on your right, you’ll hear a train approaching the city gates. 
        moveTrain = true;
        audioSourceContainer.finnSource.PlayOneShot(finnClips[13]);
        yield return new WaitForSeconds(finnClips[13].length);

        StartCoroutine(CheckIfHitTrain());
    }

    IEnumerator CheckIfHitTrain(){
        if(controls.inZoom && trainFocus.hit){
            Debug.Log("hit train");
            controls.lockZoom = true;
            zoomSound.disabledZoomSound = true;
            pBFootstepSystem.canRotate = false;
            StartCoroutine(SequenceSix());
        } else {
            yield return new WaitForSeconds(0.01f);
            StartCoroutine(CheckIfHitTrain());
        }
    }

    IEnumerator SequenceSix(){
        audioSourceContainer.protagActionSource.PlayOneShot(trainStereoClip, 0.5f);
        yield return new WaitForSeconds(trainStereoClip.length - 3f);
        pBFootstepSystem.canRotate = true;
        controls.lockZoom = false;
        zoomSound.disabledZoomSound = false;
        // Finn talks again when the player identifies the train. 
        // Finn
        // Eugh, Lambton Energy. Paper pushers in their palaces, money grabbers in their mansions, the corporate kings of Dusklight. They might have the money, but believe me, it’s The Church that hold the power. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[14]);
        yield return new WaitForSeconds(finnClips[14].length);
        // Finn
        // Okay, the last stop on our tour of the desert is the pumping station. Listen for the thing that sounds remarkably like a pumping station. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[15]);
        yield return new WaitForSeconds(finnClips[15].length);
        StartCoroutine(CheckIfHitPumpingStation());
    }

    IEnumerator CheckIfHitPumpingStation(){
        if (controls.inZoom && pumpingStationFocus.hit){
            Debug.Log("hit pumping station");
            controls.lockZoom = true;
            zoomSound.disabledZoomSound = true;
            pBFootstepSystem.canRotate = false;
            StartCoroutine(SequenceSeven());
            yield break;
        }
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(CheckIfHitPumpingStation());
    }

    IEnumerator SequenceSeven(){
        audioSourceContainer.protagActionSource.PlayOneShot(pumpingStationStereoClip, 0.5f);
        yield return new WaitForSeconds(pumpingStationStereoClip.length - 3f);
        controls.lockZoom = false;
        zoomSound.disabledZoomSound = false;
        pBFootstepSystem.canRotate = true;
        // Finn
        // That’s our target. The stations mine minerals for Lambton Energy. And apparently, the exact things we should be looking for are sitting nice and cosy like right in the overseers office. That’s the mission, get the docs and get the night out of there. Let’s do it. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[16]);
        yield return new WaitForSeconds(finnClips[16].length);
        // Just release the spacebar to deactivate the device.
        if (controls.inZoom){
            if (controls.mobile){
                audioSourceContainer.finnSource.PlayOneShot(finnReleaseMobClip);
                yield return new WaitForSeconds(finnReleaseMobClip.length);
            } else {
                audioSourceContainer.finnSource.PlayOneShot(finnReleaseSpaceBarClip);
                yield return new WaitForSeconds(finnReleaseSpaceBarClip.length);
            }
        }
        StartCoroutine(CheckIfNotInZoom());
    }

    IEnumerator CheckIfNotInZoom(){
        if (!controls.inZoom){
            StartCoroutine(SequenceEight());
            yield break;
        } 
        controls.lockZoom = false;
        zoomSound.disabledZoomSound = false;
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(CheckIfNotInZoom());
    }

    IEnumerator SequenceEight(){
        // Audio fades back to normal. 
        // Protag
        // Is there a button that turns off incompetent leaders?
        audioSourceContainer.protagSource.PlayOneShot(protagClips[1]);
        yield return new WaitForSeconds(protagClips[1].length);

        // Finn 
        // Yeah, it’s right next to the self-destruct… try not to get them mixed up… Oh, and seriously, you’ll need this to get into the overseer’s office;take these. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[17]);
        yield return new WaitForSeconds(finnClips[17].length);

        // The sound of Finn passing the object/the sound of a generic ‘item added to inventory’ signifies that the player has equipped this technology. Player begins walking towards the door.
        // Finn
        // We nabbed a card from a Lambton transport convoy a while back, the door’ll open up when you’re next to it. All you need to do now is remember the cause.  
        audioSourceContainer.protagActionSource.PlayOneShot(addedToInventoryClip);
        yield return new WaitForSeconds(addedToInventoryClip.length);
        audioSourceContainer.finnSource.PlayOneShot(finnClips[18]);
        yield return new WaitForSeconds(finnClips[18].length);

        // Protag
        // Oh, I’ll remember my cause, don’t you worry about that.
        audioSourceContainer.protagSource.PlayOneShot(protagClips[2]);
        yield return new WaitForSeconds(protagClips[2].length);

        // Finn speaks in a distrust worthy tone.
        // Finn
        // Donnie’s ready for you. Go see him. After you’ve found the docs, I’ll be ready to pick you up. Good luck. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[19]);
        yield return new WaitForSeconds(finnClips[19].length);
        Finished();
    }

    void Finished(){
        audioSourceContainer.protagActionSource.PlayOneShot(cutsceneExitClip);
        finished = 1;
        quadbikeFinishedSequence.active = 1;
        StartCoroutine(quadbikeFinishedSequence.Sequence());
        saveAndLoadEncampment.SaveEncampment();
    }
}
