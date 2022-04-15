using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LaraCombatSequence : MonoBehaviour
{
    [SerializeField] float maxDistanceFromPlayer = 3, enemyMoveSpeed;
    float angleToEnemy, enemyAttackSpeed, distanceFromPlayer, musicVol;
    Controls controls;
    AudioSource playerSource, enemySource;
    bool moveTowards, moveBackAndToSide, moveToPointOne, moveToPointTwo, reachedPointOne, reachedPointTwo, enemyInRange, canParry, attacking, reduceMusic;
    public bool enteredCombat;
    int playerHealth = 5, attackPhase, playerDiedCount;
    IEnumerator enemyCoroutine, playerAttackCoroutine;
    [SerializeField] AudioSource musicSource;
    AudioMixer audioMixer;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] GameObject ringObject;

    #region COMBAT AUDIOCLIPS
    [SerializeField] AudioClip playerAttackClip, playerParryClip, playerIsHitByAttackClip, enemyBeenKilledClip, combatMusicClip;
    [SerializeField] AudioClip[] enemyBeenParriedClips, enemyIsHitByAttackClips, enemyAttackClips;
    #endregion

    void Start(){
        enemySource = GetComponent<AudioSource>();
        playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
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
        if (reduceMusic && musicVol > -80f){
            musicVol -= 1;
            enemySource.outputAudioMixerGroup.audioMixer.SetFloat("Music_Vol", musicVol);
        } 
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
            musicSource.PlayOneShot(combatMusicClip, 0.2f);
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
        enemySource.PlayOneShot(enemyAttackClips[Random.Range(0, enemyAttackClips.Length)]);
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
        enemySource.PlayOneShot(playerIsHitByAttackClip);
        playerSource.Stop();
        if (playerAttackCoroutine != null) StopCoroutine(playerAttackCoroutine);
        playerHealth -= 1;
        attacking = false;
        AssignCoroutine("KnockbackPlayer");
        yield break;
    }

    IEnumerator KnockbackPlayer(){
        playerSource.transform.position = Vector3.MoveTowards(playerSource.transform.position, enemySource.transform.position, -8f);
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
        enemySource.Stop();
        playerSource.PlayOneShot(playerParryClip);
        enemySource.PlayOneShot(enemyBeenParriedClips[Random.Range(0, enemyBeenParriedClips.Length)]);
        AssignCoroutine("EnemyDazed");
        yield break;
    }

    IEnumerator EnemyDazed(){
        yield return new WaitForSeconds(3f);
        AssignCoroutine("MoveTowards");
    }

    // ________________ // 

    IEnumerator EnemyKilled(){
        reduceMusic = true;
        enemySource.PlayOneShot(enemyBeenKilledClip);
        yield return new WaitForSeconds(enemyBeenKilledClip.length);
        reduceMusic = false;
        musicSource.Stop();
        musicVol = 0;
        enemySource.outputAudioMixerGroup.audioMixer.SetFloat("Music_Vol", musicVol);
        darkVsLight.playerDarkness += 1;
        controls.inCombat = false;
        controls.canZoom = true;
        ringObject.SetActive(false);
    }
    #endregion

    #region PLAYER COMBAT FLOW
    IEnumerator PlayerAttack(){ 
        attacking = true;
        playerSource.PlayOneShot(playerAttackClip);
        yield return new WaitForSeconds(2f);
        if (enemyInRange) StartCoroutine(PlayerHitEnemy());
        attacking = false;
    }

    IEnumerator PlayerHitEnemy(){
        attackPhase += 1;
        enemySource.Stop();
        if (enemyCoroutine != null) StopCoroutine(enemyCoroutine);
        enemySource.PlayOneShot(enemyIsHitByAttackClips[Random.Range(0, enemyIsHitByAttackClips.Length)]);
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
