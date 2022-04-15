using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class TutorialCombatSequence : SequenceBase
{
    [SerializeField] AudioClip kidOneTauntClip, kidOneBeenHitClip, kidOneBeenParriedClip, kidOneAttackClip, playerAttackClip, tauntClip, tauntClipComp, tauntClipMob;
    [SerializeField] AudioClip[] charlieClips, playerClips, playerFootstepClips, kidOneClips, kidTwoClips, mobileControlClips, compControlClips, controlClips;
    [SerializeField] AudioSource charlieSource, playerSource, protagReverbSource, kidOneSource, kidTwoSource;
    AudioMixer audioMixer;
    [SerializeField] float maxDistanceFromPlayer, angleToEnemy, playersMaxAngleToMiss;
    [SerializeField] Controls controls;
    [SerializeField] Combat combat;
    [SerializeField] GameObject motherObject, walkingTutorialWalls, stealthTutorialWalls, combatTutorialWalls, ambienceObject;
    float tauntWaiter;
    int instructionIndex, waitCounter;
    bool moveKidOne, tooFarToHit, playerWillMiss, havePlayedDialogue, checkForAttack, checkForParry, shouldTaunt, hasTaunted, hasAttacked, hasParried, canParry, inRange;
    [SerializeField] AudioController audioController;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] AmbienceRepeater ambienceRepeater, scuffleAmbienceRepeater;

    void PlayOneShotWithVerb(AudioClip clip){
        playerSource.PlayOneShot(clip);
        protagReverbSource.PlayOneShot(clip);
    }

    // REMOVE ON BUILD
    void Awake(){active = 0;}

    #region HELPER FUNCTIONS
    void SetPositions(){
        kidOneSource.transform.position = new Vector3(-10.2f, 0.3f, 0);
        kidTwoSource.transform.position = new Vector3(10.2f, 0.3f, 0);
        charlieSource.transform.position = new Vector3(-8.2f, 0.3f, 0);
        playerSource.transform.position = new Vector3(0, 0.3f, -20);
    }

    void SetActiveOrDisable(){
        ambienceObject.SetActive(false);
        walkingTutorialWalls.SetActive(false);
        stealthTutorialWalls.SetActive(false);
        combatTutorialWalls.SetActive(true);
    }

    void SetClips(){
        if(controls.mobile){
            controlClips = mobileControlClips;
            tauntClip = tauntClipMob;
        } else {
            controlClips = compControlClips;
            tauntClip = tauntClipComp;
        };
        pBFootstepSystem.footstepClips = playerFootstepClips;
        audioMixer = charlieSource.outputAudioMixerGroup.audioMixer;
        StartCoroutine(ambienceRepeater.ambienceCoroutine);
        ambienceRepeater.stopped = false;
        StartCoroutine(scuffleAmbienceRepeater.ambienceCoroutine);
        audioMixer.SetFloat("Ambience_Vol", -60f);
        audioMixer.SetFloat("WalkingTutorialReverb_Vol", -80f);
        audioMixer.SetFloat("StealthTutorialReverb_Vol", -80f);
        audioMixer.SetFloat("CombatTutorialReverb_Vol", 0f);
    }

    void MoveKidOne(){
        if (Vector3.Distance(kidOneSource.transform.position, playerSource.transform.position) > maxDistanceFromPlayer){
            tooFarToHit = true;
            kidOneSource.transform.position = Vector3.MoveTowards(kidOneSource.transform.position, playerSource.transform.position, 0.1f);
            return;
        }
        tooFarToHit = false;
    }
    #endregion

    public override IEnumerator Sequence(){
        SetClips();
        SetPositions();
        SetActiveOrDisable();
        StartCoroutine(audioController.IncreaseMasterCutOff(10f));
        yield return new WaitForSeconds(12f);
        StartCoroutine(ScuffleLoop());
    }

    IEnumerator ScuffleLoop(){
        yield return new WaitForSeconds(0.5f);
        if (Vector3.Distance(kidOneSource.transform.position, playerSource.transform.position) > 10){
            StartCoroutine(ScuffleLoop());
            yield break;
        }
        scuffleAmbienceRepeater.stopped = true;
        StartCoroutine(ContinueSequence());
    }

    IEnumerator ContinueSequence(){
        controls.inCutscene = true;
        havePlayedDialogue = false;
        // EXT. STREET – NIGHT.
        // The next tutorial is for combat. The protag is around 14 here. The protag’s brother, Charlie, is in the street having a verbal exchange with Kid 1 and Kid 2. 
        // Their voices are noticeably menacing in comparison to Protag and Charlie. The protag can hear them and moves towards the fight. 
        // Kid 1 
        // You and your brother are messed up! 
        kidOneSource.PlayOneShot(kidOneClips[0]);
        yield return new WaitForSeconds(kidOneClips[0].length);
        
        // Kid 2 
        // And your Ma? Don’t even get us started!  
        kidTwoSource.PlayOneShot(kidTwoClips[0]);
        yield return new WaitForSeconds(kidTwoClips[0].length);
        
        // Protag
        // Charlie, move! What the hell’s going on?!
        PlayOneShotWithVerb(playerClips[0]);
        yield return new WaitForSeconds(playerClips[0].length);

        // Charlie
        // They started talking whack about ma, little brother.
        charlieSource.PlayOneShot(charlieClips[0]);
        yield return new WaitForSeconds(charlieClips[0].length);

        // Kid 2
        // Younger brother’s come to save the day huh?
        kidTwoSource.PlayOneShot(kidTwoClips[1]);
        yield return new WaitForSeconds(kidTwoClips[1].length);
        
        // Charlie
        // You know what? Pa was good for one thing, teaching me how to fight. It’s about time I carried the family torch. These losers are all yours, brother. I’ll show you what Pa showed me.  
        charlieSource.PlayOneShot(charlieClips[1]);
        yield return new WaitForSeconds(charlieClips[1].length);

        moveKidOne = true;
        controls.inCutscene = false;
        StartCoroutine(YouCantHitEachOtherIfTooFar());
    }

    IEnumerator YouCantHitEachOtherIfTooFar(){
        audioController.PlayMusic("combat");
        // You can't hit me from all the way overthere!
        yield return new WaitForSeconds(5f);
        if(tooFarToHit && !kidOneSource.isPlaying && !havePlayedDialogue){
            kidOneSource.PlayOneShot(kidOneClips[1]);
            yield return new WaitForSeconds(kidOneClips[1].length);
            havePlayedDialogue = true;
            yield return new WaitForSeconds(4f);
            havePlayedDialogue = false;
            StartCoroutine(YouveGotToFaceHim());
            yield break;
        } 
        havePlayedDialogue = false;
        StartCoroutine(YouveGotToFaceHim());
    }

    IEnumerator YouveGotToFaceHim(){
        // You’ve got to face him to hit or parry (as player gets closer)
        angleToEnemy = Quaternion.Angle(playerSource.transform.rotation, kidOneSource.transform.rotation);
        
        if (!havePlayedDialogue){
            charlieSource.PlayOneShot(controlClips[1]);
            yield return new WaitForSeconds(controlClips[1].length);
            controls.inCombat = true;
            havePlayedDialogue = true;
            StartCoroutine(YouveGotToFaceHim());
            yield break;
        } 
        if (angleToEnemy < playersMaxAngleToMiss){
            havePlayedDialogue = false;
            StartCoroutine(Parry());
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(YouveGotToFaceHim());
    }

    IEnumerator Parry(){
        // Swipe up to parry, do it when you hear him attack!
        if(!havePlayedDialogue){
            charlieSource.PlayOneShot(controlClips[3]);
            yield return new WaitForSeconds(controlClips[3].length);
            havePlayedDialogue = true;
            StartCoroutine(Parry());
            yield break;
        } 
        if (hasParried){
            havePlayedDialogue = false;
            checkForParry = false;
            StartCoroutine(AttackHim());
            yield break;
        } 
        yield return new WaitForSeconds(0.5f);
        checkForParry = true;
        StartCoroutine(EnemyAttack());
    }

    IEnumerator EnemyAttack(){
        kidOneSource.PlayOneShot(kidOneAttackClip);
        canParry = true;
        yield return new WaitForSeconds(kidOneAttackClip.length);
        canParry = false;
        yield return new WaitForSeconds(2f);
        if (!hasParried) {
             StartCoroutine(EnemyAttack());
             yield break;
        }
        StartCoroutine(AttackHim());
    }

    IEnumerator AttackHim(){
        // Attack him by double tapping/tapping space while he's dazed from the parry.
        charlieSource.PlayOneShot(controlClips[4]);
        yield return new WaitForSeconds(controlClips[4].length);
        checkForAttack = true;
        hasAttacked = false;
        StartCoroutine(WaitForAttack());
    }

    IEnumerator WaitForAttack(){
        controls.attack = false;
        if (hasAttacked){
            playerSource.Stop();
            PlayOneShotWithVerb(playerAttackClip);
            yield return new WaitForSeconds(playerAttackClip.length);
            kidOneSource.Stop();
            kidOneSource.PlayOneShot(kidOneBeenHitClip);
            yield return new WaitForSeconds(kidOneBeenHitClip.length);
            havePlayedDialogue = false;
            checkForAttack = false;
            StartCoroutine(Finished());
            yield break;
        } 
        yield return new WaitForSeconds(0.5f);
        waitCounter += 1;
        if (waitCounter > 10){
            kidOneSource.PlayOneShot(tauntClip);
            yield return new WaitForSeconds(tauntClip.length);
        }
        StartCoroutine(WaitForAttack());
    }

    IEnumerator Finished(){
        StopCoroutine(ambienceRepeater.ambienceCoroutine);
        moveKidOne = false;
        StartCoroutine(audioController.FadeMusic());
        yield return new WaitForSeconds(0.1f);
        kidOneSource.Stop();
        playerSource.Stop();
        // Charlie 
        // Well done little brother! Remember, family always look out for each other… no matter what. 
        charlieSource.PlayOneShot(charlieClips[2]);
        yield return new WaitForSeconds(charlieClips[2].length);
    }

    void CheckForAttack(){
        if (!checkForAttack) return;
        if (controls.attack) hasAttacked = true;
        if (!shouldTaunt){
            tauntWaiter += Time.deltaTime;
            if (tauntWaiter >= 5) shouldTaunt = true;
        }
    }

    void Update(){
        if (checkForParry && controls.parry && canParry){
            Debug.Log("has parried in the tutorial combat sequence");
            hasParried = true;
            kidOneSource.Stop();
            kidOneSource.PlayOneShot(kidOneBeenParriedClip);
        }
        CheckForAttack();
    }

    void FixedUpdate(){
        if (moveKidOne) MoveKidOne();
    }
}
