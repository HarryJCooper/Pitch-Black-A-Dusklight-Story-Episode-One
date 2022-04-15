using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBFootstepSystem : MonoBehaviour
{   public bool isBaby;
    public AudioClip[] footstepClips;
    public float walkSpeedModifier;
    public float walkDist, maxDist, moveSpeed, sprintMoveSpeed, initialMoveSpeed, crouchMoveSpeed, initialFootstepDelay;
    public float rotSpeed, rotationTimer;
    Vector3 recentTransform;
    float fWallDist, bWallDist;
    [SerializeField] float maxWallDist;
    const float maxDistance = 200;
    [SerializeField] AudioSource footstepSource;
    public bool canMove, firstLevel, canRotate = true;
    [SerializeField] Controls controls;
    bool initialFootstepAllow;
    int layerMask, footstepInt;

    void Start(){
        fWallDist = bWallDist = 5;
        controls = GameObject.Find("Controls").GetComponent<Controls>();
        layerMask = 1 << 12;
        layerMask = ~layerMask;
    }

    void CheckIfCanMove(){
        if (controls.inCutscene || controls.paused) {
            canMove = false; 
            return;
        }
        canMove = true;
    }

    void IncreaseRotationTimer(){
        if (rotationTimer > 0.60f){       
            footstepSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)], Random.Range(0.8f, 1f));
            rotationTimer = 0;
        }
        if (controls.turnRight && !controls.moveForward && !controls.moveBackward) {rotationTimer += Time.deltaTime; return;}
        if (controls.turnLeft && !controls.moveForward && !controls.moveBackward) {rotationTimer += Time.deltaTime; return;}
    }

    void CheckForRotate(){
        IncreaseRotationTimer();
        if (controls.turnLeft) {transform.Rotate(Vector3.up, -rotSpeed * Time.deltaTime); return;}
        if (controls.turnRight) {transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime); return;}
        if (controls.quickRotateLeft) {transform.Rotate(Vector3.up, -90); return;}
        if (controls.quickRotateRight) {transform.Rotate(Vector3.up, 90); return;}
    }

    void FirstMoveBackward(){
        recentTransform = transform.position;
        float footstepVol = controls.crouching ? Random.Range(0.2f, 0.3f) : Random.Range(0.8f, 1f);
        footstepSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)], footstepVol);
        initialFootstepAllow = false;
        initialFootstepDelay = 0;
    }

    void FirstMoveForward(){
        recentTransform = transform.position;
        float footstepVol = controls.crouching ? Random.Range(0.05f, 0.1f) : Random.Range(0.4f, 0.5f);
        footstepSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)], footstepVol);
        initialFootstepAllow = false;
        initialFootstepDelay = 0;
    }

    void CheckFirstMove(){
        if (!initialFootstepAllow) return;
        if (controls.firstMoveForward) FirstMoveForward();
        if (controls.firstMoveBackward) FirstMoveBackward(); 
    }

    void RaycastToWall(){
        Vector3 origin = transform.position;
        Vector3 fDirection = transform.forward;
        Vector3 bDirection = -transform.forward;
        Ray fRay = new Ray(origin, fDirection);
        Ray bRay = new Ray(origin, bDirection);
        if (Physics.Raycast(fRay, out RaycastHit fRaycastHit, maxDistance, layerMask, QueryTriggerInteraction.Ignore)) fWallDist = fRaycastHit.distance;
        else fWallDist = 200;
        if (Physics.Raycast(bRay, out RaycastHit bRaycastHit, maxDistance, layerMask, QueryTriggerInteraction.Ignore)) bWallDist = bRaycastHit.distance;
        else bWallDist = 200;
    }

    void IncreaseWalkDist(){
        if (controls.moveForward){ 
            walkDist += Vector3.Distance(recentTransform, transform.position) * Time.deltaTime * 50; 
            return;
        }
        if (controls.moveBackward){ 
            walkDist += Vector3.Distance(recentTransform, transform.position) * Time.deltaTime * 25; 
            return;
        }
        walkDist = 0;
        recentTransform = transform.position;
    }

    void MoveForwardAndBackward(){
        if (controls.moveForward && fWallDist > maxWallDist) {
            transform.position += transform.forward * Time.deltaTime * moveSpeed; 
            return;
        }
        if (controls.moveBackward && bWallDist > maxWallDist) {
            transform.position -= transform.forward * Time.deltaTime * moveSpeed; 
            return;
        }
    }

    int RandomNumberGen(){
        int randomInt = Random.Range(0, footstepClips.Length);
        if (randomInt == footstepInt) randomInt = RandomNumberGen();
        return randomInt;
    }

    void CheckIfPlayFootstep(){
        if (walkDist > maxDist && walkDist < (maxDist + 20)){
            footstepInt = RandomNumberGen();
            footstepSource.PlayOneShot(footstepClips[footstepInt], Random.Range(0.8f, 1f));
            if (isBaby) maxDist = Random.Range(50f, 70f);
            walkDist = 0;
            recentTransform = transform.position;
        }
    }

    void CheckMoveSpeed(){
        if (controls.sprint) {moveSpeed = sprintMoveSpeed + walkSpeedModifier; return;}
        if (controls.crouching) {moveSpeed = crouchMoveSpeed + walkSpeedModifier; return;}
        moveSpeed = initialMoveSpeed + walkSpeedModifier;
    }

    void CheckForMovement(){
        if (!initialFootstepAllow) initialFootstepDelay += Time.deltaTime;
        if (initialFootstepDelay > 0.3) initialFootstepAllow = true; 
        RaycastToWall();  
        IncreaseWalkDist();
        MoveForwardAndBackward();
        CheckIfPlayFootstep();
        CheckMoveSpeed();
        CheckFirstMove();
    }

    void Update(){
        CheckIfCanMove();
        if (canRotate) CheckForRotate();
        if (canMove) CheckForMovement();
    }
}
