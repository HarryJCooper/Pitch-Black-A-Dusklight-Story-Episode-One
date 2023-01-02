using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DearVR;

public class Parry : MonoBehaviour
{
    [SerializeField] float timer;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] int[] attackSequence, playerDazedSequence;
    [SerializeField] AudioClip enemyAttackClip, tickClip, parryClip, playerHitClip, errorClip, enemyDazedClip;
    [SerializeField] AudioSource playerSource, enemySource;
    [SerializeField] int dazedLength = 2, knockbackDistance = 10;
    int attackCounter, numberOfAttacks, parriedAttacks, missedParryWaiter;
    public bool sequenceRunning;
    bool canParry, canAttack, hasParried, missedParry, dazed;

    void BlockPlayerAttacking(){
        for (int i = 0; i <= dazedLength; i++) if (attackCounter + i < playerDazedSequence.Length) playerDazedSequence[attackCounter + i] = 1;
    }

    void DazeEnemy(){
        Debug.Log("Enemy is dazed");
        enemySource.PlayOneShot(enemyDazedClip);
    }

    void PlayerAttack(){
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (canAttack && playerDazedSequence[attackCounter] == 0){
            parriedAttacks += 1;
            Debug.Log("hasParried");
            playerSource.PlayOneShot(parryClip);
            hasParried = true;
            enemySource.Stop();
            // canParry = attack;
        } else {
            playerSource.PlayOneShot(errorClip);
            playerDazedSequence[attackCounter] = 1;
            Debug.Log(attackCounter);
            BlockPlayerAttacking();
        }
    }

    void PlayerParry(){
        if (!Input.GetKeyDown(KeyCode.Return)) return;

        if (canParry && playerDazedSequence[attackCounter] != 1){
            parriedAttacks += 1;
            Debug.Log("hasParried");
            playerSource.PlayOneShot(parryClip);
            hasParried = true;
            enemySource.Stop();
            canParry = false;
        } else {
            playerSource.PlayOneShot(errorClip);
            playerDazedSequence[attackCounter] = 1;
            Debug.Log(attackCounter);
            BlockPlayerAttacking();
        }
    }

    void Knockback(){
        playerSource.transform.position = Vector3.MoveTowards(playerSource.transform.position, enemySource.transform.position, knockbackDistance);
    }

    void PlayerHit(){
        Debug.Log("Player Been Hit");
        playerSource.PlayOneShot(playerHitClip);
    }

    void Reset(){
        attackCounter = 0;
        parriedAttacks = 0;
        Debug.Log("finished ticking");
        sequenceRunning = false;
    }

    IEnumerator Tick(){
        Debug.Log("attack counter = " + attackCounter);
        playerSource.PlayOneShot(tickClip);

        if (attackSequence[attackCounter] == 1){
            canParry = true;
            enemySource.PlayOneShot(enemyAttackClip);
            Debug.Log("parry tick");
        } else {
            canAttack = true;
            Debug.Log("attack tick");
        }

        yield return new WaitForSeconds(timer);
        
        if (!hasParried && attackSequence[attackCounter] == 1){
            playerSource.PlayOneShot(playerHitClip);
        }

        hasParried = false;
        canParry = false;
        attackCounter += 1;

        if (attackCounter >= attackSequence.Length){
            if (parriedAttacks == numberOfAttacks) DazeEnemy();
            else PlayerHit();
            Knockback();
            Reset();
            yield break;
        }

        StartCoroutine(Tick());
    }

    void GetNumberOfAttacks(){
        foreach(int attackInt in attackSequence) if(attackInt == 1) numberOfAttacks += 1;
        playerDazedSequence = new int[attackSequence.Length]; 
    }

    void Update(){
        if (sequenceRunning) PlayerParry();
    }

    public void StartSequence(){
        if (sequenceRunning) return;
        sequenceRunning = true;
        GetNumberOfAttacks();
        StartCoroutine(Tick());
    }
}
