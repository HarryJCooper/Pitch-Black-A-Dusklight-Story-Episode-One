using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncampmentCombatSequence : SequenceBase
{
    [SerializeField] AudioClip playerAttackClip, distantBirdClip, enterTheRingClip;
    [SerializeField] AudioClip[] loopClips, playerClips, laraClips, mobileClips, laraBeenHitClips, laraBeenParriedClips, laraAttackClips;
    [SerializeField] AudioSource playerSource, laraSource, distantBirdSource;
    [SerializeField] float maxDistanceFromPlayer, angleToEnemy, playersMaxAngleToMiss;
    [SerializeField] Controls controls;
    int instructionIndex;
    bool moveLara, tooFarToHit, playerWillMiss, havePlayedDialogue, checkForAttack, checkForParry, hasAttacked, hasParried, canParry;
    [SerializeField] AuditoryZoomSequence auditoryZoomSequence;
    [SerializeField] DecisionPrompt decisionPrompt;
    [SerializeField] LaraCombatSequence laraCombatSequence;
    [SerializeField] GameObject ringObject;
    [SerializeField] PBFootstepSystem pBFootstepSystem;

    #region HELPER FUNCTIONS
    void SetClips(){
        if (controls.computer) return;
        laraClips[4] = mobileClips[0];
        laraClips[5] = mobileClips[1];
        laraClips[7] = mobileClips[2];
    }

    void MoveLara(){
        float distanceFromLara = Vector3.Distance(laraSource.transform.position, playerSource.transform.position);
        if (distanceFromLara > maxDistanceFromPlayer){
            tooFarToHit = true;
            if (distanceFromLara > 2f) laraSource.transform.position = Vector3.MoveTowards(laraSource.transform.position, playerSource.transform.position, 0.1f);
            return;
        }
        tooFarToHit = false;
    }
    #endregion

    public override IEnumerator Sequence(){
        yield return new WaitForSeconds(0.5f);
        if (finished == 1) yield break;
        SetClips();
        if (triggered == 1){
            playerSource.PlayOneShot(cutsceneEnterClip);
            yield return new WaitForSeconds(cutsceneEnterClip.length);
            havePlayedDialogue = false;
            checkForAttack = false;
            yield return new WaitForSeconds(2f);
            // Lara
            // Oh listen, it’s the new guy. Finn said I should show you a few moves, hope you don’t mind getting your butt beat by a woman. 
            laraSource.PlayOneShot(laraClips[0]);
            yield return new WaitForSeconds(laraClips[0].length);
            
            // Protag
            // Don't mind
            playerSource.PlayOneShot(playerClips[0]);
            yield return new WaitForSeconds(playerClips[0].length);

            // Lara
            // Get in here... rookie. 
            laraSource.PlayOneShot(laraClips[1]);
            yield return new WaitForSeconds(laraClips[1].length);
            StartCoroutine(CheckIfMovedForward());
        } else {
            laraSource.PlayOneShot(loopClips[Random.Range(0, loopClips.Length)]);
            yield return new WaitForSeconds(loopClips[0].length);
            StartCoroutine(Sequence());
        }
    }

    IEnumerator CheckIfMovedForward(){
        // Checks if player moves away
        yield return new WaitForSeconds(2f);
        if (Vector3.Distance(laraSource.transform.position, playerSource.transform.position) < 20f){
            StartCoroutine(EnterTheRing());
            yield break;
        }
        // Lara
        // Fair enough you can always choose not to fight.
        laraSource.PlayOneShot(laraClips[10]);
        yield return new WaitForSeconds(laraClips[10].length);
        FinishedWithoutFighting();
    }

    IEnumerator EnterTheRing(){
        // A small cutscene happens whereby the protag hops over a fence. Lara then recaps on fighting. 
        // Lara
        // Alright, you seem like you can handle yourself, let’s go over what you know and then we can refine some stuff from there.
        ringObject.SetActive(true);
        playerSource.transform.position = new Vector3(34.3f, 0.3f, 46.5f);
        controls.inCutscene = true;
        pBFootstepSystem.canRotate = false;
        playerSource.PlayOneShot(enterTheRingClip);
        yield return new WaitForSeconds(enterTheRingClip.length);
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
        yield return new WaitForSeconds(5f);
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
            playerSource.Stop();
            playerSource.PlayOneShot(playerAttackClip);
            yield return new WaitForSeconds(playerAttackClip.length);
            laraSource.Stop();
            int i = Random.Range(0, laraBeenHitClips.Length);
            laraSource.PlayOneShot(laraBeenHitClips[i]);
            yield return new WaitForSeconds(laraBeenHitClips[i].length);
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
            laraSource.PlayOneShot(laraClips[6]);
            yield return new WaitForSeconds(laraClips[6].length);
            havePlayedDialogue = true;
            // A distant bird sound with a wind gush denotes the awkwardness of the joke. 
            distantBirdSource.PlayOneShot(distantBirdClip);
            yield return new WaitForSeconds(distantBirdClip.length);
            // Lara
            // I’ll throw a few punches. You just need to parry. Press enter to parry/swipe up at the right time and you’re golden. 
            laraSource.PlayOneShot(laraClips[7]);
            yield return new WaitForSeconds(laraClips[7].length);
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
        int i = Random.Range(0, laraAttackClips.Length);
        laraSource.PlayOneShot(laraAttackClips[i]);
        canParry = true;
        yield return new WaitForSeconds(laraAttackClips[i].length);
        canParry = false;
        yield return new WaitForSeconds(2f);
        if (!hasParried) {
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
        int i = Random.Range(0, laraAttackClips.Length);
        laraSource.PlayOneShot(laraAttackClips[i]);
        canParry = true;
        yield return new WaitForSeconds(laraAttackClips[i].length);
        canParry = false;
        yield return new WaitForSeconds(2f);
        if (!hasParried) {
            StartCoroutine(EnemyAttackAgain());
            yield break;
        }
        StartCoroutine(Finished());
    }

    void FinishedWithoutFighting(){
        controls.inCombat = false;
        laraSource.Stop();
        playerSource.Stop();
        moveLara = false;
        finished = 1;
        playerSource.PlayOneShot(cutsceneExitClip);
    }

    IEnumerator Finished(){
        controls.inCombat = false;
        laraSource.Stop();
        playerSource.Stop();
        laraSource.PlayOneShot(laraClips[9]);
        yield return new WaitForSeconds(laraClips[9].length);
        moveLara = false;
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
            yield break;
        } 
        if (decisionPrompt.lightOrDarkDecision == 2){
            playerSource.PlayOneShot(enterTheRingClip);
            yield return new WaitForSeconds(enterTheRingClip.length);
            auditoryZoomSequence.CheckIfShouldStart();
            playerSource.PlayOneShot(cutsceneExitClip);
            ringObject.SetActive(false);
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
        laraSource.PlayOneShot(laraBeenParriedClips[Random.Range(0, laraBeenParriedClips.Length)]);
        hasParried = true;
    }

    void Update(){
        if (checkForAttack && controls.attack) hasAttacked = true;
        if (checkForParry) CheckForParry();
    }

    void FixedUpdate(){if (moveLara) MoveLara();}
}
