using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] private float doubleTapDelta = 0.5f;

    public bool canZoom, tap, doubleTap, swipeLeft, swipeRight, swipeUp, swipeDown;
    public bool currentlyPaused, mobile, computer, canPause;
    private Vector2 swipeDelta, startTouch;
    private float lastTapTime, lastRightTime, lastLeftTime, leftTime, rightTime;
    public float pauseTimer;

    // GENERAL MOVEMENT
    public bool doubleTapRight, doubleTapLeft, turnRight, turnLeft, moveForward, moveBackward, firstMoveForward, firstMoveBackward, sprint, crouching, notMoving;

    // MODE
    public bool inZoom, inCombat, inStealth, inCutscene, enteredZoom, inPause, inExplore;
    
    // COMBAT
    public bool parry, attack, quickRotateRight, quickRotateLeft;
    public bool paused, enter, cutscenePlaying, allowPause;
    public Vector3 touchPosition;
    public Touch touch;
    [SerializeField] Joystick joystick;
    [SerializeField] float minDirection, sprintModifier;
    float firstStepCounter;

    // IN MENU
    public bool inMenu, moveUp, moveDown;

    void Awake(){ 
        canZoom = true;
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
        swipeUp = swipeDown = swipeLeft = swipeRight = tap = quickRotateLeft = quickRotateRight = doubleTapLeft = doubleTapRight 
        = attack = parry = moveUp = moveDown = moveForward = moveBackward = turnRight = turnLeft = false;
    }
    void CheckIfInExplore(){if (!inZoom && !inCombat && !inStealth && !inCutscene && !enteredZoom && !inPause) inExplore = true; else inExplore = false; }
    void CheckForEnter(){
        if (doubleTap || Input.GetKeyDown(KeyCode.Space)){
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
        if (!Input.GetKeyDown(KeyCode.RightArrow)) return;
        rightTime = Time.time - lastRightTime;
        if (rightTime < doubleTapDelta) doubleTapRight = true;
        lastRightTime = Time.time;
    }
    void CheckForDoubleLeft(){
        if (!Input.GetKeyDown(KeyCode.LeftArrow)) return;
        leftTime = Time.time - lastLeftTime;
        if (leftTime < doubleTapDelta) doubleTapLeft = true;
        lastLeftTime = Time.time;
    }
    void TurnRight(){ if ((joystick.Horizontal > minDirection) || Input.GetKey(KeyCode.RightArrow)) turnRight = true; else turnRight = false; }
    void TurnLeft(){ if ((joystick.Horizontal < -minDirection) || Input.GetKey(KeyCode.LeftArrow)) turnLeft = true; else turnLeft = false; }
    void MoveForward(){
        if ((joystick.Vertical > minDirection) || Input.GetKey(KeyCode.UpArrow)){
            Debug.Log("MoveForward Controls");
            moveForward = true;
            firstStepCounter += Time.deltaTime;
        } else {
            moveForward = false;
        }
    }
    void MoveBackward(){
        if ((joystick.Vertical < -minDirection) || Input.GetKey(KeyCode.DownArrow)){
            Debug.Log("MoveBackward Controls");
            moveBackward = true;
            firstStepCounter += Time.deltaTime;
        } else {
            moveBackward = false;
        }
    }
    void CheckForNotMoving(){ if (!moveForward && !moveBackward) notMoving = true; else notMoving = false;}
    void CheckForFirstStep(){
        if (!(joystick.Vertical > minDirection) && !(joystick.Vertical < -minDirection) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)){
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
        if (Input.GetKeyUp(KeyCode.Space)) inZoom = false;
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended) inZoom = false;
    }
    void CheckForCombat(){
        if (!inCombat) return;
        inStealth = canZoom = inZoom = enteredZoom = crouching = false;
        if (swipeUp || Input.GetKeyDown(KeyCode.Return)){ parry = true; Debug.Log("Parry"); } 
        if (doubleTap || Input.GetKeyDown(KeyCode.Space)) attack = true;
        // if (swipeRight || doubleTapLeft) quickRotateLeft = true;
        // if (swipeLeft || doubleTapRight) quickRotateRight = true;
    }
    void CheckForStealth(){
        if (!inStealth) return;
        inCombat = false;
        if (Input.GetKeyDown(KeyCode.LeftControl) || swipeDown) crouching = !crouching;
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
        if (swipeUp || Input.GetKeyDown(KeyCode.DownArrow)){
            moveDown = true; 
            return;
        }
        if (swipeDown || Input.GetKeyDown(KeyCode.UpArrow)){
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
        CheckForZoom();
        CheckForCombat();
        CheckForStealth();
        CheckForPause();
        CheckForMenu();
    }
}
