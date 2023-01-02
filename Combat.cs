using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Combat : MonoBehaviour 
{
//     IEnumerator playerCurrentAttack, enemyCurrentAttack;
//     [SerializeField] DarkVsLight darkVsLight;

//     #region COMBAT BOOLS
//     bool playerWillMiss, playerIsHit;
//     bool enemyAttacking, enemyBeenParried, enemyMoveAway, enemyMoveAwayTimePicked;
//     bool enemyCanParry, playerCanParry, playerCanHit, closeEnoughToFastAttack, closeEnoughToSlowAttack;
//     bool playerSlowAttack;
//     public bool playerAttacking, playerStunned;
//     #endregion

//     public float fastAttackWeighting, playersMaxAngleToParry, playersMaxAngleToMiss, enemyMaxAttackTimer, maxEnemyMoveAwayTimer, 
//         enemysMaxAngleToParry, maxDistanceSlowAttack, maxDistanceFastAttack, minDistanceFromPlayer, playerKnockbackEnemyDistance,
//         maxFightDistance = 30;

//     float enemyAttackTimer, enemyMoveAwayTimer, enemyNewMoveAwayTimer, distanceFromPlayer, angleToEnemy;

//     #region COMBAT AUDIOCLIPS
//     public AudioClip playerFastAttackClip, playerSlowAttackClip, playerParryClip,
//         playerStunnedClip, playerTryToAttackWhenStunnedClip, playerMissedParryClip,
//         playerIsHitByFastAttackClip, playerIsHitBySlowAttackClip, playerRanAwayClip, playerBeenKilledClip,

//         enemyBreathingClip, enemyFastAttackClip, enemySlowAttackClip, enemyTauntClip, enemyParryClip,
//         enemyBeenParriedClip, enemyMissedPlayerClip, enemyIsHitByFastAttackClip, enemyIsHitBySlowAttackClip, enemyBeenKilledClip;
//     #endregion

//     public AudioClip[] enemyBreathingHighHealthClips, enemyBreathingMidHealthClips, enemyBreathingLowHealthClips,
//         playerBreathingHighHealthClips, playerBreathingMidHealthClips, playerBreathingLowHealthClips;

//     AudioClip[] enemyBreathingClips, playerBreathingClips;

//     AudioSource playerSource, enemySource;
//     AudioMixer audioMixer;

//     Controls controls;

//     public float enemyTemper, enemyTemperModifier;

//     #region ENEMY MOVEMENT VARIABLES
//     public float moveSpeedMultiplier, rotateSpeedMultiplier, sprintSpeedMultiplier, initialSpeedMultiplier, walkDist, maxDist;
//     float moveToTheSideTimer;
//     public float minMoveToTheSideTimer, maxMoveToTheSideTimer, moveToTheSideMultiplier;
//     int leftOrRight;
//     bool closeEnoughToMoveLeftOrRight, enemyCanMove;
//     public Vector3 recentPosition;
//     #endregion

//     public int playerHealth, enemyHealth, enemyDefensiveHealth;

//     int initialPlayerHealth, initialEnemyHealth;

//     public void Start(){
//         enemySource = GetComponent<AudioSource>();
//         playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
//         controls = GameObject.Find("Controls").GetComponent<Controls>();
//         audioMixer = enemySource.outputAudioMixerGroup.audioMixer;
//         initialPlayerHealth = playerHealth;
//     }

//     void OnEnable(){
//         enemySource = GetComponent<AudioSource>();
//         playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
//         controls = GameObject.Find("Controls").GetComponent<Controls>();
//         audioMixer = enemySource.outputAudioMixerGroup.audioMixer;
//         initialPlayerHealth = playerHealth;
//     }

//     void Update(){
//         if (controls.inCombat){
//             #region GET PLAYER INPUT
//             if (controls.attack){
//                 if (!playerStunned && !playerAttacking){
//                     playerAttacking = true;
//                     playerCurrentAttack = PlayerFastAttack();
//                     StartCoroutine(playerCurrentAttack);   
//                 } else if (playerStunned && !playerAttacking){
//                     playerSource.PlayOneShot(playerTryToAttackWhenStunnedClip);
//                 } else {
//                     Debug.Log("Can't currently attack as currently attacking");
//                 }
//             }

//             if (controls.slowAttack){
//                 controls.slowAttack = false;
//                 if (!playerStunned && !playerAttacking){
//                     playerAttacking = true;
//                     playerCurrentAttack = PlayerSlowAttack();
//                     StartCoroutine(playerCurrentAttack);
//                 } else if (playerStunned && !playerAttacking){
//                     playerSource.PlayOneShot(playerTryToAttackWhenStunnedClip);
//                 } else {
//                     Debug.Log("Can't currently attack as currently attacking");
//                 }
//             }

//             if (controls.parry){
//                 if ((angleToEnemy > playersMaxAngleToParry) && (distanceFromPlayer < maxDistanceFastAttack) && 
//                     !playerStunned && !playerAttacking && enemyAttacking){
//                     playerCanParry = true;
//                 } else {
//                     playerCanParry = false;
//                 }

//                 if (playerCanParry){
//                     StartCoroutine(EnemyBeenParried());
//                 } else if (!playerCanParry){
//                     StartCoroutine(PlayerMissedParry());
//                 }
//                 controls.parry = false;
//             }
//             #endregion

//             if (playerAttacking && playerIsHit){
//                 StopCoroutine(playerCurrentAttack);
//                 playerIsHit = false;
//                 playerAttacking = false;
//             }
            
//             EnemyMovement();
//             StartEnemyAttack();
//             CheckIfCenteredAndDistance();
//             CheckIfDead();
//         }
//     }

//     public IEnumerator EnemyMoveTowardsOrTaunt(){
//         float aggressionTest = Random.Range(0, 100);

//         if ((aggressionTest < (enemyTemper + enemyTemperModifier)) && (distanceFromPlayer > (maxDistanceSlowAttack * 3))){
//             Debug.Log("start taunt");
//             enemyCanMove = false;
//             enemyTemperModifier += 10;
//             enemySource.PlayOneShot(enemyTauntClip);
//             yield return new WaitForSeconds(enemyTauntClip.length);
//             Debug.Log("finish taunt");
//             StartCoroutine(MoveTowardsPlayer());
//             yield break;
//         } else {
//             StartCoroutine(MoveTowardsPlayer());
//             yield break;
//         }
//     }

//     IEnumerator MoveTowardsPlayer(){
//         enemyCanMove = true;
//         EnemyAttackDecider();
//         yield break;
//     }

//     void StartEnemyAttack(){
//         enemyAttackTimer -= Time.deltaTime;
//         if (!enemyAttacking && (distanceFromPlayer < minDistanceFromPlayer) && enemyAttackTimer < 0){
//             EnemyAttackDecider();
//             enemyAttackTimer = enemyMaxAttackTimer;
//         }
//     }

//     void EnemyAttackDecider(){
//         // MOVED SLOW ATTACK FROM BEING A FUNCTION OF DISTANCE
//         if (!enemyBeenParried && !enemyAttacking){
//             if (Random.Range(0, 100) < fastAttackWeighting){
//                 if (playerSlowAttack && (distanceFromPlayer < maxDistanceFastAttack)){
//                     enemyCurrentAttack = EnemyFastAttack();
//                     StartCoroutine(enemyCurrentAttack);
//                     enemySource.PlayOneShot(enemyFastAttackClip);
//                 }
//                 if (distanceFromPlayer < maxDistanceFastAttack){
//                     enemyCurrentAttack = EnemyFastAttack();
//                     StartCoroutine(enemyCurrentAttack);
//                     enemySource.PlayOneShot(enemyFastAttackClip);
//                 }
//             } else {
//                 if (distanceFromPlayer < maxDistanceSlowAttack){
//                     enemyCurrentAttack = EnemySlowAttack();
//                     StartCoroutine(enemyCurrentAttack);
//                     enemySource.PlayOneShot(enemySlowAttackClip);
//                 }

//                 if (distanceFromPlayer > (maxDistanceSlowAttack * 2)){
//                     enemyCurrentAttack = EnemyTaunt();
//                     Debug.Log("Started Enemy Taunt");
//                     StartCoroutine(enemyCurrentAttack);
//                 }
//             }
//         }
//     }
    
//     void EnemyMovement(){
//         float enemyMoveSpeed = Time.deltaTime * moveSpeedMultiplier;
//         float enemyRotationSpeed = Time.deltaTime * rotateSpeedMultiplier;

//         // ENEMY MOVES TO THE SIDE WHEN FAST ATTACKING, AND WHEN THE PLAYER ATTACKS

//         #region SPRINT CHECK
//         if (distanceFromPlayer > (2f * maxDistanceSlowAttack)){
//             moveSpeedMultiplier = sprintSpeedMultiplier;
//         } else {
//             moveSpeedMultiplier = initialSpeedMultiplier;
//         }
//         #endregion

//         if (enemyCanMove) {
//             #region MOVE FRONT AND BACK
//             if (enemyMoveAway){
//                 if (!enemyMoveAwayTimePicked){
//                     enemyNewMoveAwayTimer = Random.Range(maxEnemyMoveAwayTimer + (maxEnemyMoveAwayTimer / 5), 
//                                                          maxEnemyMoveAwayTimer - (maxEnemyMoveAwayTimer / 5));
//                     enemyMoveAwayTimePicked = true;
//                 }
//                 transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, -(enemyMoveSpeed / 2));
//                 walkDist += Vector3.Distance(recentPosition, transform.position) * Time.deltaTime;
//                 enemyMoveAwayTimer += Time.deltaTime;
//                 if (enemyMoveAwayTimer > enemyNewMoveAwayTimer){
//                     enemyMoveAwayTimePicked = false;
//                     enemyMoveAway = false;
//                     enemyMoveAwayTimer = 0;
//                 }
//             } else if (playerSlowAttack || (enemyHealth < enemyDefensiveHealth)){
//                 // ENEMY TRIES TO ESCAPE
//                 transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, -(enemyMoveSpeed / 2));
//                 walkDist += Vector3.Distance(recentPosition, transform.position) * Time.deltaTime;
//             } else if ((distanceFromPlayer > minDistanceFromPlayer) && !playerSlowAttack){
//                 transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, enemyMoveSpeed);
//                 walkDist += Vector3.Distance(recentPosition, transform.position) * Time.deltaTime;
//             }
//             #endregion

//             #region MOVE LEFT OR RIGHT CHECK
//             if (distanceFromPlayer < (maxDistanceFastAttack * 2) && (angleToEnemy > 90)){
//                 closeEnoughToMoveLeftOrRight = true;
//             } else {
//                 closeEnoughToMoveLeftOrRight = false;
//                 moveToTheSideTimer = 0;
//             }
//             #endregion

//             #region MOVE LEFT OR RIGHT
//             if (closeEnoughToMoveLeftOrRight){
//                 moveToTheSideTimer += Time.deltaTime;
//                 if ((moveToTheSideTimer > minMoveToTheSideTimer) && (moveToTheSideTimer < maxMoveToTheSideTimer)){
//                     if (leftOrRight > 49){
//                         transform.position += transform.right * moveToTheSideMultiplier;
//                         walkDist += Vector3.Distance(recentPosition, transform.position) * Time.deltaTime;
//                     } else if (leftOrRight <= 49){
//                         transform.position -= transform.right * moveToTheSideMultiplier;
//                         walkDist += Vector3.Distance(recentPosition, transform.position) * Time.deltaTime;
//                     }
//                 }

//                 if (moveToTheSideTimer > maxMoveToTheSideTimer){
//                     leftOrRight = Random.Range(0, 100);
//                     moveToTheSideTimer = 0;
//                 }
//             }
//             #endregion

//             #region TURN TO FACE ENEMY
//             if (angleToEnemy < 180){
//                 Vector3 targetDirection = playerSource.transform.position - transform.position;
//                 Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, enemyRotationSpeed, 0.0f);
//                 transform.rotation = Quaternion.LookRotation(newDirection);
//             }
//             #endregion
//         }
//     }

//     #region ATTACKS, TAUNT AND PARRIES
//     IEnumerator PlayerFastAttack(){
//         playerSource.PlayOneShot(playerFastAttackClip);
//         // enemyMovingToTheSide = true;
//         yield return new WaitForSeconds(playerFastAttackClip.length);
//         playerAttacking = false;
//         // enemyMovingToTheSide = false;
//         if (playerCanHit && closeEnoughToFastAttack){
//             enemyHealth -= 1;
//             enemyAttacking = false;
//             enemySource.Stop();
//             enemySource.PlayOneShot(enemyIsHitByFastAttackClip);
//             Vector3 direction = (enemySource.transform.position - playerSource.transform.position).normalized;
//             enemySource.transform.position += direction * playerKnockbackEnemyDistance;
//             enemyMoveAway = true;
//             yield return new WaitForSeconds(enemyIsHitByFastAttackClip.length);
//             StartCoroutine(RepeatEnemyBreathing());
//             StartCoroutine(RepeatPlayerBreathing());
//             yield break;
//         } else if (enemyCanParry){
//             playerStunned = true;
//             enemySource.PlayOneShot(enemyParryClip);
//             playerSource.PlayOneShot(playerStunnedClip);
//             // MAKE THINGS WAVEY
//             yield return new WaitForSeconds(playerStunnedClip.length);
//             playerStunned = false;
//             yield break;
//         }
//     }

//     IEnumerator PlayerSlowAttack(){
//         playerSlowAttack = true;
//         playerSource.PlayOneShot(playerSlowAttackClip);
//         yield return new WaitForSeconds(playerSlowAttackClip.length);
//         playerAttacking = false;
//         if (playerCanHit && closeEnoughToSlowAttack){
//             enemyHealth -= 2;
//             enemyAttacking = false;
//             enemySource.Stop();
//             enemySource.PlayOneShot(enemyIsHitBySlowAttackClip);
//             Vector3 direction = (enemySource.transform.position - playerSource.transform.position).normalized;
//             enemySource.transform.position += direction * playerKnockbackEnemyDistance * 2;
//             enemyMoveAway = true;
//             yield return new WaitForSeconds(enemyIsHitBySlowAttackClip.length);
//             StartCoroutine(RepeatEnemyBreathing());
//             StartCoroutine(RepeatPlayerBreathing());
//         }
//         playerSlowAttack = false;
//         yield break;
//     }

//     IEnumerator PlayerMissedParry(){
//         playerSource.PlayOneShot(playerMissedParryClip);
//         playerStunned = true;
//         EnemyAttackDecider();
//         yield return new WaitForSeconds(playerMissedParryClip.length);
//         playerStunned = false;
//         yield break;
//     }

//     IEnumerator EnemyFastAttack(){
//         enemySource.Stop();
//         enemySource.PlayOneShot(enemyFastAttackClip);
//         // enemyMovingToTheSide = true;
//         enemyAttacking = true;
//         yield return new WaitForSeconds(enemyFastAttackClip.length);
//         // enemyMovingToTheSide = false;
//         enemyAttacking = false;
//         if (!enemyBeenParried && closeEnoughToFastAttack){
//             playerHealth -= 1;
//             playerAttacking = false;
//             playerSource.Stop();
//             playerSource.PlayOneShot(playerIsHitByFastAttackClip);
//             Vector3 direction = (enemySource.transform.position - playerSource.transform.position).normalized;
//             playerSource.transform.position -= direction * playerKnockbackEnemyDistance;
//             PlayerBeenHit();
//             enemyMoveAway = true;
//             StartCoroutine(RepeatEnemyBreathing());
//         } else {
//             enemySource.PlayOneShot(enemyMissedPlayerClip);
//             yield return new WaitForSeconds(enemyMissedPlayerClip.length);
//             StartCoroutine(RepeatEnemyBreathing());
//             enemyMoveAway = true;
//         }
//         yield break;
//     }

//     IEnumerator EnemySlowAttack(){
//         enemySource.Stop();
//         enemySource.PlayOneShot(enemySlowAttackClip);
//         enemyAttacking = true;
//         yield return new WaitForSeconds(enemySlowAttackClip.length);
//         enemyAttacking = false;

//         if (!enemyBeenParried && closeEnoughToSlowAttack){
//             playerHealth -= 2;
//             playerAttacking = false;
//             playerSource.Stop();
//             playerSource.PlayOneShot(playerIsHitByFastAttackClip);
//             Vector3 direction = (enemySource.transform.position - playerSource.transform.position).normalized;
//             playerSource.transform.position -= direction * playerKnockbackEnemyDistance * 1.5f;
//             PlayerBeenHit();
//             StartCoroutine(EnemyMoveTowardsOrTaunt());
//             enemyMoveAway = true;
//         } else {
//             enemySource.PlayOneShot(enemyMissedPlayerClip);
//             yield return new WaitForSeconds(enemyMissedPlayerClip.length);
//             StartCoroutine(RepeatEnemyBreathing());
//             StartCoroutine(EnemyMoveTowardsOrTaunt());
//             enemyMoveAway = true;
//             EnemyAttackDecider();
//         }
//         yield break;
//     }

//     IEnumerator EnemyTaunt(){
//         enemySource.Stop();
//         enemyCanMove = false;
//         enemyAttacking = true;
//         enemySource.PlayOneShot(enemyTauntClip);
//         yield return new WaitForSeconds(enemyTauntClip.length);
//         StartCoroutine(RepeatEnemyBreathing());
//         enemyAttacking = false;
//         enemyCanMove = true;
//         yield break;
//     }

//     IEnumerator EnemyBeenParried(){
//         playerSource.PlayOneShot(playerParryClip);
//         enemyCanMove = false;
//         enemySource.Stop();
//         StopCoroutine(enemyCurrentAttack);
//         enemySource.PlayOneShot(enemyBeenParriedClip);
//         yield return new WaitForSeconds(enemyBeenParriedClip.length);
//         StartCoroutine(RepeatEnemyBreathing());
//         enemyCanMove = true;
//         EnemyAttackDecider();
//         yield break;
//     }
    
//     void PlayerBeenHit(){ 
//         /* 
//         REFACTOR - Add this when hit
//         float waveyFloat;
//         audioMixer.GetFloat("waveyParamString", out waveyFloat);
//         while (waveyFloat < 1)
//         {
//             waveyFloat += Time.deltaTime * 6;
//             audioMixer.SetFloat("waveyParamString", waveyFloat);
//         }
//         while (waveyFloat > 0)
//         {
//             waveyFloat -= Time.deltaTime * 3;
//             audioMixer.SetFloat("waveyParamString", waveyFloat);
//         }
//         */
//     }
//     #endregion
    
//     void CheckIfCenteredAndDistance(){
//         if (controls.inCombat){
//             distanceFromPlayer = Vector3.Distance(playerSource.transform.position, enemySource.transform.position);
//             angleToEnemy = Quaternion.Angle(playerSource.transform.rotation, enemySource.transform.rotation);
//             bool playerWillMiss;

//             if (angleToEnemy < playersMaxAngleToMiss){
//                 playerWillMiss = true;
//                 enemyCanParry = false;
//             } else if (angleToEnemy < enemysMaxAngleToParry && !enemyAttacking){
//                 playerWillMiss = false;
//                 enemyCanParry = true;
//             } else {
//                 playerWillMiss = false;
//                 enemyCanParry = false;
//             }

//             if (!playerWillMiss && !enemyCanParry){
//                 playerCanHit = true;
//             } else {
//                 playerCanHit = false;
//             }

//             // DISTANCE
//             closeEnoughToSlowAttack = distanceFromPlayer < maxDistanceSlowAttack ? true : false;
//             closeEnoughToFastAttack = distanceFromPlayer < maxDistanceFastAttack ? true : false;
//         }
//     }

//     void CheckIfRanAway(){
//         if(Vector3.Distance(enemySource.transform.position, playerSource.transform.position) > maxFightDistance && controls.inCombat){
//             controls.inCombat = false;
//             enemySource.PlayOneShot(playerRanAwayClip);
//             darkVsLight.playerDarkness -= 1;
//         }
//     }

//     void CheckIfDead(){
//         if (enemyHealth < 1 && controls.inCombat){
//             controls.inCombat = false;
//             Debug.Log("enemy been killed");
//             enemySource.Stop();
//             enemySource.PlayOneShot(enemyBeenKilledClip);
//             darkVsLight.playerDarkness += 1;
//         }
//         if (playerHealth < 1 && controls.inCombat){
//             controls.inCombat = false;
//             Debug.Log("player been killed");
//             enemySource.Stop();
//             enemySource.PlayOneShot(playerBeenKilledClip);
//             darkVsLight.playerDarkness += 1;
//         }
//     }

//     IEnumerator RepeatEnemyBreathing(){
//         if (enemyHealth > (initialEnemyHealth * 0.66f)){
//             enemyBreathingClips = enemyBreathingHighHealthClips; // REFACTOR - THIS WILL BE CHANGED WHEN HAVE DIFFERENT LEVELS OF BREATHING
//         } else if (enemyHealth > (initialEnemyHealth * 0.33f)){
//             enemyBreathingClips = enemyBreathingMidHealthClips;
//         } else if (enemyHealth > 0){
//             enemyBreathingClips = enemyBreathingLowHealthClips;
//         }
//         enemySource.PlayOneShot(enemyBreathingClips[Random.Range(0, enemyBreathingClips.Length)]);
//         yield return new WaitForSeconds(enemyBreathingClip.length);
//         if (!enemySource.isPlaying && enemyHealth > 0){
//             StartCoroutine(RepeatEnemyBreathing());
//         }
//     }

//     IEnumerator RepeatPlayerBreathing(){
//         if (playerHealth > (initialPlayerHealth * 0.66f)){
//             playerBreathingClips = playerBreathingHighHealthClips;
//         } else if (playerHealth > (initialPlayerHealth * 0.33f)){
//             playerBreathingClips = playerBreathingMidHealthClips;
//         } else if (playerHealth > 0){
//             playerBreathingClips = playerBreathingLowHealthClips;
//         }
//         Debug.Log("repeating player breathing");
//         playerSource.PlayOneShot(playerBreathingClips[Random.Range(0, playerBreathingClips.Length)]);
//         yield return new WaitForSeconds(playerBreathingClips[0].length);
//         if (!playerSource.isPlaying){
//             StartCoroutine(RepeatPlayerBreathing());
//         }
//     }
// }
}

