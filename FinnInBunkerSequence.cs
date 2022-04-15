using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FinnInBunkerSequence : SequenceBase
{
    public bool skipIntroMusic;
    [SerializeField] EnemyFootsteps finnFootsteps;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioClip[] finnClips, finnLoopClips, protagClips, doorClips, radioClips, playerFootstepClips, finnDesertFootstepClips;
    [SerializeField] AudioClip openingStatement, innerDoorOpeningClip;
    [SerializeField] AudioSourceContainer audioSourceContainer;
    [SerializeField] AudioSource protagReverbSource, initialSource, innerDoorSource;
    int finnLoopClipsInt = 0;
    [SerializeField] float maxDistanceFromPlayer;
    [SerializeField] GameObject door, innerDoor;
    [SerializeField] Controls controls;
    Transform finnTransform, doorTransform, playerTransform;
    bool moveFinnRight, moveFinnLeft, moveFinnForward, moveFinnTowardsRadio, moveFinnToCentreOfBunker, moveDoorOpen, moveDoorShut, breakLoop, checkPosition;
    public SecretSequence secretSequence;
    [SerializeField] AudioController audioController;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] AmbienceRepeater drippingRepeater, electricalRepeater, desertRepeater, openDoorAmbienceRepeater, bunkerAmbienceRepeater; 
    [SerializeField] SaveAndLoadEncampment saveAndLoadEncampment;
    [SerializeField] GameObject bunkerObject;

    int RandomNumberGen(){
        int randomInt = Random.Range(0, finnLoopClips.Length);
        if (randomInt == finnLoopClipsInt) randomInt = RandomNumberGen();
        return randomInt;
    }

    void Awake(){ active = 1;}

    void PlayOneShotWithVerb(AudioClip clip){
        audioSourceContainer.protagSource.PlayOneShot(clip);
        protagReverbSource.PlayOneShot(clip);
    }

    void Setup(){
        StartCoroutine(bunkerAmbienceRepeater.ambienceCoroutine);
        audioMixer = protagReverbSource.outputAudioMixerGroup.audioMixer;
        controls.canZoom = false;
        moveFinnRight = false;
        moveFinnLeft = false;
        moveFinnTowardsRadio = false;
        moveFinnToCentreOfBunker = false;
        moveDoorOpen = false;
        moveDoorShut = false;
        audioSourceContainer.radioSource.PlayOneShot(radioClips[0]);
        finnTransform = audioSourceContainer.finnSource.transform;
        doorTransform = door.transform;
        playerTransform = audioSourceContainer.protagSource.transform;
        finnTransform.position = new Vector3(0f, 0.3f, 10f);
        playerTransform.position = new Vector3(0f, 0.3f, -30f);
        pBFootstepSystem.footstepClips = playerFootstepClips;
        StartCoroutine(drippingRepeater.ambienceCoroutine);
        StartCoroutine(electricalRepeater.ambienceCoroutine);
        checkPosition = true;
    }

    void MoveFinnTowardsRadio(){ finnTransform.position = Vector3.MoveTowards(finnTransform.position, audioSourceContainer.radioSource.transform.position, 0.1f);}
    void MoveFinnToCentreOfBunker(){finnTransform.position = Vector3.MoveTowards(finnTransform.position, new Vector3(0, 0.3f, 10f), 0.1f);}
    // set move direction to Vector3, then call moveFinn
    void MoveFinnRight(){finnTransform.position += new Vector3(0.07f, 0, 0);}
    void MoveFinnLeft(){finnTransform.position += new Vector3(-0.07f, 0, 0);}
    void MoveFinnForward(){finnTransform.position += new Vector3(0, 0, 0.1f);}
    void MoveDoorOpen(){doorTransform.position += new Vector3(0.07f, 0, 0);}
    void MoveDoorShut(){doorTransform.position -= new Vector3(0.07f, 0, 0);}

    IEnumerator MoveTimer(float time){
        moveFinnRight = true;
        yield return new WaitForSeconds(time / 2);
        moveFinnRight = false;
        moveFinnLeft = true;
        yield return new WaitForSeconds(time);
        moveFinnLeft = false;
        moveFinnRight = true;
        yield return new WaitForSeconds(time / 2);
        moveFinnRight = false;
    }

    public override IEnumerator Sequence(){
        if (finished == 0){
            audioController.increaseCutOffAtStart = false;
            if (skipIntroMusic && Application.isEditor){
                yield return new WaitForSeconds(0.5f);
                Setup();
                StartCoroutine(audioController.IncreaseMasterCutOff(2f));
                yield return new WaitForSeconds(2f);
                StartCoroutine(SequenceLoop());
                yield break;
            }
            initialSource.PlayOneShot(openingStatement);
            yield return new WaitForSeconds(openingStatement.length - 15f);
            Setup();
            StartCoroutine(audioController.IncreaseMasterCutOff(15f));
            yield return new WaitForSeconds(15f);

            // INT. BUNKER – NIGHT
            // The player awakes in a cold bleak concrete room. The clanging of chains and dripping of water can be heard as the title music blends and 
            // fades to the sound of whistling, continuing the melody featured in the title music. As the player approaches an open door, the whistling halts, and the 
            // sound of a troubled Irish voice is heard, accompanied by the sound of heavy footsteps. The voice is muttering, as if the speaker is practicing something. 
            // The voice stutters and stammers in an attempt to read clearly and strongly. 
            StartCoroutine(SequenceLoop());            
        }
    }

    IEnumerator SequenceLoop(){
        // Finn
        // No longer will The Nightlanders be trapped. No longer will… Aggghh that’s just not it! 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[0]);
        yield return new WaitForSeconds(finnClips[0].length + 4f);
        if (breakLoop) yield break;

        // The smashing of glasses/objects can be heard as the voice begins to pace in frustration. He repeats the sentence again as if he is trying to retrace his speech. 
        // He mutters softly.
        StartCoroutine(MoveTimer(3.2f));
        if (breakLoop) yield break;

        // Finn
        // No longer will we be under the big man’s boot. No longer will we be afraid in the face of… 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[1]);
        yield return new WaitForSeconds(finnClips[1].length);
        if (breakLoop) yield break;

        // Finn clicks his fingers.
        // Finn
        // Nearly had it that time. Come on, come on. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[2]);
        yield return new WaitForSeconds(finnClips[2].length + Random.Range(4f, 7f));
        if (breakLoop) yield break;

        // The voice pauses for a few seconds, noticing the radios are becoming an issue. 
        // Finn
        // No longer will the moths take control of our days and nights.
        audioSourceContainer.finnSource.PlayOneShot(finnClips[3]);
        yield return new WaitForSeconds(finnClips[3].length + Random.Range(1f, 2f));
        if (breakLoop) yield break;

        // His voice gets louder as his thoughts become clearer and more confident as his speech practice develops.
        // Finn
        // No longer will they… torture and decimate our people. No longer wi… aggghh
        audioSourceContainer.finnSource.PlayOneShot(finnClips[4]);
        yield return new WaitForSeconds(finnClips[4].length + Random.Range(1f, 2f));
        if (breakLoop) yield break;

        // The voice lets out an angry grunt as he descends back into an unconfident frustrated stupor. He curses, pacing around the room like a frustrated child. 
        // He begins to speak again, but this time, the tone feels more genuine and less synthesised. The room is silent for a few seconds before the voice begins 
        // to speak in a softer tone, accompanied by a rising string sound denoting hope and valor.
        StartCoroutine(MoveTimer(3.2f));
        if (breakLoop) yield break;


        // Finn
        // Our group has been growing, growing stronger, smarter, and most importantly… louder. Our voices will be heard by those devils. And they will listen. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[5]);
        yield return new WaitForSeconds(finnClips[5].length + Random.Range(7f, 10f));
        if (breakLoop){
            yield break; 
        } else {
            StartCoroutine(SequenceLoop());
        }
    }

    IEnumerator SequenceContinue(){
        innerDoorSource.PlayOneShot(innerDoorOpeningClip);
        yield return new WaitForSeconds(0.2f);
        innerDoor.SetActive(false);
        audioSourceContainer.finnSource.Stop();
        yield return new WaitForSeconds(innerDoorOpeningClip.length);
        controls.inCutscene = true;
        playerTransform.position = new Vector3(0, 0.3f, 0);
        // Protag 
        // Nice speech, nearly had me listening, I’m almost inspired. 
        PlayOneShotWithVerb(protagClips[0]);
        yield return new WaitForSeconds(protagClips[0].length);
        audioSourceContainer.finnSource.transform.position = new Vector3(0, 0.3f, 10);
        // Finn
        // Ahh I thought I heard ya creeping in. I’ll take that as a serious compliment, unless you want to help out of course. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[6]);
        yield return new WaitForSeconds(finnClips[6].length);

        // The protagonist speaks in a resentfully displeased tone. He gets to the point.
        // Protag
        // Think I’ll leave the public speaking to the pros. You called? 
        PlayOneShotWithVerb(protagClips[1]);
        yield return new WaitForSeconds(protagClips[1].length);

        // Finn responds in a patronising tone, before instructing the protag in a more hostile manner.
        // Finn
        // I did, and you came. Turn that radio off, would you? If I keep analysing The Emperor’s ramblings, I might actually start believing them. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[7]);
        yield return new WaitForSeconds(finnClips[7].length);

        // Protag utters the next line with a discreetly, jokingly dismissive tone. 
        // Protag
        // Do it yourself. 
        PlayOneShotWithVerb(protagClips[2]);
        yield return new WaitForSeconds(protagClips[2].length);

        // Protag’s tone turns combative and pauses between dialogue, combined with Finn’s reaction, create tension in the room. 
        // Protag
        // I’m not your dog. 
        PlayOneShotWithVerb(protagClips[3]);
        yield return new WaitForSeconds(protagClips[3].length);

        // Finn
        // No… dogs are far more obedient. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[8]);
        yield return new WaitForSeconds(finnClips[8].length);

        // Finn walks over to the radio and switches it off. There’s a second pause between finishing the dialogue and turning the radio off, and the ambience quietly fades 
        // just for a second as Finn flicks the switch. This tense pause denotes resentment and distrust. The next line is spoken in a lighter tone than the last, but still 
        // not the full flare we hear of Finn throughout (ease back into comedic tone). 
        moveFinnTowardsRadio = true;
        yield return new WaitForSeconds(3f);
        moveFinnTowardsRadio = false;
        audioSourceContainer.radioSource.Stop();
        audioSourceContainer.radioSource.PlayOneShot(radioClips[1]);
            // turn radio off sound.
        // /a loud action interrupts the tense silence… maybe a bang at the door (would need to shuffle the next bang around – the flick of the radio turning off maybe too subtle)- Severe white noise cut off with glitchy fx and delayed emperor voice.
        // Finn speaks in a very sincere tone. A sense of desperation breaks through every now and again, but is covered by his needs to appear strong as a leader. 

        // Finn 
        // Let’s talk business. 
        moveFinnToCentreOfBunker = true;
        audioSourceContainer.finnSource.PlayOneShot(finnClips[9]);
        yield return new WaitForSeconds(finnClips[9].length);

        // Protag 
        // Let’s
        PlayOneShotWithVerb(protagClips[4]);
        yield return new WaitForSeconds(protagClips[4].length);
        moveFinnToCentreOfBunker = false;

        // Finn
        // Now, that Church, Lambton Energy, they’ve imprisoned The people of Dusklight for too long, and we’re not doing enough to help them. Robbing transports, taking out officials, it’s too slow. We need to step up. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[10]);
        yield return new WaitForSeconds(finnClips[10].length);

        // Protag
        // What’s the play? 
        PlayOneShotWithVerb(protagClips[5]);
        yield return new WaitForSeconds(protagClips[5].length);

        // Finn 
        // It’s simple; for the Nightlander name to be screamed from the rooftops, for the chains of Dusklight’s oppressors to crash… we need to use our brains.
        audioSourceContainer.finnSource.PlayOneShot(finnClips[11]);
        yield return new WaitForSeconds(finnClips[11].length);

        // Protag
        // Great.
        PlayOneShotWithVerb(protagClips[6]);
        yield return new WaitForSeconds(protagClips[6].length);

        // Finn 
        // It’s simple, we need to work smarter. Next mission should be an easy smash and grab, for you anyway.
        audioSourceContainer.finnSource.PlayOneShot(finnClips[12]);
        yield return new WaitForSeconds(finnClips[12].length);

        // Protag 
        // Should be… 
        PlayOneShotWithVerb(protagClips[7]);
        yield return new WaitForSeconds(protagClips[7].length);

        // Finn
        // Should be, yes, you can listen. We need to steal some documents in a near-by pumping station. Head in, get out, then bring them to me. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[13]);
        yield return new WaitForSeconds(finnClips[13].length);

        // Protag 
        // What’s in them? 
        PlayOneShotWithVerb(protagClips[8]);
        yield return new WaitForSeconds(protagClips[8].length);

        // Finn
        // Not sure. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[14]);
        yield return new WaitForSeconds(finnClips[14].length);

        // Protag
        // Not sure? 
        PlayOneShotWithVerb(protagClips[9]);
        yield return new WaitForSeconds(protagClips[9].length);

        // Finn
        // Not yet. That’s why you’re going to get them. One of our top sources tipped us off. And when that source has something, it’s usually worth it’s weight.
        audioSourceContainer.finnSource.PlayOneShot(finnClips[15]);
        yield return new WaitForSeconds(finnClips[15].length);

        // Protag
        // Not enough, need more info, don’t like working in the dark.  
        PlayOneShotWithVerb(protagClips[10]);
        yield return new WaitForSeconds(protagClips[10].length);

        // Finn
        // Champ, you’re with the Nightlanders now, fighting in the dark is all you can do. Realise that or those light worshipping loonies’ll flush us out, and Dusklight’ll be in the dark forever. Tthat can’t happen. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[16]);
        yield return new WaitForSeconds(finnClips[16].length);

        // A few seconds of silence pass. Finn presses a buzzer, and his bunker door opens, revealing the ambience of the encampment outside.  
        moveFinnTowardsRadio = true;
        yield return new WaitForSeconds(3f);
        moveFinnTowardsRadio = false;
        audioSourceContainer.doorSource.PlayOneShot(doorClips[0]);
        yield return new WaitForSeconds(doorClips[0].length);
        audioSourceContainer.doorSource.PlayOneShot(doorClips[1]);
        audioMixer.SetFloat("Room_Vol", -50f);
        moveDoorOpen = true;
        StartCoroutine(openDoorAmbienceRepeater.ambienceCoroutine);
        yield return new WaitForSeconds(6f);
        moveDoorOpen = false;

        // Finn
        // Right, let’s get to it; church isn’t gonna topple itself. Follow me. 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[17]);
        yield return new WaitForSeconds(finnClips[17].length);
        StopCoroutine(bunkerAmbienceRepeater.ambienceCoroutine);
        bunkerAmbienceRepeater.stopped = true;
        finnFootsteps.footstepClips = finnDesertFootstepClips;
        moveFinnForward = true;
        yield return new WaitForSeconds(6f);
        moveFinnForward = false;
        StartCoroutine(SequenceLoopTwo());
    }

    IEnumerator SequenceLoopTwo(){
        controls.inCutscene = false;
        if (Vector3.Distance(finnTransform.position, playerTransform.position) > maxDistanceFromPlayer){
            // Finn 
            // Come on ya dafty, this way! etc.
            finnLoopClipsInt = RandomNumberGen();
            audioSourceContainer.finnSource.PlayOneShot(finnLoopClips[finnLoopClipsInt]);
            yield return new WaitForSeconds(7.5f);
            StartCoroutine(SequenceLoopTwo());
        } else {
            StartCoroutine(SequenceTwo());
        }
    }

    IEnumerator SequenceTwo(){
        StopCoroutine(drippingRepeater.ambienceCoroutine);
        StopCoroutine(electricalRepeater.ambienceCoroutine);
        StopCoroutine(openDoorAmbienceRepeater.ambienceCoroutine);
        controls.inCutscene = true;
        // Finn
        // Donnie’ll hook you up with a quadbike but *tut* I don’t know whether it’s ready yet. Get chatting to the others if you have to wait. Oh, and Lara said she wanted to see how well you actually fight… 
        audioSourceContainer.finnSource.PlayOneShot(finnClips[19]);
        yield return new WaitForSeconds(finnClips[19].length);

        // Finn switches to a dismissive yet somewhat threatened voice. 
        // Finn
        // given your big mouth and all… 
        // audioSourceContainer.finnSource.PlayOneShot(finnClips[20]); - TO DO
        // yield return new WaitForSeconds(finnClips[20].length);

        // Protag 
        // I’ll see if I can get a word in. 
        PlayOneShotWithVerb(protagClips[11]);
        yield return new WaitForSeconds(protagClips[11].length);

        // Finn speaks in a lighter tone for the next line, almost as though he is awkwardly indoctrinating the protag. 
        // The player stands and Finn continues to speak. Player will stand automatically (probably)
        Finished();
        controls.inCutscene = false;
        moveFinnToCentreOfBunker = true;
        yield return new WaitForSeconds(6f);
        moveFinnToCentreOfBunker = false;
        moveDoorShut = true;
        audioSourceContainer.doorSource.PlayOneShot(doorClips[0]);
        audioSourceContainer.doorSource.PlayOneShot(doorClips[1]);
        yield return new WaitForSeconds(6f);
        moveDoorShut = false;
    }

    public void Finished(){
        audioSourceContainer.protagSource.PlayOneShot(cutsceneExitClip);
        secretSequence.active = 1;
        StartCoroutine(secretSequence.Sequence());
        finished = 1;
        saveAndLoadEncampment.SaveEncampment();
        bunkerObject.SetActive(false);
    }

    void FixedUpdate(){
        if(moveDoorOpen) MoveDoorOpen();
        if(moveDoorShut) MoveDoorShut();
        if(moveFinnLeft) MoveFinnLeft();
        else if(moveFinnRight) MoveFinnRight();
        else if(moveFinnForward) MoveFinnForward();
        else if(moveFinnTowardsRadio) MoveFinnTowardsRadio();
        else if(moveFinnToCentreOfBunker) MoveFinnToCentreOfBunker();
    }

    void Update(){
        if (audioSourceContainer.protagSource.transform.position.z > -6 && !breakLoop && checkPosition){
            breakLoop = true;
            StartCoroutine(SequenceContinue());
        } 
    }
}
