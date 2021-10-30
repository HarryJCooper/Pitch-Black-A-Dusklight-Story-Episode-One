using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    [SerializeField] private float doubleTapDelta = 0.5f;

    public bool canZoom, tap, doubleTap, swipeLeft, swipeRight, swipeUp, swipeDown;
    public bool currentlyPaused, mobile, computer, canPause, canFocus;
    private Vector2 swipeDelta, startTouch;
    private float lastTapTime, lastRightTime, lastLeftTime, leftTime, rightTime;
    public float pauseTimer;

    // GENERAL MOVEMENT
    public bool turnRight, turnLeft, moveForward, moveBackward, firstMoveForward, firstMoveBackward, sprint, crouching, notMoving;
    bool hold;

    // MODE
    public bool inZoom, inCombat, inStealth, inCutscene, enteredZoom, inPause;
    
    // COMBAT
    public bool parry, fastAttack, slowAttack, quickRotateRight, quickRotateLeft;
    public bool paused, enter, cutscenePlaying, allowPause;
    public Vector3 touchPosition;
    public Touch touch;
    [SerializeField] Joystick joystick;
    [SerializeField] float minDirection, sprintModifier;
    float firstStepCounter;

    // REFACTOR - public ChooseDisplayImage chooseDisplayImage;

    private void Awake(){
        canZoom = true; // REFACTOR - This is just for testing, should start at false. 
    }

    private void Start(){
        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.WindowsEditor){
            computer = true;
            mobile = false;
        }
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer){
            computer = false;
            mobile = true;
        }
    }

    IEnumerator DoublePressStopper(){
        yield return new WaitForSeconds(0.1f);
        canPause = true;
    }

    IEnumerator DoubleTapAndHoldAttack(){
        yield return new WaitForSeconds(0.15f);
        if (hold){
            slowAttack = true;
            fastAttack = false;
        } else {
            slowAttack = false;
            fastAttack = true;
        }
    }

    void Update(){
        if (Input.GetMouseButton(0) || Input.touchCount > 0 || Input.GetKey(KeyCode.Space)){
            hold = true;
        } else {
            hold = false;
        }
        swipeUp = swipeDown = swipeLeft = swipeRight = tap = quickRotateLeft = quickRotateRight = false;

        if (doubleTap || Input.GetKeyDown(KeyCode.Space)){
            enter = true;
        } else {
            enter = false;
        }

        #region Sprint
        // joystick.Vertical > sprintModifier
        // REFACTOR - Removed for first mobile build
        if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)){
            sprint = true;
        } else {
            sprint = false;
        }
        #endregion

        #region Movement Controls
        // REFACTOR - JOYSTICK CONTROLS
        if ((joystick.Horizontal > minDirection) || Input.GetKey(KeyCode.RightArrow)){
            turnRight = true;
        } else {
            turnRight = false;
        }

        if ((joystick.Horizontal < -minDirection) || Input.GetKey(KeyCode.LeftArrow)){
            turnLeft = true;
        } else {
            turnLeft = false;
        }

        if ((joystick.Vertical > minDirection) || Input.GetKey(KeyCode.UpArrow)){
            moveForward = true;
            firstStepCounter += Time.deltaTime;
        } else {
            moveForward = false;
        }

        if ((joystick.Vertical < -minDirection) || Input.GetKey(KeyCode.DownArrow)){
            moveBackward = true;
            firstStepCounter += Time.deltaTime;
        } else {
            moveBackward = false;
        }

        if (!moveForward && !moveBackward) notMoving = true; else notMoving = false;
        #endregion

        #region First Step Forward 
        if (!(joystick.Vertical > minDirection) && !(joystick.Vertical < -minDirection) && !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow)){
            firstStepCounter = 0;
        }
        
        if (firstStepCounter < 0.1 && moveForward) firstMoveForward = true;
        else firstMoveForward = false;

        if (firstStepCounter < 0.1 && moveBackward) firstMoveBackward = true;
        else firstMoveBackward = false;
        #endregion

        #region Zoom
        if (canZoom){
            if (doubleTap || Input.GetKeyDown(KeyCode.Space)){
                Debug.Log("inZoom");
                enteredZoom = true;
                inZoom = true;
                if (mobile) {
                    Handheld.Vibrate();
                }
                doubleTap = false;
            } else {
                enteredZoom = false;
            }

            // MOBILE
            if (Input.touchCount == 1){
                if (Input.GetTouch(0).phase == TouchPhase.Ended){
                    inZoom = false;
                    Debug.Log("exit Zoom");
                    if (mobile){
                        Handheld.Vibrate();
                    }
                }
            }

            // COMPUTER
            if (Input.GetKeyUp(KeyCode.Space)){
                inZoom = false;
                Debug.Log("exit Zoom");
            }
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.C)){
            inCombat = true;
            StartCoroutine(GameObject.Find("Enemy").GetComponent<Combat>().EnemyMoveTowardsOrTaunt());
        }

        #region inCombat
        if (inCombat){
            inStealth = canZoom = false;
            if (swipeUp || Input.GetKeyDown(KeyCode.Return)) parry = true;
            if (doubleTap || Input.GetKeyDown(KeyCode.Space)) StartCoroutine(DoubleTapAndHoldAttack());
            if (swipeRight) quickRotateLeft = true;
            if (swipeLeft) quickRotateRight = true;
            if(Input.GetKeyDown(KeyCode.RightArrow)){
                rightTime = Time.time - lastRightTime;
                if (rightTime < doubleTapDelta){
                    quickRotateRight = true;
                }
                lastRightTime = Time.time;
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow)){
                leftTime = Time.time - lastLeftTime;
                if (leftTime < doubleTapDelta){
                    quickRotateLeft = true;
                }
                lastLeftTime = Time.time;
            }
        }
        #endregion

        #region inStealth
        if (inStealth){
            inCombat = canZoom = false;
            if (Input.GetKeyDown(KeyCode.LeftControl) || swipeDown) 
                if (crouching) crouching = false; else crouching = true;
        }
        #endregion

        if (doubleTap) doubleTap = false;

        #region Pause
        if (Input.touchCount == 2 && allowPause) pauseTimer += Time.deltaTime; else pauseTimer = 0;

        // REFACTOR - All controls should be dictated by simple states, e.g. InCutscene, InMainGame. 

        if ((pauseTimer > 1 || Input.GetKeyDown(KeyCode.Escape)) && !paused && canPause){
            Debug.Log("paused");
            paused = true;
            canZoom = false;
            canPause = false;
            StartCoroutine(DoublePressStopper());
        }

        if (paused && Input.GetKeyDown(KeyCode.Escape)){
            Debug.Log("off Pause");
            pauseTimer = 0;
            paused = false;
            StartCoroutine(DoublePressStopper());
        }
        #endregion

        if (mobile){
            UpdateMobile();
        }

        if (Input.touchCount == 1){
            touch = Input.GetTouch(0);
            touchPosition = touch.position;
            if (Input.GetTouch(0).phase == TouchPhase.Ended){
                touchPosition.x = 0;
            }
        }
    }

    private void UpdateMobile(){
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
        
        if (startTouch != Vector2.zero && Input.touches.Length != 0){
            swipeDelta = Input.touches[0].position - startTouch;
        }

        if (swipeDelta.sqrMagnitude > 0){
            float x = swipeDelta.x;
            float y = swipeDelta.y;
            if (Mathf.Abs(x) > Mathf.Abs(y)){
                if (x < 0) swipeLeft = true; else swipeRight = true;
            } else {
                if (y < 0) swipeDown = true; else swipeUp = true;
            }
            startTouch = swipeDelta = Vector2.zero;
        }
    }
}
