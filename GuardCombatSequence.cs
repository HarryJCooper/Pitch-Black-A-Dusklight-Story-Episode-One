using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardCombatSequence : MonoBehaviour
{
    [SerializeField] float maxDistanceFromPlayer = 3, enemyMoveSpeed;
    float angleToEnemy, enemyAttackSpeed;
    [SerializeField] Transform wallTransform;
    Controls controls;
    AudioSource playerSource;
    public AudioSource enemySource;
    bool moveTowards, moveBackAndToSide, moveToPointOne, moveToPointTwo, reachedPointOne, reachedPointTwo, enemyInRange, canParry, attacking, startedFight, fightOver;
    int playerHealth = 3, attackPhase, playerDiedCount, playerAttackInt, playerBeenHitInt, randomInt;
    public IEnumerator enemyCoroutine, playerAttackCoroutine;
    [SerializeField] PlayerWonFightSequence playerWonFightSequence;
    [SerializeField] PlayerLostFightSequence playerLostFightSequence;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] AudioController audioController;
    [SerializeField] AudioSource doorSource;
    [SerializeField] AudioClip accessDeniedClip, enterAccessCodeClip;

    #region PATROL VARIABLES
    [SerializeField] Transform guardPositionOne, guardPositionTwo;
    [SerializeField] AudioClip beepingClip;
    #endregion

    #region STEALTH VARIABLES
    public float currentDetectionDistance, detectionDistance, distanceFromPlayer, detectionTimer, maxDetectionTime;
    bool startedDetectionBeep, hasEnteredStealth;
    [SerializeField] AudioClip enemyFoundPlayerSneakingClip, playerFoundSneakingClip, playerFoundWalkingClip;
    #endregion

    #region COMBAT AUDIOCLIPS
    [SerializeField] AudioClip playerParryClip, playerMissedParryClip, playerRanAwayClip, playerBeenKilledClip,
        enemyAttackClip, enemyBeenKilledClip, enemyLostPlayerClip, drawBatonClip;
    [SerializeField] AudioClip[] playerAttackClips, playerIsHitByAttackClips, playerPunchClips,
    enemyBeenParriedClips, enemyFoundPlayerClips, enemyTauntClips, enemyAttackClips, enemyHitByAttackClips, enemyPatrolClips, guardClips, playerClips;
    #endregion

    int RandomNumberGen(string clipType){
        if (clipType == "attack"){
            randomInt = Random.Range(0, playerAttackClips.Length);
            if (randomInt == playerAttackInt) randomInt = RandomNumberGen("attack");
        } else {
            randomInt = Random.Range(0, playerIsHitByAttackClips.Length);
            if (randomInt == playerBeenHitInt) randomInt = RandomNumberGen("");
        }
        return randomInt;
    }

    void Start(){
        enemySource = GetComponent<AudioSource>();
        playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
        controls = GameObject.Find("Controls").GetComponent<Controls>();
        AssignCoroutine("MoveToPointOne");
    }

     void CheckIfDetected(){
        if (distanceFromPlayer < currentDetectionDistance) {
            StopCoroutine(enemyCoroutine);
            AssignCoroutine("EnterCombat");
        }
    }

    void DetectionDistanceModifier(){
        distanceFromPlayer = Vector3.Distance(playerSource.transform.position, transform.position);
        if (controls.sprint){ currentDetectionDistance = detectionDistance * 2; return;}
        if (controls.crouching){ currentDetectionDistance = detectionDistance / 2; return;} 
        currentDetectionDistance = detectionDistance;
    }

    void Update(){
        if (Vector3.Distance(transform.position, playerSource.transform.position) < 100 && !controls.inCombat) controls.inStealth = true; else controls.inStealth = false;  
        if (controls.crouching && !startedDetectionBeep){
            startedDetectionBeep = true; 
            StartCoroutine(DistanceFromEnemySignifier());
        }
        if (controls.inStealth){
            DetectionDistanceModifier();
            CheckIfDetected();
            return;
        }
        if (controls.inCombat){
            if (canParry && controls.parry) StartCoroutine(Parry()); 
            if (controls.attack && !attacking){
                playerAttackCoroutine = PlayerAttack();
                StartCoroutine(playerAttackCoroutine);
            }
        }
    }

    void UpdateMoveTowards(){
        if (distanceFromPlayer > 20){
            transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, enemyMoveSpeed * 2f);
            return;
        } 
        if (distanceFromPlayer > 10){
            transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, enemyMoveSpeed * 1.5f);
            return;
        } 
        transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, enemyMoveSpeed * 1.5f);
    }

    void FixedUpdate(){
        if (moveTowards) UpdateMoveTowards();
        if (moveBackAndToSide) transform.position = Vector3.MoveTowards(transform.position, playerSource.transform.position, -enemyMoveSpeed/2) + new Vector3(enemyMoveSpeed/2, 0, 0); 
        if (moveToPointOne) transform.position = Vector3.MoveTowards(transform.position, guardPositionOne.position, 0.1f); 
        if (Vector3.Distance(transform.position, guardPositionOne.position) < 1f) reachedPointOne = true; else reachedPointOne = false;
        if (moveToPointTwo) transform.position = Vector3.MoveTowards(transform.position, guardPositionTwo.position, 0.1f); 
        if (Vector3.Distance(transform.position, guardPositionTwo.position) < 1f) reachedPointTwo = true; else reachedPointTwo = false;
        if (Vector3.Distance(transform.position, playerSource.transform.position) < maxDistanceFromPlayer - 0.5f) enemyInRange = true; else enemyInRange = false;  
    }

    void AssignVariables(){
        if (attackPhase == 0) {
            enemyAttackClip = enemyAttackClips[Random.Range(0, enemyAttackClips.Length)];
            enemyMoveSpeed = 0.05f;
            enemyAttackSpeed = 2f;
            return;
        } 
        if (attackPhase == 2){
            enemyAttackClip = enemyAttackClips[Random.Range(0, enemyAttackClips.Length)];
            enemyMoveSpeed = 0.07f;
            enemyAttackSpeed = 1.7f;
            return;
        } 
        if (attackPhase == 4){
            enemyAttackClip = enemyAttackClips[Random.Range(0, enemyAttackClips.Length)];
            enemyMoveSpeed = 0.09f;
            enemyAttackSpeed = 1.4f;
            return;
        }
    }

    void AssignCoroutine(string desiredMethod){
        if (Vector3.Distance(enemySource.transform.position, playerSource.transform.position) > 80 && controls.inCombat && startedFight){
            startedFight = false;
            StartCoroutine(PlayerRanAway());
        } else if (attackPhase == 6 && !fightOver){
            fightOver = true;
            StartCoroutine(EnemyKilled());
        } else if (playerHealth == 0 && !fightOver){
            StartCoroutine(PlayerKilled());
        } else if (desiredMethod == "MoveToPointOne"){
            enemyCoroutine = MoveToPointOne();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "MoveToPointTwo"){
            enemyCoroutine = MoveToPointTwo();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "MoveToPointThree"){
            enemyCoroutine = MoveToPointThree();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "MoveToPointFour"){
            enemyCoroutine = MoveToPointFour();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "EnterCombat" && !startedFight){
            startedFight = true;
            enemyCoroutine = EnterCombat();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "MoveTowards"){
            startedFight = true;
            AssignVariables();
            enemyCoroutine = MoveTowards();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "EnemyPatrolOne"){
            enemyCoroutine = EnemyPatrolOne();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "EnemyPatrolTwo"){
            enemyCoroutine = EnemyPatrolTwo();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "EnemyPatrolThree"){
            enemyCoroutine = EnemyPatrolThree();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "EnemyPatrolFour"){
            enemyCoroutine = EnemyPatrolFour();
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

    #region ENEMY PATROL FLOW
    IEnumerator EnemyPatrolOne(){
        // As the player draws closer to the Pumping Station, a guard’s footsteps and murmurs can be heard. He is pacing around a door, a light’s flicker, and the door malfunctioning, in combination with his remarks, denote the door location. 
        // Guard
        // Damn door, is everything broken in this station? Oh yeah, IT IS! So why the hell are they making me guard some ‘out of action’ pumping station when aaaall the others get to go on border detail… 
        doorSource.PlayOneShot(enterAccessCodeClip);
        yield return new WaitForSeconds(enterAccessCodeClip.length);
        enemySource.PlayOneShot(enemyPatrolClips[0]);
        yield return new WaitForSeconds(enemyPatrolClips[0].length);
        AssignCoroutine("MoveToPointOne");
        yield break;
    }

    IEnumerator MoveToPointOne(){
        moveToPointOne = true;
        if (reachedPointOne){
            moveToPointOne = false;
            AssignCoroutine("EnemyPatrolTwo");
            yield break;
        }
        yield return new WaitForSeconds(0.1f);
        AssignCoroutine("MoveToPointOne");
    }

    IEnumerator EnemyPatrolTwo(){
        // Next line is said with a fit of horrific anger, contrasting with the humorous tone of the prior line.
        // Guard
        // DAMN DOOR! 
        doorSource.PlayOneShot(accessDeniedClip);
        yield return new WaitForSeconds(accessDeniedClip.length);
        enemySource.PlayOneShot(enemyPatrolClips[1]);
        yield return new WaitForSeconds(enemyPatrolClips[1].length);
        AssignCoroutine("MoveToPointTwo");
        yield break;
    }

    IEnumerator MoveToPointTwo(){
        moveToPointTwo = true;
        if (reachedPointTwo){
            moveToPointTwo = false;
            AssignCoroutine("EnemyPatrolThree");
            yield break;
        }
        yield return new WaitForSeconds(0.1f);
        AssignCoroutine("MoveToPointTwo");
    }

    IEnumerator EnemyPatrolThree(){
        // As the player draws closer, Guard begins uttering again. (Maybe… as the player reaches a trigger the guard remarks on hearing something, no matter how quiet the player is being) 
        // Guard 
        // What was that?! If anyone’s there… and you haven’t figured out I’m the guard already, well… be warned, I’m not one for playing games… 
        enemySource.PlayOneShot(enemyPatrolClips[2]);
        yield return new WaitForSeconds(enemyPatrolClips[2].length);
        AssignCoroutine("MoveToPointThree");
        yield break;
    }

    IEnumerator MoveToPointThree(){
        moveToPointOne = true;
        if (reachedPointOne){
            moveToPointOne = false;
            AssignCoroutine("EnemyPatrolFour");
            yield break;
        } 
        yield return new WaitForSeconds(0.1f);
        AssignCoroutine("MoveToPointThree");
    }

    IEnumerator EnemyPatrolFour(){
        // The player has two approaches of dealing with the guard. After a while, the guard will stop his routine and turn to fixing the door, but the player has to wait a while for this. He leans into a comically scornful tone in the last utterance. 
        // Guard
        // Ahh it was probably nothing. Screw it, I’m gonna try and fix this door. Once and for all… 
        enemySource.PlayOneShot(enemyPatrolClips[3]);
        yield return new WaitForSeconds(enemyPatrolClips[3].length);
        AssignCoroutine("EnemyPatrolOne");
        yield break;
    }

    IEnumerator MoveToPointFour(){
        moveToPointTwo = true;
        if (reachedPointTwo){
            moveToPointTwo = false;
            AssignCoroutine("EnemyPatrolOne");
            yield break;
        }
        yield return new WaitForSeconds(0.1f);
        AssignCoroutine("MoveToPointFour");
    }
    #endregion

    #region ENEMY COMBAT FLOW 
    IEnumerator EnterCombat(){
        moveToPointOne = false;
        moveToPointTwo = false;
        controls.inStealth = false;
        if (controls.crouching){
            controls.crouching = false;
            controls.inCombat = true;  
            enemySource.Stop();
            enemySource.PlayOneShot(enemyFoundPlayerSneakingClip);
            yield return new WaitForSeconds(enemyFoundPlayerSneakingClip.length);
            playerSource.PlayOneShot(playerFoundSneakingClip);
        } else {
            controls.inCombat = true;  
            enemySource.Stop();
            enemySource.PlayOneShot(enemyFoundPlayerClips[0]);
            yield return new WaitForSeconds(enemyFoundPlayerClips[0].length);
            enemySource.PlayOneShot(drawBatonClip);
            yield return new WaitForSeconds(drawBatonClip.length);
            enemySource.PlayOneShot(enemyFoundPlayerClips[1]);
            yield return new WaitForSeconds(enemyFoundPlayerClips[1].length);
            playerSource.PlayOneShot(playerFoundWalkingClip);
        }
        audioController.PlayMusic("combat", 0.15f);
        AssignCoroutine("MoveTowards");
        yield break;
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

    IEnumerator EnemyAttack(){ // ROUTINE CAN BE CANCELLED AT ANY TIME BY A PARRY
        enemySource.PlayOneShot(enemyAttackClip);
        canParry = true;
        yield return new WaitForSeconds(enemyAttackSpeed);
        canParry = false;
        if (enemyInRange){
            AssignCoroutine("EnemyHitPlayer");
            yield break;
        }
        AssignCoroutine("EnemyMissPlayer");
    }

    // ________________ // 

    IEnumerator EnemyHitPlayer(){
        playerSource.Stop();
        playerBeenHitInt = RandomNumberGen("");
        playerSource.PlayOneShot(playerIsHitByAttackClips[playerBeenHitInt]);
        if (playerAttackCoroutine != null) StopCoroutine(playerAttackCoroutine);
        playerHealth -= 1;
        attacking = false;
        AssignCoroutine("KnockbackPlayer");
        yield break;
    }

    IEnumerator KnockbackPlayer(){
        Vector3 newPosition = Vector3.MoveTowards(playerSource.transform.position, enemySource.transform.position, -8f);
        if (newPosition.z < wallTransform.position.z){
            enemySource.transform.position = newPosition;
        } else {
            playerSource.transform.position = new Vector3(newPosition.x, newPosition.y, wallTransform.position.z);
        }
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
        if (attackPhase == 2){
            enemySource.PlayOneShot(enemyTauntClips[0]);
            yield return new WaitForSeconds(enemyTauntClips[0].length);
        } 
        if (attackPhase == 4){
            enemySource.PlayOneShot(enemyTauntClips[1]);
            yield return new WaitForSeconds(enemyTauntClips[1].length);
        }
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
        StartCoroutine(audioController.FadeMusic());
        enemySource.PlayOneShot(enemyBeenKilledClip);
        yield return new WaitForSeconds(enemyBeenKilledClip.length);
        darkVsLight.playerDarkness += 1;
        playerWonFightSequence.enabled = true;
        controls.inCombat = false;
        StartCoroutine(playerWonFightSequence.Sequence());
        controls.canZoom = true;
    }

    IEnumerator PlayerKilled(){
        if (playerDiedCount == 0){
            StartCoroutine(audioController.FadeMusic());
            // Guard
            // I told you, we don’t want to make things even more interesting for me… stay. Down. Before I do more than knock you out… 
            enemySource.PlayOneShot(guardClips[0]);
            yield return new WaitForSeconds(guardClips[0].length);
            // Protag 
            // Damn, only thing’s knocked is my ego…  
            playerSource.PlayOneShot(playerClips[0]);
            yield return new WaitForSeconds(playerClips[0].length);
            // Guard
            // Aghhh! 
            enemySource.PlayOneShot(guardClips[1]);
            yield return new WaitForSeconds(guardClips[1].length);
            Debug.Log("player died < 2");
            playerHealth = 3 + darkVsLight.playerDarkness;
            attackPhase = 0;
            AssignCoroutine("MoveTowards");
            audioController.PlayMusic("combat", 0.15f);
            playerDiedCount += 1;
            yield break;
        } 
        if (playerDiedCount == 1){
            StartCoroutine(audioController.FadeMusic());
            // Guard
            // Seriously, stay down, whatever it is you’re looking for, ain’t worth this. 
            enemySource.PlayOneShot(guardClips[2]);
            yield return new WaitForSeconds(guardClips[2].length);
            // Protag 
            // Can’t go back… too many questions… not enough answers… 
            playerSource.PlayOneShot(playerClips[1]);
            yield return new WaitForSeconds(playerClips[1].length);
            Debug.Log("player died < 2");
            playerHealth = 3 + darkVsLight.playerDarkness;
            attackPhase = 0;
            AssignCoroutine("MoveTowards");
            audioController.PlayMusic("combat", 0.15f);
            playerDiedCount += 1;
            yield break;
        } 
        if (playerDiedCount == 2){
            fightOver = true;
            Debug.Log("player died count == 2");
            controls.inCombat = false;
            StartCoroutine(audioController.FadeMusic());
            // Protag
            // Aghhh… 
            playerSource.PlayOneShot(playerClips[2]);
            yield return new WaitForSeconds(playerClips[2].length);
            // Guard
            // I expected more… 
            enemySource.PlayOneShot(guardClips[3]);
            yield return new WaitForSeconds(guardClips[3].length);
            playerLostFightSequence.enabled = true;
            StartCoroutine(playerLostFightSequence.Sequence());
            yield break;
        }
    }
    #endregion

    #region PLAYER COMBAT FLOW
    IEnumerator PlayerAttack(){ 
        attacking = true;
        playerAttackInt = RandomNumberGen("attack");
        playerSource.PlayOneShot(playerAttackClips[playerAttackInt]);
        yield return new WaitForSeconds(2f);
        if (enemyInRange) StartCoroutine(PlayerHitEnemy());
        attacking = false;
    }

    IEnumerator PlayerHitEnemy(){
        attackPhase += 1;
        playerSource.PlayOneShot(playerPunchClips[Random.Range(0, playerPunchClips.Length)]);
        enemySource.Stop();
        if (enemyCoroutine != null) StopCoroutine(enemyCoroutine);
        enemySource.PlayOneShot(enemyHitByAttackClips[Random.Range(0, enemyHitByAttackClips.Length)]);
        AssignCoroutine("KnockbackEnemy");
        yield break;
    }

    IEnumerator KnockbackEnemy(){
        Vector3 newPosition = Vector3.MoveTowards(enemySource.transform.position, playerSource.transform.position, -8f);
        if (newPosition.z < wallTransform.position.z){
            enemySource.transform.position = newPosition;
        } else {
            enemySource.transform.position = new Vector3(newPosition.x, newPosition.y, wallTransform.position.z - 10f);
        }
        AssignCoroutine("MoveBackAndToSide");
        yield break;
    }

    IEnumerator PlayerRanAway(){
        controls.canZoom = true;
        controls.inCombat = false;
        StopCoroutine(enemyCoroutine);
        moveTowards = false;
        StartCoroutine(audioController.FadeMusic());
        playerSource.PlayOneShot(playerRanAwayClip);
        yield return new WaitForSeconds(playerRanAwayClip.length);
        darkVsLight.playerDarkness -= 1;
        attackPhase = 0;
        playerHealth = 3 + darkVsLight.playerDarkness;
        enemySource.PlayOneShot(enemyLostPlayerClip);
        yield return new WaitForSeconds(enemyLostPlayerClip.length);
        AssignCoroutine("MoveToPointOne");
        yield break;
    }
    #endregion

    IEnumerator DistanceFromEnemySignifier(){
        yield return new WaitForSeconds(distanceFromPlayer / 30); 
        enemySource.PlayOneShot(beepingClip);
        if (!audioController.musicSource.isPlaying) audioController.PlayMusic("stealth", 0.05f);
        if (controls.crouching && controls.inStealth){
            StartCoroutine(DistanceFromEnemySignifier()); 
            yield break;
        } 
        startedDetectionBeep = false;
        yield break;
    }
}
