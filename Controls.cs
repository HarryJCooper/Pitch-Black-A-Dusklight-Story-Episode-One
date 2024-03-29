﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] private float doubleTapDelta = 0.5f;

    public bool canZoom, tap, doubleTap, swipeLeft, swipeRight, swipeUp, swipeDown;
    public bool currentlyPaused, mobile, computer, canPause;
    private Vector2 swipeDelta, startTouch;
    private float lastTapTime, lastRightTime, lastLeftTime, lastSpaceTime, leftTime, rightTime, spaceTime;
    public float pauseTimer;

    // GENERAL MOVEMENT
    public bool doubleTapRight, doubleTapLeft, doubleTapSpace, turnRight, turnLeft, moveForward, moveBackward, firstMoveForward, firstMoveBackward, sprint, crouching, notMoving;

    // MODE
    public bool inZoom, inCombat, inStealth, inCutscene, enteredZoom, inPause, inExplore;
    
    // COMBAT
    public bool parry, attack, quickRotateRight, quickRotateLeft;
    public bool paused, enter, cutscenePlaying, allowPause, lockZoom;
    public Vector3 touchPosition;
    public Touch touch;
    [SerializeField] Joystick joystick;
    [SerializeField] float minDirection, sprintModifier;
    float firstStepCounter;

    // IN MENU
    public bool inMenu, moveUp, moveDown;

    // CROUCH CLIP
    [SerializeField] AudioClip crouchClip;
    [SerializeField] AudioSource playerActionSource;

    void Awake(){ 
        canPause = true;
    }

    void Start(){
        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor){
            computer = true;
            mobile = false;
            return;
        }
        computer = false;
        mobile = true;
    }
    IEnumerator DoublePressStopper(){
        yield return new WaitForSeconds(0.1f);
        canPause = true;
    }
    void ResetControls(){
        swipeUp = swipeDown = swipeLeft = swipeRight = tap = quickRotateLeft = quickRotateRight = doubleTapLeft = doubleTapRight = doubleTapSpace
        = attack = parry = moveUp = moveDown = moveForward = moveBackward = turnRight = turnLeft = false;
    }
    void CheckIfInExplore(){
        if (!inZoom && !inCombat && !inStealth && !inCutscene && !enteredZoom && !inPause){
            inExplore = true; 
        } else {
            inExplore = false; 
        }
    }
    void CheckForEnter(){
        if (doubleTap || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
            enter = true; return;
        } 
        enter = false;
    }
    void CheckForSprint(){
        if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)){
            sprint = true; return;
        } 
        sprint = false;
    }
    void CheckForDoubleRight(){
        if (!Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.D)) return;
        rightTime = Time.time - lastRightTime;
        if (rightTime < doubleTapDelta) doubleTapRight = true;
        lastRightTime = Time.time;
    }
    void CheckForDoubleLeft(){
        if (!Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.A)) return;
        leftTime = Time.time - lastLeftTime;
        if (leftTime < doubleTapDelta) doubleTapLeft = true;
        lastLeftTime = Time.time;
    }

    void CheckForDoubleSpace(){
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        spaceTime = Time.time - lastSpaceTime;
        if (spaceTime < doubleTapDelta) doubleTapSpace = true;
        lastSpaceTime = Time.time;
    }

    void TurnRight(){ if ((joystick.Horizontal > minDirection) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) turnRight = true; else turnRight = false; }
    void TurnLeft(){ if ((joystick.Horizontal < -minDirection) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) turnLeft = true; else turnLeft = false; }
    void MoveForward(){
        if ((joystick.Vertical > minDirection) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)){
            moveForward = true;
            firstStepCounter += Time.deltaTime;
        } else {
            moveForward = false;
        }
    }
    void MoveBackward(){
        if ((joystick.Vertical < -minDirection) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)){
            moveBackward = true;
            firstStepCounter += Time.deltaTime;
        } else {
            moveBackward = false;
        }
    }
    void CheckForNotMoving(){ if (!moveForward && !moveBackward) notMoving = true; else notMoving = false;}
    void CheckForFirstStep(){
        if (!(joystick.Vertical > minDirection) && !(joystick.Vertical < -minDirection) 
        && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)){
            firstStepCounter = 0;
        }
        if (firstStepCounter < 0.1 && moveForward) firstMoveForward = true; else firstMoveForward = false;
        if (firstStepCounter < 0.1 && moveBackward) firstMoveBackward = true; else firstMoveBackward = false;
    }
    void CheckForZoom(){
        if (!canZoom) return;
        if (doubleTap || Input.GetKeyDown(KeyCode.Space)){
            enteredZoom = true;
            inZoom = true;
            doubleTap = false;
        } else {
            enteredZoom = false;
        }
        if (!Input.GetKey(KeyCode.Space) && !lockZoom && computer){
            inZoom = false;
        }
        
        if (Input.touchCount == 0 && !lockZoom && mobile){
            inZoom = false;
        } 
    }
    void CheckForCombat(){
        if (!inCombat) return;
        inStealth = canZoom = inZoom = enteredZoom = crouching = false;
        if (swipeUp || Input.GetKeyDown(KeyCode.Return)) parry = true; 
        if (doubleTap || Input.GetKeyDown(KeyCode.Space)) attack = true;
        // if (swipeRight || doubleTapLeft) quickRotateLeft = true;
        // if (swipeLeft || doubleTapRight) quickRotateRight = true;
    }
    void CheckForStealth(){
        if (!inStealth) return;
        inCombat = false;
        if (Input.GetKeyDown(KeyCode.LeftControl) || swipeDown){
            crouching = !crouching;
            if (crouching) playerActionSource.PlayOneShot(crouchClip, 0.15f);
        }
    }
    void CheckForPause(){
        if (Input.touchCount == 2) pauseTimer += Time.deltaTime; else pauseTimer = 0;
        if ((pauseTimer > 1 || Input.GetKeyDown(KeyCode.Escape)) && !paused && canPause){
            paused = inMenu = true;
            canZoom = canPause = false;
            StartCoroutine(DoublePressStopper());
            return;
        }
        if (paused && canPause && Input.GetKeyDown(KeyCode.Escape)){
            pauseTimer = 0;
            canZoom = true;
            paused = inMenu = false;
            StartCoroutine(DoublePressStopper());
        }
    }
    void CheckForMenu(){
        if (!inMenu) return;
        if (swipeUp || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)){
            moveDown = true; 
            return;
        }
        if (swipeDown || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
            moveUp = true; 
            return;
        }
    }

    void UpdateMobile(){
        // if (!mobile) return;
        if (doubleTap) doubleTap = false;
        if (Input.touches.Length != 0){
            if (Input.touches[0].phase == TouchPhase.Began){
                tap = true;
                startTouch = Input.mousePosition;
                doubleTap = Time.time - lastTapTime < doubleTapDelta;
                lastTapTime = Time.time;
            } else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled){
                startTouch = swipeDelta = Vector2.zero;
            }
        }
        swipeDelta = Vector2.zero;
        if (startTouch != Vector2.zero && Input.touches.Length != 0) swipeDelta = Input.touches[0].position - startTouch;

        if (swipeDelta.sqrMagnitude > 0){
            float x = swipeDelta.x;
            float y = swipeDelta.y;
            
            if (x < 0){
                swipeLeft = true;
            } else {
                swipeRight = true;
            } 

            if (y < 0){
                swipeDown = true;
            } else {
                swipeUp = true;
            }
            startTouch = swipeDelta = Vector2.zero;
        }
        if (Input.touchCount == 1){
            touch = Input.GetTouch(0);
            touchPosition = touch.position;
            if (Input.GetTouch(0).phase == TouchPhase.Ended) touchPosition.x = 0;
        }
    }

    void Update(){
        ResetControls();
        UpdateMobile();
        CheckIfInExplore();
        CheckForEnter();
        TurnRight();
        TurnLeft();
        MoveForward();
        MoveBackward();
        CheckForNotMoving();
        CheckForFirstStep();
        CheckForSprint();
        CheckForDoubleRight();
        CheckForDoubleLeft();
        CheckForDoubleSpace();
        CheckForZoom();
        CheckForCombat();
        CheckForStealth();
        CheckForPause();
        CheckForMenu();
    }
}
