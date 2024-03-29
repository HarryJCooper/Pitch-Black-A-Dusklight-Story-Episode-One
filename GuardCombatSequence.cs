using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DearVR;

public class GuardCombatSequence : MonoBehaviour
{
    [SerializeField] float maxDistanceFromPlayer = 3, enemyMoveSpeed;
    float angleToEnemy, enemyAttackSpeed;
    [SerializeField] Transform wallTransform;
    Controls controls;
    AudioSource playerSource;
    public AudioSource enemySource;
    DearVRSource enemyVRSource;
    bool moveTowards, moveBackAndToSide, moveToPointOne, moveToPointTwo, reachedPointOne, reachedPointTwo, enemyInRange, canParry, attacking, startedFight, fightOver, hasPlayedHeardProtag, stopPlayerAttack;
    // HERE HERE - reset player health to 3
    int attackPhase, playerDiedCount, playerAttackInt, playerBeenHitInt, randomInt;
    public IEnumerator enemyCoroutine, playerAttackCoroutine;
    [SerializeField] PlayerWonFightSequence playerWonFightSequence;
    [SerializeField] PlayerLostFightSequence playerLostFightSequence;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] AudioController audioController;
    [SerializeField] AudioSource doorSource, outOfWorldSource;
    [SerializeField] AudioClip accessDeniedClip, accessDeniedAgainClip, protagMakeDecisionClip, charlieInstructionClipMob, charlieInstructionClipComp;
    [SerializeField] Explosion explosion;
    public int playerHealth;

    #region PATROL VARIABLES
    [SerializeField] Transform guardPositionOne, guardPositionTwo;
    [SerializeField] AudioClip beepingClip;
    #endregion

    #region STEALTH VARIABLES
    public float currentDetectionDistance, detectionDistance, distanceFromPlayer, detectionTimer, maxDetectionTime;
    bool startedDetectionBeep, hasEnteredStealth, hasPlayedMakeDecision;
    [SerializeField] AudioClip enemyFoundPlayerSneakingClip, playerFoundSneakingClip, playerFoundWalkingClip, playerFoundClang;
    #endregion

    #region COMBAT AUDIOCLIPS
    [SerializeField] AudioClip playerMissedParryClip, playerRanAwayClip, playerBeenKilledClip,
        enemyAttackClip, enemyBeenKilledClip, enemyLostPlayerClip, drawBatonClip;
    [SerializeField] AudioClip[] playerParryClips, playerAttackClips, playerIsHitByAttackClips, playerPunchClips,
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
        enemyVRSource = enemySource.GetComponent<DearVRSource>();
        enemyVRSource.performanceMode = true;
        playerSource = GameObject.Find("Player").GetComponent<AudioSource>();
        controls = GameObject.Find("Controls").GetComponent<Controls>();
        AssignCoroutine("EnemyPatrolOne");
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

    IEnumerator PlayProtagMakeDecisionClip(){
        playerSource.PlayOneShot(protagMakeDecisionClip);
        yield return new WaitForSeconds(protagMakeDecisionClip.length);
        if (controls.computer){
            playerSource.PlayOneShot(charlieInstructionClipComp);
        } else {
            playerSource.PlayOneShot(charlieInstructionClipMob);
        }
    }

    void Update(){
        if (Vector3.Distance(transform.position, playerSource.transform.position) < 100 && !controls.inCombat){
            if (!playerSource.isPlaying && !hasPlayedMakeDecision && !fightOver){
                hasPlayedMakeDecision = true;
                StartCoroutine(PlayProtagMakeDecisionClip());
            } 
            if (!fightOver) controls.inStealth = true; 
        } else {
            controls.inStealth = false;  
        } 
        if (controls.crouching && !startedDetectionBeep){
            startedDetectionBeep = true;
            StartCoroutine(DistanceFromEnemySignifier());
        }
        if ((!controls.inStealth || !controls.crouching) && !controls.inCombat){
            StartCoroutine(audioController.FadeMusic());
        }
        if (controls.inStealth){
            DetectionDistanceModifier();
            CheckIfDetected();
            return;
        }
        if (controls.inCombat){
            if (canParry && controls.parry) StartCoroutine(Parry()); 
            if (controls.attack && !attacking && !stopPlayerAttack){
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
            // enemyCoroutine = MoveToPointOne();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "MoveToPointTwo"){
            enemyCoroutine = MoveToPointTwo();
            StartCoroutine(enemyCoroutine);
        } else if (desiredMethod == "MoveToPointThree"){
            // enemyCoroutine = MoveToPointThree();
            // StartCoroutine(enemyCoroutine);
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
        if (explosion.finished == 1) yield break;
        doorSource.PlayOneShot(accessDeniedClip);
        yield return new WaitForSeconds(accessDeniedClip.length);
        if (explosion.finished == 1) yield break;
        enemyVRSource.DearVRPlayOneShot(enemyPatrolClips[0]);
        yield return new WaitForSeconds(enemyPatrolClips[0].length);
        AssignCoroutine("EnemyPatrolTwo");
        yield break;
    }

    IEnumerator EnemyPatrolTwo(){
        // Next line is said with a fit of horrific anger, contrasting with the humorous tone of the prior line.
        // Guard
        // Ah! this door is a inconvenience
        if (explosion.finished == 1) yield break;
        doorSource.PlayOneShot(accessDeniedAgainClip);
        yield return new WaitForSeconds(accessDeniedAgainClip.length);
        if (explosion.finished == 1) yield break;
        enemyVRSource.DearVRPlayOneShot(enemyPatrolClips[1]);
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
        // huh! my ears detected something, guard mode initiated
        if (distanceFromPlayer < 150 && !controls.notMoving){
            hasPlayedHeardProtag = true;
            enemyVRSource.DearVRPlayOneShot(enemyPatrolClips[2]);
            yield return new WaitForSeconds(enemyPatrolClips[2].length);
        }
        AssignCoroutine("EnemyPatrolFour");
        yield break;
    }

    IEnumerator EnemyPatrolFour(){
        // The player has two approaches of dealing with the guard. After a while, the guard will stop his routine and turn to fixing the door, but the player has to wait a while for this. He leans into a comically scornful tone in the last utterance. 
        // Guard
        // Ahh it was like nothing. Now, time to fix this door fix this door. Once and for all… 
        yield return new WaitForSeconds(10f);
        if (hasPlayedHeardProtag){
            enemyVRSource.DearVRPlayOneShot(enemyPatrolClips[3]);
            yield return new WaitForSeconds(enemyPatrolClips[3].length);
            hasPlayedHeardProtag = false;
        }
        enemyVRSource.DearVRPlayOneShot(enemyPatrolClips[4]);
        yield return new WaitForSeconds(enemyPatrolClips[4].length);
        
        AssignCoroutine("MoveToPointFour");
        yield break;
    }

    IEnumerator MoveToPointFour(){
        moveToPointOne = true;
        if (reachedPointOne){
            moveToPointOne = false;
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
            enemyVRSource.DearVRStop();
            if (explosion.finished == 1) yield break;
            playerSource.PlayOneShot(playerFoundClang);
            enemyVRSource.DearVRPlayOneShot(enemyFoundPlayerSneakingClip);
            yield return new WaitForSeconds(enemyFoundPlayerSneakingClip.length);
            if (explosion.finished == 1) yield break;
            enemyVRSource.DearVRPlayOneShot(drawBatonClip);
            yield return new WaitForSeconds(drawBatonClip.length);
            if (explosion.finished == 1) yield break;
            playerSource.PlayOneShot(playerFoundSneakingClip);
        } else {
            stopPlayerAttack = true;
            controls.inCombat = true;  
            enemyVRSource.DearVRStop();
            if (explosion.finished == 1) yield break;
            playerSource.PlayOneShot(playerFoundClang);
            enemyVRSource.DearVRPlayOneShot(enemyFoundPlayerClips[0]);
            yield return new WaitForSeconds(enemyFoundPlayerClips[0].length);
            if (explosion.finished == 1) yield break;
            enemyVRSource.DearVRPlayOneShot(drawBatonClip);
            yield return new WaitForSeconds(drawBatonClip.length);
            if (explosion.finished == 1) yield break;
            enemyVRSource.DearVRPlayOneShot(enemyFoundPlayerClips[1]);
            yield return new WaitForSeconds(enemyFoundPlayerClips[1].length);
            if (explosion.finished == 1) yield break;
            playerSource.PlayOneShot(playerFoundWalkingClip);
            stopPlayerAttack = false;
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
        if (!controls.inCombat) yield break;
        enemyVRSource.DearVRPlayOneShot(enemyAttackClip);
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
            stopPlayerAttack = true;
            enemyVRSource.DearVRPlayOneShot(enemyTauntClips[0]);
            yield return new WaitForSeconds(enemyTauntClips[0].length);
            stopPlayerAttack = false;
        } 
        if (attackPhase == 4){
            stopPlayerAttack = true;
            enemyVRSource.DearVRPlayOneShot(enemyTauntClips[1]);
            yield return new WaitForSeconds(enemyTauntClips[1].length);
            stopPlayerAttack = false;
        }
        AssignCoroutine("MoveTowards");
    }

    // ________________ // 

    IEnumerator Parry(){
        if (enemyCoroutine != null) StopCoroutine(enemyCoroutine);
        canParry = false;
        enemyVRSource.DearVRStop();
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
        StartCoroutine(audioController.FadeMusic());
        enemyVRSource.DearVRPlayOneShot(enemyBeenKilledClip);
        yield return new WaitForSeconds(enemyBeenKilledClip.length);
        darkVsLight.playerDarkness += 1;
        playerWonFightSequence.enabled = true;
        controls.inCombat = false;
        StartCoroutine(playerWonFightSequence.Sequence());
        controls.canZoom = true;
    }

    IEnumerator PlayerKilled(){
        if (playerDiedCount == 0){
            stopPlayerAttack = true;
            StartCoroutine(audioController.FadeMusic());
            // Guard
            // I told you, we don’t want to make things even more interesting for me… stay. Down. Before I do more than knock you out… 
            enemyVRSource.DearVRPlayOneShot(guardClips[0]);
            yield return new WaitForSeconds(guardClips[0].length);
            // Protag 
            // Damn, only thing’s knocked is my ego…  
            playerSource.PlayOneShot(playerClips[0]);
            yield return new WaitForSeconds(playerClips[0].length);
            yield return new WaitForSeconds(1f);
            stopPlayerAttack = false;
            playerHealth = 3 + darkVsLight.playerDarkness;
            attackPhase = 0;
            AssignCoroutine("MoveTowards");
            audioController.PlayMusic("combat", 0.15f);
            playerDiedCount += 1;
            yield break;
        } 
        if (playerDiedCount == 1){
            stopPlayerAttack = true;
            StartCoroutine(audioController.FadeMusic());
            // Guard
            // Seriously, stay down, whatever it is you’re looking for, ain’t worth this. 
            enemyVRSource.DearVRPlayOneShot(guardClips[2]);
            yield return new WaitForSeconds(guardClips[2].length);
            // Protag 
            // Can’t go back… too many questions… not enough answers… 
            playerSource.PlayOneShot(playerClips[1]);
            yield return new WaitForSeconds(playerClips[1].length);
            stopPlayerAttack = false;
            playerHealth = 3 + darkVsLight.playerDarkness;
            attackPhase = 0;
            AssignCoroutine("MoveTowards");
            audioController.PlayMusic("combat", 0.15f);
            playerDiedCount += 1;
            yield break;
        } 
        if (playerDiedCount == 2){
            stopPlayerAttack = true;
            fightOver = true;
            controls.inCombat = false;
            controls.inStealth = false;
            stopPlayerAttack = false;
            StartCoroutine(audioController.FadeMusic());
            // Protag
            // Aghhh… 
            playerSource.PlayOneShot(playerClips[2]);
            yield return new WaitForSeconds(playerClips[2].length);
            outOfWorldSource.PlayOneShot(guardClips[3]); 
            // Guard
            // I expected more… 
            yield return new WaitForSeconds(guardClips[3].length -2f);
            playerLostFightSequence.enabled = true;
            playerLostFightSequence.active = 1;
            playerLostFightSequence.StartSequence();
            yield break;
        }
    }
    #endregion

    #region PLAYER COMBAT FLOW
    IEnumerator PlayerAttack(){
        attacking = true;
        playerAttackInt = RandomNumberGen("attack");
        playerSource.Stop();
        playerSource.PlayOneShot(playerAttackClips[playerAttackInt]);
        yield return new WaitForSeconds(playerAttackClips[playerAttackInt].length);
        if (enemyInRange) StartCoroutine(PlayerHitEnemy());
        attacking = false;
    }

    IEnumerator PlayerHitEnemy(){
        attackPhase += 1;
        playerSource.PlayOneShot(playerPunchClips[Random.Range(0, playerPunchClips.Length)]);
        enemyVRSource.DearVRStop();
        if (enemyCoroutine != null) StopCoroutine(enemyCoroutine);
        enemyVRSource.DearVRPlayOneShot(enemyHitByAttackClips[Random.Range(0, enemyHitByAttackClips.Length)]);
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
        // HERE HERE HERE HERE HERE HERE HERE
        // playerSource.PlayOneShot(playerRanAwayClip);
        // yield return new WaitForSeconds(playerRanAwayClip.length);
        darkVsLight.playerDarkness -= 1;
        attackPhase = 0;
        playerHealth = 3 + darkVsLight.playerDarkness;
        enemyVRSource.DearVRPlayOneShot(enemyLostPlayerClip);
        yield return new WaitForSeconds(enemyLostPlayerClip.length);
        yield return new WaitForSeconds(3f);
        audioController.musicSource.Stop();
        audioController.SetMusicToZero();
        AssignCoroutine("MoveToPointOne");
        yield break;
    }
    #endregion

    IEnumerator DistanceFromEnemySignifier(){
        yield return new WaitForSeconds(distanceFromPlayer / 30); 
        enemyVRSource.DearVRPlayOneShot(beepingClip);
        if (!audioController.musicSource.isPlaying){
            audioController.musicSource.volume = 1;
            audioController.SetMusicToZero();
            audioController.PlayMusic("stealth", 0.05f);
        } 
        if (controls.crouching && controls.inStealth){
            StartCoroutine(DistanceFromEnemySignifier()); 
            yield break;
        } 
        startedDetectionBeep = false;
        yield break;
    }
}
