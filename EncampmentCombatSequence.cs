using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EncampmentCombatSequence : SequenceBase
{
    [SerializeField] AudioClip playerAttackClip, distantBirdClip, enterTheRingClip;
    [SerializeField] AudioClip[] loopClips, playerParryClips, playerClips, laraClips, mobileClips, playerBeenHitClips, laraBeenHitClips, laraBeenParriedClips, laraAttackClips, 
    playerPunchClips, laraPunchClips;
    [SerializeField] AudioSource laraSource, distantBirdSource;
    [SerializeField] float maxDistanceFromPlayer, angleToEnemy, playersMaxAngleToMiss;
    [SerializeField] Controls controls;
    int laraAttackInt, laraParryInt, randomInt;
    bool moveLara, tooFarToHit, playerWillMiss, havePlayedDialogue, checkForAttack, checkForParry, hasAttacked, hasParried, canParry;
    [SerializeField] AuditoryZoomSequence auditoryZoomSequence;
    [SerializeField] DecisionPrompt decisionPrompt;
    [SerializeField] LaraCombatSequence laraCombatSequence;
    [SerializeField] GameObject ringObject;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioSourceContainer audioSourceContainer;
    public bool startedSequence;
    [SerializeField] SaveAndLoadEncampment saveAndLoadEncampment;
    [SerializeField] AudioController audioController;


    #region HELPER FUNCTIONS
    void SetClips(){
        if (controls.computer) return;
        laraClips[4] = mobileClips[0];
        laraClips[5] = mobileClips[1];
        laraClips[7] = mobileClips[2];
    }

    void MoveLara(){
        float distanceFromLara = Vector3.Distance(laraSource.transform.position, audioSourceContainer.protagSource.transform.position);
        if (distanceFromLara > maxDistanceFromPlayer){
            tooFarToHit = true;
            if (distanceFromLara > 2f) laraSource.transform.position = Vector3.MoveTowards(laraSource.transform.position, audioSourceContainer.protagSource.transform.position, 0.1f);
            return;
        }
        tooFarToHit = false;
    }
    #endregion

    public override IEnumerator Sequence(){
        yield return new WaitForSeconds(0.5f);
        if (finished == 1) yield break;
        SetClips();
        if (startedSequence){
            audioMixer.SetFloat("Lara_Vol", -10f);
            controls.inCutscene = true;
            laraSource.Stop();
            audioSourceContainer.protagActionSource.PlayOneShot(cutsceneEnterClip);
            yield return new WaitForSeconds(cutsceneEnterClip.length / 2);
            audioSourceContainer.protagSource.transform.position = new Vector3 (laraSource.transform.position.x - 2f, 0.3f, laraSource.transform.position.z);
            yield return new WaitForSeconds(cutsceneEnterClip.length / 2);
            havePlayedDialogue = false;
            checkForAttack = false;
            // Lara
            // Oh listen, it’s the new guy. Finn said I should show you a few moves, hope you don’t mind getting your butt beat by a woman. 
            laraSource.PlayOneShot(laraClips[0]);
            yield return new WaitForSeconds(laraClips[0].length);
            
            // Protag
            // Don't mind
            audioSourceContainer.protagSource.PlayOneShot(playerClips[0]);
            yield return new WaitForSeconds(playerClips[0].length);

            // Lara
            // Get in here... rookie. 
            laraSource.PlayOneShot(laraClips[1]);
            yield return new WaitForSeconds(laraClips[1].length);
            StartCoroutine(EnterTheRing());
        } else {
            yield return new WaitForSeconds(5f); // this is here to stop initial punch sound on load in
            if (startedSequence) yield break;
            laraSource.PlayOneShot(loopClips[Random.Range(0, loopClips.Length)]);
            yield return new WaitForSeconds(loopClips[0].length);
            if (startedSequence) yield break;
            StartCoroutine(Sequence());
        }
    }

    int RandomNumberGen(string type){
        if (type == "Attack"){
            randomInt = Random.Range(0, laraAttackClips.Length);
            if (randomInt == laraAttackInt) randomInt = RandomNumberGen("Attack");
            return randomInt;
        } 
        if (type == "Parry"){
            randomInt = Random.Range(0, laraBeenParriedClips.Length);
            if (randomInt == laraParryInt) randomInt = RandomNumberGen("Parry");
            return randomInt;
        }
        return 0;
    }

    IEnumerator EnterTheRing(){
        // A small cutscene happens whereby the protag hops over a fence. Lara then recaps on fighting. 
        // Lara
        // Alright, you seem like you can handle yourself, let’s go over what you know and then we can refine some stuff from there.
        ringObject.SetActive(true);
        audioSourceContainer.protagSource.transform.position = new Vector3(36.3f, 0.3f, 46.5f);
        pBFootstepSystem.canRotate = false;
        audioSourceContainer.protagActionSource.PlayOneShot(enterTheRingClip, 0.2f);
        yield return new WaitForSeconds(enterTheRingClip.length);
        audioController.PlayMusic("combat", 0.02f);
        pBFootstepSystem.canRotate = true;
        laraSource.PlayOneShot(laraClips[2]);
        yield return new WaitForSeconds(laraClips[2].length);
        controls.inCutscene = false;
        moveLara = true;
        controls.inCombat = true;
        StartCoroutine(YouCantHitEachOtherIfTooFar());
    }

    IEnumerator YouCantHitEachOtherIfTooFar(){
        // Lara
        // Remember, you gotta face your opponent to hit them, and you can't hit them if you’re at the other end of the ring 
        yield return new WaitForSeconds(2f);
        if (tooFarToHit && !laraSource.isPlaying && !havePlayedDialogue){
            laraSource.PlayOneShot(laraClips[3]);
            yield return new WaitForSeconds(laraClips[3].length);
            havePlayedDialogue = true;
            yield return new WaitForSeconds(4f);
            havePlayedDialogue = false;
            StartCoroutine(YouCantHitEachOtherIfTooFar());
            yield break;
        }
        havePlayedDialogue = false;
        StartCoroutine(Attack());
    }

    IEnumerator Attack(){
        // Lara
        // Try holding the space bar for a attack/Try double tap and holding for a attack
        checkForAttack = true;
        if(!havePlayedDialogue){
            laraSource.PlayOneShot(laraClips[4]);
            yield return new WaitForSeconds(laraClips[4].length);
            havePlayedDialogue = true;
            StartCoroutine(Attack());
            yield break;
        }
        if (hasAttacked){ 
            audioSourceContainer.protagSource.Stop();
            audioSourceContainer.protagSource.PlayOneShot(playerAttackClip);
            yield return new WaitForSeconds(playerAttackClip.length);
            laraSource.Stop();
            laraSource.PlayOneShot(playerPunchClips[Random.Range(0, playerPunchClips.Length)]);
            laraSource.PlayOneShot(laraBeenHitClips[Random.Range(0, laraBeenHitClips.Length)]);
            yield return new WaitForSeconds(1f);
            havePlayedDialogue = false;
            checkForAttack = false;
            checkForParry = true;
            StartCoroutine(Parry());
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Attack());
    }

    IEnumerator Parry(){
        // Lara
        // Alright good, guess I could teach you how to parry properly. Most people seem to let out a grunt right before they throw a punch,
        // it’s almost as if they’re just giving you a prompt…
        if(!havePlayedDialogue){
            distantBirdSource.gameObject.SetActive(true);
            laraSource.PlayOneShot(laraClips[6]);
            yield return new WaitForSeconds(laraClips[6].length);
            havePlayedDialogue = true;
            // A distant bird sound with a wind gush denotes the awkwardness of the joke. 
            distantBirdSource.PlayOneShot(distantBirdClip);
            yield return new WaitForSeconds(distantBirdClip.length - 1f);
            // Lara
            // I’ll throw a few punches. You just need to parry. Press enter to parry/swipe up at the right time and you’re golden. 
            laraSource.PlayOneShot(laraClips[7]);
            yield return new WaitForSeconds(laraClips[7].length);
            distantBirdSource.gameObject.SetActive(false);
            StartCoroutine(Parry());
            yield break;
        } 
        if (hasParried){
            havePlayedDialogue = false;
            checkForParry = false;
            StartCoroutine(Finished());
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(EnemyAttack());
    }

    IEnumerator EnemyAttack(){
        laraAttackInt = RandomNumberGen("Attack");
        laraSource.PlayOneShot(laraAttackClips[laraAttackInt]);
        canParry = true;
        yield return new WaitForSeconds(laraAttackClips[laraAttackInt].length);
        canParry = false;
        if (!hasParried) {
            laraSource.PlayOneShot(laraPunchClips[Random.Range(0, laraPunchClips.Length)]);
            audioSourceContainer.protagSource.PlayOneShot(playerBeenHitClips[Random.Range(0, playerBeenHitClips.Length)]);
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyAttack());
            yield break;
        }
        hasParried = false;
        checkForParry = true;
        StartCoroutine(ParryAgain());
    }

    IEnumerator ParryAgain(){
        controls.parry = false;
        hasParried = false;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(EnemyAttackAgain());
    }

    IEnumerator EnemyAttackAgain(){
        laraAttackInt = RandomNumberGen("Attack");
        laraSource.PlayOneShot(laraAttackClips[laraAttackInt]);
        canParry = true;
        yield return new WaitForSeconds(laraAttackClips[laraAttackInt].length);
        canParry = false;
        yield return new WaitForSeconds(2f);
        if (!hasParried) {
            laraSource.PlayOneShot(laraPunchClips[Random.Range(0, laraPunchClips.Length)]);
            audioSourceContainer.protagSource.PlayOneShot(playerBeenHitClips[Random.Range(0, playerBeenHitClips.Length)]);
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(EnemyAttackAgain());
            yield break;
        }
        StartCoroutine(Finished());
    }

    void FinishedWithoutFighting(){
        controls.inCombat = false;
        laraSource.Stop();
        audioSourceContainer.protagSource.Stop();
        StartCoroutine(audioController.FadeMusic());
        moveLara = false;
        finished = 1;
        audioSourceContainer.protagActionSource.PlayOneShot(cutsceneExitClip);
    }

    IEnumerator Finished(){
        StartCoroutine(audioController.FadeMusic());
        controls.inCombat = false;
        laraSource.Stop();
        audioSourceContainer.protagSource.Stop();
        laraSource.PlayOneShot(laraClips[9]);
        yield return new WaitForSeconds(laraClips[9].length - 0.5f);
        moveLara = false;
        audioMixer.SetFloat("Lara_Vol", -5f);
        finished = 1;
        StartCoroutine(CheckDecision());
    }

    IEnumerator CheckDecision(){
        StartCoroutine(decisionPrompt.DecisionLoop());
        decisionPrompt.lightOrDarkDecision = 1;
        StartCoroutine(CheckDecisionLoop());
        yield break;
    }

    IEnumerator CheckDecisionLoop(){
        if (decisionPrompt.lightOrDarkDecision == 3){
            laraSource.PlayOneShot(laraClips[11]);
            yield return new WaitForSeconds(laraClips[11].length);
            laraCombatSequence.enteredCombat = true;
            laraCombatSequence.AssignCoroutine("EnterCombat");
            saveAndLoadEncampment.SaveEncampment();
            yield break;
        } 
        if (decisionPrompt.lightOrDarkDecision == 2){
            // Lara
            // Fair enough you can always choose not to fight.
            laraSource.PlayOneShot(laraClips[10]);
            yield return new WaitForSeconds(laraClips[10].length);
            audioSourceContainer.protagActionSource.PlayOneShot(enterTheRingClip, 0.2f);
            yield return new WaitForSeconds(enterTheRingClip.length);
            auditoryZoomSequence.CheckIfShouldStart();
            audioSourceContainer.protagActionSource.PlayOneShot(cutsceneExitClip);
            ringObject.SetActive(false);
            saveAndLoadEncampment.SaveEncampment();
            yield break;
        }
        if (decisionPrompt.lightOrDarkDecision == 1){
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(CheckDecisionLoop());
            yield break;
        }
    }

    void CheckForParry(){
        if (!controls.parry || !canParry || hasParried) return;    
        laraSource.Stop();
        audioSourceContainer.protagActionSource.PlayOneShot(playerParryClips[Random.Range(0, playerParryClips.Length)]);
        laraParryInt = RandomNumberGen("Parry");
        laraSource.PlayOneShot(laraBeenParriedClips[laraParryInt]);
        hasParried = true;
    }

    void Update(){
        if (checkForAttack && controls.attack) hasAttacked = true;
        if (checkForParry) CheckForParry();
    }

    void FixedUpdate(){if (moveLara) MoveLara();}
}
