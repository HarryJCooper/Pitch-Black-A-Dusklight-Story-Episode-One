using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    bool hasPlayedIntro, continueGame, saveGame, controlsBool, compass, credits, close, hasMoved, paused, hasPaused;
    public bool compassActivated;
    bool moveUp, moveDown, enter;
    bool fadeAmbience;
    public AudioClip enterPauseClip, continueGameClip, saveGameClip, gameSavedClip, compassClip, compassOnClip, compassOffClip, controlsClip, creditsClip, fullCreditsClip, closeClip, moveUpClip, moveDownClip, enterClip, pauseMusicClip, errorClip;
    public AudioClip mobileControlsClip, computerControlsClip;
    public AudioSource pauseMenuAnnouncerSource, pauseMenuTransitionSource;
    public AudioClip[] betweenOptionsClips;
    public Controls controls;
    public AudioMixer audioMixer;
    float ambienceFader;
    public float cutoffTimer;
    int betweenOptionsInt;
    public Compass compassScript;
    public SaveAndLoad saveAndLoad;

    void Update(){
        paused = controls.paused;

        if (hasPlayedIntro && paused){
            GetMenuPosition();
            GetControls();
        }

        if (paused){
            if (cutoffTimer < 1){
                cutoffTimer += Time.deltaTime;
                Debug.Log("pausedContinue");
            }
        } else {
            if (cutoffTimer > 0){
                cutoffTimer -= Time.deltaTime;
                Debug.Log("finishedPaused");
            }
        }

        if (!hasPaused && paused){
            hasPaused = true;
            StartCoroutine(PauseScreen());
        }

        if (!paused && !controls.inCutscene){
            hasPaused = false;
            controls.canFocus = true;
            // REFACTOR - think this can just be done with canFocus.
        }
        audioMixer.SetFloat("PauseCutOff", Mathf.Lerp(22000, 200, cutoffTimer));
    }

    IEnumerator PauseScreen(){
        Debug.Log("startedPause");
        // pauseMenuAnnouncerSource.PlayOneShot(enterPauseClip);
        controls.canFocus = false;
        yield return new WaitForSeconds(0f);
        Debug.Log("afterPauseIntro");
        hasPlayedIntro = true;
        continueGame = true;
        yield return new WaitForSeconds(4f);
        if (!pauseMenuAnnouncerSource.isPlaying && continueGame)
        {
            pauseMenuAnnouncerSource.PlayOneShot(continueGameClip);
        }
    }

    void GetMenuPosition(){
        if (!pauseMenuAnnouncerSource.isPlaying){
            #region MOVE DOWN
            if (continueGame && moveDown && !hasMoved){
                hasMoved = true;
                continueGame = false;
                saveGame = false;
                compass = true;
                controlsBool = false;
                credits = false;
                close = false;
                StartCoroutine(CompassClip());
            }

            if (saveGame && moveDown && !hasMoved){
                hasMoved = true;
                continueGame = false;
                saveGame = false;
                compass = true;
                controlsBool = false;
                credits = false;
                close = false;
                StartCoroutine(CompassClip());
            }

            if (compass && moveDown && !hasMoved){
                hasMoved = true;
                continueGame = false;
                saveGame = false;
                compass = false;
                controlsBool = true;
                credits = false;
                close = false;
                StartCoroutine(ControlsClip());
            }

            if (controls && moveDown && !hasMoved){
                hasMoved = true;
                continueGame = false;
                saveGame = false;
                compass = false;
                controlsBool = false;
                credits = true;
                close = false;
                StartCoroutine(CreditsClip());
            }

            if (credits && moveDown && !hasMoved){
                hasMoved = true;
                continueGame = false;
                saveGame = false;
                compass = false;
                controlsBool = false;
                credits = false;
                close = true;
                StartCoroutine(CloseClip());
            }

            if (close && moveDown && !hasMoved){
                hasMoved = true;
                continueGame = true;
                saveGame = false;
                compass = false;
                controlsBool = false;
                credits = false;
                close = false;
                StartCoroutine(ContinueGameClip());
            }
            #endregion

            #region MOVE UP
            if (saveGame && moveUp && !hasMoved){
                hasMoved = true;
                continueGame = true;
                saveGame = false;
                compass = false;
                controlsBool = false;
                credits = false;
                close = false;
                StartCoroutine(ContinueGameClip());
            }

            if (compass && moveUp && !hasMoved){
                hasMoved = true;
                continueGame = true;
                saveGame = false;
                compass = false;
                controlsBool = false;
                credits = false;
                close = false;
                StartCoroutine(ContinueGameClip());
            }

            if (controls && moveUp && !hasMoved){
                hasMoved = true;
                continueGame = false;
                saveGame = false;
                compass = true;
                controlsBool = false;
                credits = false;
                close = false;
                StartCoroutine(CompassClip());
            }

            if (credits && moveUp && !hasMoved){
                hasMoved = true;
                continueGame = false;
                saveGame = false;
                compass = false;
                controlsBool = true;
                credits = false;
                close = false;
                StartCoroutine(ControlsClip());
            }

            if (close && moveUp && !hasMoved){
                hasMoved = true;
                continueGame = false;
                saveGame = false;
                compass = false;
                controlsBool = false;
                credits = true;
                close = false;
                StartCoroutine(CreditsClip());
            }

            if (continueGame && moveUp && !hasMoved){
                hasMoved = true;
                continueGame = false;
                saveGame = false;
                compass = false;
                controlsBool = false;
                credits = false;
                close = true;
                StartCoroutine(CloseClip());
            }
        }
        #endregion

        #region ENTER
        if (continueGame && enter && !hasMoved){
            hasMoved = true;
            pauseMenuAnnouncerSource.PlayOneShot(enterClip);
            StartCoroutine(ContinueGame());
            audioMixer.SetFloat("EnteredGameVol", 0);
        }

        if (saveGame && enter && !hasMoved){
            hasMoved = true;
            pauseMenuAnnouncerSource.PlayOneShot(enterClip);
            StartCoroutine(SaveGame());
            audioMixer.SetFloat("EnteredGameVol", 0);
        }

        if (compass && enter && !hasMoved){
            hasMoved = true;
            pauseMenuAnnouncerSource.PlayOneShot(enterClip);
            StartCoroutine(Compass());
        }

        if (controlsBool && enter && !hasMoved){
            hasMoved = true;
            pauseMenuAnnouncerSource.PlayOneShot(enterClip);
            StartCoroutine(Controls());
        }

        if (credits && enter && !hasMoved){
            hasMoved = true;
            pauseMenuAnnouncerSource.PlayOneShot(enterClip);
            StartCoroutine(Credits());
        }

        if (close && enter && !hasMoved){
            hasMoved = true;
            pauseMenuAnnouncerSource.PlayOneShot(enterClip);
            StartCoroutine(CloseGame());
        }
        #endregion
        
    }

    IEnumerator RandomNumberGen(){
        yield return new WaitForSeconds(0f);
        int randomInt = Random.Range(0, betweenOptionsClips.Length);
        if (randomInt == betweenOptionsInt){
            StartCoroutine(RandomNumberGen());
        } else {
            betweenOptionsInt = randomInt;
        }
    }

    IEnumerator ContinueGameClip(){
        StartCoroutine(RandomNumberGen());
        yield return new WaitForSeconds(0.1f);
        pauseMenuAnnouncerSource.PlayOneShot(betweenOptionsClips[betweenOptionsInt]);
        yield return new WaitForSeconds(betweenOptionsClips[betweenOptionsInt].length);
        pauseMenuAnnouncerSource.PlayOneShot(continueGameClip);
    }

    
    IEnumerator SaveGameClip(){
        StartCoroutine(RandomNumberGen());
        yield return new WaitForSeconds(0.1f);
        pauseMenuAnnouncerSource.PlayOneShot(betweenOptionsClips[betweenOptionsInt]);
        yield return new WaitForSeconds(betweenOptionsClips[betweenOptionsInt].length);
        pauseMenuAnnouncerSource.PlayOneShot(saveGameClip);
    }
    

    IEnumerator CompassClip(){
        StartCoroutine(RandomNumberGen());
        yield return new WaitForSeconds(0.1f);
        pauseMenuAnnouncerSource.PlayOneShot(betweenOptionsClips[betweenOptionsInt]);
        yield return new WaitForSeconds(betweenOptionsClips[betweenOptionsInt].length);
        pauseMenuAnnouncerSource.PlayOneShot(compassClip);
    }

    IEnumerator ControlsClip(){
        StartCoroutine(RandomNumberGen());
        yield return new WaitForSeconds(0.1f);
        pauseMenuAnnouncerSource.PlayOneShot(betweenOptionsClips[betweenOptionsInt]);
        yield return new WaitForSeconds(betweenOptionsClips[betweenOptionsInt].length);
        pauseMenuAnnouncerSource.PlayOneShot(controlsClip);
    }

    IEnumerator CreditsClip(){
        StartCoroutine(RandomNumberGen());
        yield return new WaitForSeconds(0.1f);
        pauseMenuAnnouncerSource.PlayOneShot(betweenOptionsClips[betweenOptionsInt]);
        yield return new WaitForSeconds(betweenOptionsClips[betweenOptionsInt].length);
        pauseMenuAnnouncerSource.PlayOneShot(creditsClip);
    }

    IEnumerator CloseClip(){
        StartCoroutine(RandomNumberGen());
        yield return new WaitForSeconds(0.1f);
        pauseMenuAnnouncerSource.PlayOneShot(betweenOptionsClips[betweenOptionsInt]);
        yield return new WaitForSeconds(betweenOptionsClips[betweenOptionsInt].length);
        pauseMenuAnnouncerSource.PlayOneShot(closeClip);
    }

    IEnumerator ContinueGame(){
        if (controls.mobile){
            Handheld.Vibrate();
        }
        yield return new WaitForSeconds(0.1f);
        controls.paused = false;
        controls.canFocus = true;
        hasContinued = true;
        controls.pauseTimer = 0;
        hasPlayedIntro = false;
        Debug.Log("off pause");
    }

    IEnumerator SaveGame(){
        if (controls.mobile){
            Handheld.Vibrate();
        }
        fadeAmbience = true;
        pauseMenuAnnouncerSource.PlayOneShot(gameSavedClip);
        saveAndLoad.SaveGame();
        yield return new WaitForSeconds(gameSavedClip.length);
    }

    IEnumerator Compass(){
        if (compassActivated){
            compassActivated = false;
            pauseMenuAnnouncerSource.PlayOneShot(compassOffClip, 0.5f);
            Debug.Log("compassOff");
            compassScript.compassTurnedOn = false;
        } else if (!compassActivated) {
            compassActivated = true;
            pauseMenuAnnouncerSource.PlayOneShot(compassOnClip, 0.5f);
            Debug.Log("compassOn");
            compassScript.compassTurnedOn = true;
        }
        yield return new WaitForSeconds(compassOnClip.length);
        continueGame = true;
        saveGame = false;
        compass = false;
        controlsBool = false;
        credits = false;
        close = false;
    }

    IEnumerator Controls(){
        yield return new WaitForSeconds(controlsClip.length);
        if (controls.computer){
            pauseMenuAnnouncerSource.PlayOneShot(computerControlsClip);
            yield return new WaitForSeconds(computerControlsClip.length);
            hasMoved = false;
            continueGame = true;
            saveGame = false;
            compass = false;
            controlsBool = false;
            credits = false;
            close = false;
        }
        if (controls.mobile){
            pauseMenuAnnouncerSource.PlayOneShot(controlsClip);
            yield return new WaitForSeconds(controlsClip.length);
            hasMoved = false;
            continueGame = true;
            saveGame = false;
            compass = false;
            controlsBool = false;
            credits = false;
            close = false;
        }
    }

    IEnumerator Credits(){
        yield return new WaitForSeconds(creditsClip.length);
        pauseMenuAnnouncerSource.PlayOneShot(fullCreditsClip, 0.7f);
        yield return new WaitForSeconds(fullCreditsClip.length);
        hasMoved = false;
        continueGame = true;
        saveGame = false;
        compass = false;
        controlsBool = false;
        credits = false;
        close = false;
    }

    IEnumerator CloseGame(){
        yield return new WaitForSeconds(closeClip.length);
        Application.Quit();
    }

    void GetControls(){
        // REFACTOR - this should could from controls script
        if ((controls.swipeDown || Input.GetKeyDown(KeyCode.UpArrow)) && !hasMoved && !pauseMenuAnnouncerSource.isPlaying){
            Debug.Log("uparrow");
            moveUp = true;
            StartCoroutine(StopDoublePress());
            pauseMenuTransitionSource.PlayOneShot(moveUpClip);
        } else {
            moveUp = false;
        }

        if ((controls.swipeUp || Input.GetKeyDown(KeyCode.DownArrow)) && !hasMoved && !pauseMenuAnnouncerSource.isPlaying){
            Debug.Log("downarrow");
            moveDown = true;
            StartCoroutine(StopDoublePress());
            pauseMenuTransitionSource.PlayOneShot(moveDownClip);
        } else {
            moveDown = false;
        }

        if ((controls.doubleTap || Input.GetKeyDown(KeyCode.Space)) && !hasMoved && !pauseMenuAnnouncerSource.isPlaying){
            enter = true;
            StartCoroutine(StopDoublePress());
        } else {
            enter = false;
        }
    }

    IEnumerator StopDoublePress(){
        yield return new WaitForSeconds(0.1f);
        hasMoved = false;
        Debug.Log("StopDoublePress");
    }
    
    void AdjustVolumes(){
        if (fadeAmbience){
            ambienceFader += Time.deltaTime;
            audioMixer.SetFloat("PauseMenuVol", Mathf.Lerp(0, -80, ambienceFader));
        }
    }
}
