using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DearVR;

public class LaraCombatSequence : MonoBehaviour
{
    [SerializeField] float maxDistanceFromPlayer = 3, enemyMoveSpeed;
    float angleToEnemy, enemyAttackSpeed, distanceFromPlayer, musicVol;
    Controls controls;
    AudioSource playerSource, enemySource;
    DearVRSource enemyVRSource, playerVRSource;
    [SerializeField] AudioSource playerActionSource;
    bool moveTowards, moveBackAndToSide, moveToPointOne, moveToPointTwo, reachedPointOne, reachedPointTwo, enemyInRange, canParry, attacking, reduceMusic;
    public bool enteredCombat;
    int playerHealth = 5, attackPhase, playerDiedCount;
    IEnumerator enemyCoroutine, playerAttackCoroutine;
    [SerializeField] AudioSource musicSource;
    AudioMixer audioMixer;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] GameObject ringObject;
    [SerializeField] AudioClip[] playerPunchClips;
    [SerializeField] AudioClip[] laraPunchClips;
    [SerializeField] AudioController audioController;
    [SerializeField] AudioClip cutsceneExitClip;
    [SerializeField] AuditoryZoomSequence auditoryZoomSequence;

    #region COMBAT AUDIOCLIPS
    [SerializeField] AudioClip enemyBeenKilledClip, combatMusicClip, enterTheRingClip;
    [SerializeField] AudioClip[] playerParryClips, enemyBeenParriedClips, enemyIsHitByAttackClips, enemyAttackClips, playerIsHitByAttackClips, playerAttackClips;
    #endregion

    void Start(){
        enemySource = GetComponent<AudioSource>();
        enemyVRSource = enemySource.GetComponent<DearVRSource>();
        playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
        playerVRSource = playerSource.GetComponent<DearVRSource>();
        audioMixer = enemySource.outputAudioMixerGroup.audioMixer;
        controls = GameObject.Find("Controls").GetComponent<Controls>();
        musicSource.loop = true;
        musicSource.clip = combatMusicClip;
    }

    void Update(){
        if (enteredCombat){
            if (canParry && controls.parry) StartCoroutine(Parry()); 
            if (controls.attack && !attacking) {
                playerAttackCoroutine = PlayerAttack();
                StartCoroutine(playerAttackCoroutine);
            }
        }
    }

    void FixedUpdate(){
        if (moveTowards){
            if (distanceFromPlayer > 20) transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, enemyMoveSpeed * 2f);
            else if (distanceFromPlayer > 10) transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, enemyMoveSpeed * 1.5f);
            else transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, enemyMoveSpeed * 1.5f);
        } 
        if (moveBackAndToSide) transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, -enemyMoveSpeed/2) + new Vector3(enemyMoveSpeed/2, 0, 0); 
        if (Vector3.Distance(transform.position, playerSource.transform.position) < maxDistanceFromPlayer - 0.5f) enemyInRange = true; else enemyInRange = false;  
    }

    void AssignVariables(){
        if (attackPhase == 0) {
            enemyMoveSpeed = 0.05f;
            enemyAttackSpeed = 2f;
        }
    }

    public void AssignCoroutine(string desiredMethod){
        if (attackPhase == 4){
            StartCoroutine(EnemyKilled());
        } else if (desiredMethod == "EnterCombat"){
            enemyCoroutine = EnterCombat();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "MoveTowards"){
            AssignVariables();
            enemyCoroutine = MoveTowards();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "EnemyAttack"){
            enemyCoroutine = EnemyAttack();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "EnemyHitPlayer"){
            enemyCoroutine = EnemyHitPlayer();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "KnockbackPlayer"){
            enemyCoroutine = KnockbackPlayer();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "EnemyMissPlayer"){
            enemyCoroutine = EnemyMissPlayer();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "MoveBackAndToSide"){
            enemyCoroutine = MoveBackAndToSide();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "Parry"){
            enemyCoroutine = Parry();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "EnemyDazed"){
            enemyCoroutine = EnemyDazed();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "KnockbackEnemy"){
            enemyCoroutine = KnockbackEnemy();
            StartCoroutine(enemyCoroutine);
        }
    }

    #region ENEMY COMBAT FLOW 
    IEnumerator EnterCombat(){
        if (enteredCombat){
            controls.inCombat = true;
            audioController.PlayMusic("Combat", 0.3f);
            AssignCoroutine("MoveTowards");
            yield break;
        }
    }

    IEnumerator MoveTowards(){
        moveTowards = true;
        if (enemyInRange){
            moveTowards = false;
            AssignCoroutine("EnemyAttack");
            yield break;
        } else {
            yield return new WaitForSeconds(0.1f);
            AssignCoroutine("MoveTowards");
        }
    }

    IEnumerator EnemyAttack(){ 
        enemyVRSource.DearVRPlayOneShot(enemyAttackClips[Random.Range(0, enemyAttackClips.Length)]);
        canParry = true;
        yield return new WaitForSeconds(enemyAttackSpeed);
        canParry = false;
        if (enemyInRange){
            AssignCoroutine("EnemyHitPlayer");
        } else {
            AssignCoroutine("EnemyMissPlayer");
        }
    }

    // ________________ // 

    IEnumerator EnemyHitPlayer(){
        enemyVRSource.DearVRPlayOneShot(playerIsHitByAttackClips[Random.Range(0, playerIsHitByAttackClips.Length)]);
        enemyVRSource.DearVRPlayOneShot(laraPunchClips[Random.Range(0, laraPunchClips.Length)]);
        playerSource.Stop();
        if (playerAttackCoroutine != null) StopCoroutine(playerAttackCoroutine);
        playerHealth -= 1;
        attacking = false;
        AssignCoroutine("KnockbackPlayer");
        yield break;
    }

    IEnumerator KnockbackPlayer(){
        enemySource.transform.position = Vector3.MoveTowards(enemySource.transform.position, playerSource.transform.position, -8f);
        yield return new WaitForSeconds(1f);
        AssignCoroutine("MoveTowards");
        yield break;
    }

    // ________________ //

    IEnumerator EnemyMissPlayer(){
        AssignCoroutine("MoveBackAndToSide");
        yield break;
    }

    IEnumerator MoveBackAndToSide(){
        moveBackAndToSide = true;
        yield return new WaitForSeconds(4f);
        moveBackAndToSide = false;
        AssignCoroutine("MoveTowards");
    }

    // ________________ // 

    IEnumerator Parry(){
        if (enemyCoroutine != null) StopCoroutine(enemyCoroutine);
        canParry = false;
        enemyVRSource.DearVRStop();;
        playerSource.PlayOneShot(playerParryClips[Random.Range(0, playerParryClips.Length)]);
        enemyVRSource.DearVRPlayOneShot(enemyBeenParriedClips[Random.Range(0, enemyBeenParriedClips.Length)]);
        AssignCoroutine("EnemyDazed");
        yield break;
    }

    IEnumerator EnemyDazed(){
        yield return new WaitForSeconds(3f);
        AssignCoroutine("MoveTowards");
    }

    // ________________ // 

    IEnumerator EnemyKilled(){
        yield return new WaitForSeconds(enemyIsHitByAttackClips[0].length + 0.5f);  
        enemyVRSource.DearVRPlayOneShot(enemyBeenKilledClip);
        yield return new WaitForSeconds(enemyBeenKilledClip.length - 1f);
        StartCoroutine(audioController.FadeMusic());
        darkVsLight.playerDarkness += 1;
        controls.inCombat = false;
        playerActionSource.PlayOneShot(enterTheRingClip, 0.2f);
        yield return new WaitForSeconds(enterTheRingClip.length);
        playerActionSource.PlayOneShot(cutsceneExitClip);
        auditoryZoomSequence.CheckIfShouldStart();
        ringObject.SetActive(false);
        this.gameObject.SetActive(false);
        controls.canPause = true;
    }
    #endregion

    #region PLAYER COMBAT FLOW
    IEnumerator PlayerAttack(){ 
        attacking = true;
        playerSource.PlayOneShot(playerAttackClips[Random.Range(0, playerAttackClips.Length)]);
        yield return new WaitForSeconds(1.2f);
        if (enemyInRange) StartCoroutine(PlayerHitEnemy());
        attacking = false;
    }

    IEnumerator PlayerHitEnemy(){
        attackPhase += 1;
        playerSource.PlayOneShot(playerPunchClips[Random.Range(0, playerPunchClips.Length)]);
        enemyVRSource.DearVRStop();
        if (enemyCoroutine != null) StopCoroutine(enemyCoroutine);
        enemyVRSource.DearVRPlayOneShot(enemyIsHitByAttackClips[Random.Range(0, enemyIsHitByAttackClips.Length)]);
        AssignCoroutine("KnockbackEnemy");
        yield break;
    }

    IEnumerator KnockbackEnemy(){
        enemySource.transform.position = Vector3.MoveTowards(enemySource.transform.position, playerSource.transform.position, -8f);
        AssignCoroutine("MoveBackAndToSide");
        yield break;
    }
    #endregion
}
