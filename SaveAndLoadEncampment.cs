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
    [SerializeField] PlaySound leaveBunkerPlaySound;
    [SerializeField] GameObject bunkerObject;

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
        bunkerObject.SetActive(false);
        leaveBunkerPlaySound.hasTriggered = true;
        if (secretSequence.finished == 0 && auditoryZoomSequence.finished == 0){
            secretSequence.active = 1;
            StartCoroutine(secretSequence.Sequence());
            return;
        }
        if (mechanicSequence.finished == 1  && encampmentCombatSequence.finished == 1 && aroundTableSequence.finished == 1 && auditoryZoomSequence.finished == 0){
            auditoryZoomSequence.CheckIfShouldStart();
            return;
        }
        if (auditoryZoomSequence.finished == 1){
            quadbikeFinishedSequence.active = 1;
            StartCoroutine(quadbikeFinishedSequence.Sequence());
        }
    }

    void LoadOther(){
        if (finnInBunkerSequence.finished == 1){
            removeReverbTrigger.TriggerRemotely();
            changeFootsteps.ChangePlayerFootsteps();
        }
        if (auditoryZoomSequence.finished == 1){
            quadbikeFinishedSequence.active = 1;
            StartCoroutine(quadbikeFinishedSequence.Sequence());
        }
    }

    void LoadDarkVsLight(){darkVsLight.playerDarkness = PlayerPrefs.GetInt("darkVsLight");}

    void LoadPlayerPosition(){ playerTransform.position = new Vector3(PlayerPrefs.GetFloat("playerX"), 0.3f, PlayerPrefs.GetFloat("playerZ")); }

    void LoadEncampmentSequences(){
        finnInBunkerSequence.finished = PlayerPrefs.GetInt("finnInBunkerSequence");
        encampmentCombatSequence.finished = PlayerPrefs.GetInt("encampmentCombatSequence");
        aroundTableSequence.finished = PlayerPrefs.GetInt("aroundTableSequence");
        auditoryZoomSequence.finished = PlayerPrefs.GetInt("auditoryZoomSequence");
        mechanicSequence.finished = PlayerPrefs.GetInt("mechanicSequence");
        secretSequence.finished = PlayerPrefs.GetInt("secretSequence");
        if (finnInBunkerSequence.finished == 1) LoadAfterFinnInBunkerSequence();
    }

    void LoadEncampment(){
        LoadEncampmentSequences();
        LoadPlayerPosition();
        LoadDarkVsLight();
        LoadOther();
    }

    void Start(){ 
        PlayerPrefs.SetInt("inMainMenu", 0);
        LoadEncampment(); 
    }
}
