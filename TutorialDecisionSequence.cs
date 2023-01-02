using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DearVR;

public class TutorialDecisionSequence : SequenceBase
{
    [SerializeField] bool skip;
    [SerializeField] DecisionPrompt decisionPrompt;
    [SerializeField] AudioSource babySource, motherSource, windSource, swingSource;
    DearVRSource motherVRSource, swingVRSource;
    [SerializeField] AudioClip[] babyClips, motherClips, mobileClips;
    [SerializeField] AudioClip drinkClip, babyHitClip, bottleDropClip, scoreTutorialWalkingIntroClip;
    [SerializeField] Controls controls;
    [SerializeField] TutorialWalkingSequence tutorialWalkingSequence;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] AudioController audioController;
    [SerializeField] AmbienceRepeater ambienceRepeater;
    [SerializeField] Text instructionText;
    AudioMixer audioMixer;
    [SerializeField] string[] mobileWords, computerWords;
    string[] words;

    IEnumerator WriteSentence(){
        foreach (string word in words){
            instructionText.text = instructionText.text += (word + " ");
            yield return new WaitForSeconds(0.3f);
        }
        yield break;
    }

    // REMOVE ON BUILD
    void Awake(){
        active = 1;
        motherVRSource = motherSource.GetComponent<DearVRSource>();
        swingVRSource = swingSource.GetComponent<DearVRSource>();
        motherVRSource.performanceMode = true;
        swingVRSource.performanceMode = true;
        instructionText.text = "";
        if (controls.mobile) instructionText.fontSize = 72;
    }

    void Setup(){
        audioController.SetCutOffToZero();
        babySource.transform.position = new Vector3(0, 0.3f, 0);
        audioMixer = swingSource.outputAudioMixerGroup.audioMixer;
        audioMixer.SetFloat("Mother_InitialVol", 6f);
        audioMixer.SetFloat("WalkingTutorialReverb_Vol", -80f);
        audioMixer.SetFloat("StealthTutorialReverb_Vol", -80f);
        if (controls.mobile) motherClips[1] = mobileClips[0];
        swingSource.loop = true;
        swingVRSource.DearVRPlay();
        windSource.loop = true;
        windSource.Play();
        StartCoroutine(ambienceRepeater.ambienceCoroutine);
    }

    public override IEnumerator Sequence(){
        if (skip && Application.isEditor){
            SkipToNextSequence();
            yield break;
        }
        controls.inCutscene = true;
        controls.canZoom = false;
        pBFootstepSystem.canRotate = false;
        pBFootstepSystem.canMove = false;
        yield return new WaitForSeconds(10f);
        Setup();
        // INT. HOUSE – DAY 
        // Player is subject to a series of tutorials that revolve around the protagonist’s early life. The first tutorial is a simple choice mechanics whereby le baby needs to drink or to not drink. 
        // Mother is a haunted, volatile character. She is a creepy mum stereotype with mental instability. Her tone is volatile as she switches from nice and innocent into twisted and horrific. A high pitch voice whimpers and breathes heavy on occasion. 
        yield return new WaitForSeconds(5f);
        // Baby Protag 
        // Waa, waa. 
        babySource.PlayOneShot(babyClips[0]);
        yield return new WaitForSeconds(babyClips[0].length);

        // Mother
        // Okay, okay, little boy… I've got something for you. You want a little drink of water? 
        motherVRSource.DearVRPlayOneShot(motherClips[0]);
        yield return new WaitForSeconds(motherClips[0].length);

        // Baby Protag
        // Ga.
        babySource.PlayOneShot(babyClips[1]);
        yield return new WaitForSeconds(babyClips[1].length);

        // Mother
        // Just swipe right if you wanna, and left if you don’t/double tap left if you wanna, and right if you don’t.
        if (controls.mobile){
            words = mobileWords;
        } else {
            words = computerWords;
        }
        StartCoroutine(WriteSentence());
        motherVRSource.DearVRPlayOneShot(motherClips[1]);
        yield return new WaitForSeconds(motherClips[1].length);

        decisionPrompt.lightOrDarkDecision = 1;
        StartCoroutine(decisionPrompt.DecisionLoop());
        StartCoroutine(SequenceLoop());
    }

    IEnumerator SequenceLoop(){
        if (decisionPrompt.lightOrDarkDecision == 1){
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SequenceLoop());
        } else {
            instructionText.text = "";
            if (decisionPrompt.lightOrDarkDecision == 2){
                StartCoroutine(SequenceLight());
            } else {
                StartCoroutine(SequenceDark());
            }
        }
    }

    IEnumerator SequenceLight(){
        // If player drinks then ‘the light’ choice fx sounds and the Mother remarks. 
        babySource.PlayOneShot(drinkClip);
        yield return new WaitForSeconds(drinkClip.length);
        
        // Baby Protag 
        // Hehehe.
        babySource.PlayOneShot(babyClips[2]);
        yield return new WaitForSeconds(babyClips[2].length);

        // Mother 
        // Aww well done. Always remember baby, all you’ve got in this world are choices… 
        motherVRSource.DearVRPlayOneShot(motherClips[2]);
        yield return new WaitForSeconds(motherClips[2].length);
        // Mother’s tone turns to sinister and leans into a dark and creepy cadence. A musical element and rising fx trigger too, heightening the cinematic effect and tension.
        // Mother
        // Even if they’re not yours to make… Drink up.
        motherVRSource.DearVRPlayOneShot(motherClips[3]);
        yield return new WaitForSeconds(motherClips[3].length);
        StartCoroutine(Outro());
    }

    IEnumerator SequenceDark(){
        babySource.PlayOneShot(babyHitClip);
        yield return new WaitForSeconds(babyHitClip.length - 0.8f);
        babySource.PlayOneShot(bottleDropClip);
        yield return new WaitForSeconds(bottleDropClip.length - 1f);

        // If player chooses not to drink then the baby knocks over the glass and cries as the ‘dark choice’ sfx triggers, and mother remarks. 
        // Baby Protag
        // Waaaa
        babySource.PlayOneShot(babyClips[3]);
        yield return new WaitForSeconds(babyClips[3].length);

        // Mother
        // Ahh you LITTLE! You and your brother will never get it… you can’t live in this world without playing by it’s rules. It’s not up to you *whimper* just you wait… 
        motherVRSource.DearVRPlayOneShot(motherClips[4]);
        yield return new WaitForSeconds(motherClips[4].length);

        // A musical element and rising fx trigger too, heightening the cinematic effect and tension, and leading to the next tutorial.
        StartCoroutine(Outro());
    }

    IEnumerator Outro(){
        StartCoroutine(audioController.ReduceMasterCutOff(7.5f));
        yield return new WaitForSeconds(5f);
        babySource.PlayOneShot(scoreTutorialWalkingIntroClip);
        yield return new WaitForSeconds(2f);
        ambienceRepeater.StopAllSources();
        swingVRSource.DearVRStop();
        controls.inCutscene = false;
        pBFootstepSystem.canRotate = true;
        pBFootstepSystem.canMove = true;
        yield return new WaitForSeconds(3f);
        finished = 1;
        audioMixer.SetFloat("Mother_InitialVol", 0f);
        StartCoroutine(tutorialWalkingSequence.Sequence());
    }

    void SkipToNextSequence(){
        finished = 1;
        StartCoroutine(tutorialWalkingSequence.Sequence());
    }
}
