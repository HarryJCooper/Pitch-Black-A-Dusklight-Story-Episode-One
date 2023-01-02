using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoadEncampment : MonoBehaviour
{
    [SerializeField] Transform playerTransform;
    [SerializeField] FinnInBunkerSequence finnInBunkerSequence;
    [SerializeField] EncampmentCombatSequence encampmentCombatSequence;
    [SerializeField] AroundTableSequence aroundTableSequence;
    [SerializeField] AuditoryZoomSequence auditoryZoomSequence;
    [SerializeField] MechanicSequence mechanicSequence;
    [SerializeField] SecretSequence secretSequence;
    [SerializeField] QuadbikeFinishedSequence quadbikeFinishedSequence;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] RemoveReverbTrigger removeReverbTrigger;
    [SerializeField] ChangeFootsteps changeFootsteps;
    [SerializeField] GameObject bunkerObject;
    [SerializeField] Controls controls;
    [SerializeField] PlaySound windSweetenerPlaySound;
    public bool loadStartOfEncampment;
    [SerializeField] PlayClipWithInterval megapedeOnePlayClipWithInterval, megapedeTwoPlayClipWithInterval, guitarPlayClipWithInterval;
    [SerializeField] AmbienceRepeater fountainRepeater;


    public void FinishedEncampment(){ PlayerPrefs.SetInt("bunkerAndEncampment", 1);}

    void SaveDarkVsLight(){PlayerPrefs.SetFloat("darkVsLight", darkVsLight.playerDarkness);}

    void SavePlayerPosition(){
        PlayerPrefs.SetFloat("playerX", playerTransform.position.x);
        PlayerPrefs.SetFloat("playerZ", playerTransform.position.z);
    }

    void SaveEncampmentSequences(){
        PlayerPrefs.SetInt("finnInBunkerSequence", finnInBunkerSequence.finished);
        PlayerPrefs.SetInt("encampmentCombatSequence", encampmentCombatSequence.finished);
        PlayerPrefs.SetInt("aroundTableSequence", aroundTableSequence.finished);
        PlayerPrefs.SetInt("auditoryZoomSequence", auditoryZoomSequence.finished);
        PlayerPrefs.SetInt("mechanicSequence", mechanicSequence.finished);
        PlayerPrefs.SetInt("secretSequence", secretSequence.finished);
    }

    public void SaveEncampment(){
        SaveEncampmentSequences();
        SavePlayerPosition();
        SaveDarkVsLight();
    }
    
    void LoadAfterFinnInBunkerSequence(){
        megapedeOnePlayClipWithInterval.turnedOn = true;
        StartCoroutine(megapedeOnePlayClipWithInterval.LoopClips());
        megapedeTwoPlayClipWithInterval.turnedOn = true;
        StartCoroutine(megapedeTwoPlayClipWithInterval.LoopClips());
        guitarPlayClipWithInterval.turnedOn = true;
        StartCoroutine(guitarPlayClipWithInterval.LoopClips());
        windSweetenerPlaySound.hasTriggered = true;
        fountainRepeater.turnedOn = true;
        StartCoroutine(fountainRepeater.ambienceCoroutine);
        controls.canZoom = false;
        bunkerObject.SetActive(false);
        if (secretSequence.finished == 0 && auditoryZoomSequence.finished == 0){
            secretSequence.active = 1;
            StartCoroutine(secretSequence.Sequence());
        }
        if (mechanicSequence.finished == 1 && encampmentCombatSequence.finished == 1 && aroundTableSequence.finished == 1 && auditoryZoomSequence.finished == 0){
            auditoryZoomSequence.CheckIfShouldStart();
            return;
        }
        if (auditoryZoomSequence.finished == 1){
            controls.canZoom = true;
            quadbikeFinishedSequence.active = 1;
            StartCoroutine(quadbikeFinishedSequence.SequenceLoop());
        }
    }

    void LoadOther(){
        if (finnInBunkerSequence.finished == 1){
            removeReverbTrigger.TriggerRemotely();
            changeFootsteps.ChangePlayerFootsteps();
        }
        if (aroundTableSequence.finished == 1){
            aroundTableSequence.audioSourceContainer.nightlanderOneSource.gameObject.SetActive(false);
            aroundTableSequence.audioSourceContainer.nightlanderTwoSource.gameObject.SetActive(false);
            aroundTableSequence.audioSourceContainer.nightlanderThreeSource.gameObject.SetActive(false);
            aroundTableSequence.audioSourceContainer.nightlanderFourSource.gameObject.SetActive(false);
        }
        if (auditoryZoomSequence.finished == 1){
            quadbikeFinishedSequence.active = 1;
            StartCoroutine(quadbikeFinishedSequence.Sequence());
        }
    }

    void LoadDarkVsLight(){darkVsLight.playerDarkness = PlayerPrefs.GetInt("darkVsLight");}

    void LoadPlayerPosition(){ 
        if (finnInBunkerSequence.finished == 1) playerTransform.position = new Vector3(PlayerPrefs.GetFloat("playerX"), 0.3f, PlayerPrefs.GetFloat("playerZ")); 
    }

    void LoadEncampmentSequences(){
        finnInBunkerSequence.finished = PlayerPrefs.GetInt("finnInBunkerSequence");
        encampmentCombatSequence.finished = PlayerPrefs.GetInt("encampmentCombatSequence");
        aroundTableSequence.finished = PlayerPrefs.GetInt("aroundTableSequence");
        auditoryZoomSequence.finished = PlayerPrefs.GetInt("auditoryZoomSequence");
        mechanicSequence.finished = PlayerPrefs.GetInt("mechanicSequence");
        secretSequence.finished = PlayerPrefs.GetInt("secretSequence");
        if (finnInBunkerSequence.finished == 1){
            LoadAfterFinnInBunkerSequence();
        } else {
            StartCoroutine(finnInBunkerSequence.Sequence());
        }
    }

    void LoadStartOfEncampment(){
        finnInBunkerSequence.finished = 0;
        encampmentCombatSequence.finished = 0;
        aroundTableSequence.finished = 0;
        auditoryZoomSequence.finished = 0;
        mechanicSequence.finished = 0;
        secretSequence.finished = 0;
        StartCoroutine(finnInBunkerSequence.Sequence());
    }

    void LoadEncampment(){
        if(loadStartOfEncampment && Application.isEditor){
            LoadStartOfEncampment();
        } else {
            LoadEncampmentSequences();
            LoadPlayerPosition(); 
            LoadDarkVsLight();
            LoadOther();
        }
    }

    void Start(){ 
        PlayerPrefs.SetInt("inMainMenu", 0);
        LoadEncampment();
        LoadingData.hasLoaded = true;
    }
}
