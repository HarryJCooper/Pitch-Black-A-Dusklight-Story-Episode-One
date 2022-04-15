using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public bool paused, increaseCutOffAtStart = true;
    bool reduceLowpass, increaseLowpass, reduceMusicVol, reduceRoomVol, reduceExposedParameter, increaseRoomVol, increaseLowpassForPause, decreaseLowpassForPause;
    float lowpass = 22000, pauseLowpass = 22000, musicVol, exposedParameterVol, reverbVol, roomVol, roomMaxVol, changeLowpassFloat;
    public AudioSource musicSource;
    string exposedParameter;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioClip combatMusicClip, stealthMusicClip, menuMusicClip;
    [SerializeField] DarkVsLight darkVsLight;

    void IncreaseLowpassForPause(){
        Debug.Log("inc pause");
        if (pauseLowpass < 22000){
            pauseLowpass += 100;
            audioMixer.SetFloat("Pause_CutOff", pauseLowpass); 
            return;
        } 
        increaseLowpassForPause = false;
    }

    void DecreaseLowpassForPause(){
        if (pauseLowpass > 200){ 
            pauseLowpass -= 100;
            audioMixer.SetFloat("Pause_CutOff", pauseLowpass); 
            return;
        }
        decreaseLowpassForPause = false;
    }

    void ModifyMasterCutOff(){
        if (reduceLowpass && lowpass > 0){ 
            lowpass -= changeLowpassFloat; 
            audioMixer.SetFloat("Master_CutOff", lowpass);
            return;
        }
        if (increaseLowpass && lowpass < 22000){
            lowpass += changeLowpassFloat;
            audioMixer.SetFloat("Master_CutOff", lowpass);
        } 
    }

    void ModifyRoomVol(){
        if (reduceRoomVol && roomVol > -10000f){ 
            roomVol -= 100;
            audioMixer.SetFloat("Room_Vol", roomVol);
            return;}
        if (increaseRoomVol && roomVol < roomMaxVol){
            audioMixer.SetFloat("Room_Vol", roomVol);
            roomVol += 100;
        } 
    }

    void FixedUpdate(){
        if (increaseLowpassForPause) IncreaseLowpassForPause();
        if (decreaseLowpassForPause) DecreaseLowpassForPause();
        ModifyMasterCutOff();
        ModifyRoomVol();
        if (reduceMusicVol && musicVol > -80f){
            musicVol -= 1;
            audioMixer.SetFloat("Music_Vol", musicVol);
        } 
        if (reduceExposedParameter && exposedParameterVol > -80f){
            audioMixer.SetFloat(exposedParameter, exposedParameterVol);
            exposedParameterVol -= 1;
        }
    }

    public void SetVolumeToNothing(string m_exposedParameter){ audioMixer.SetFloat(m_exposedParameter, -80f);}
    public void SetVolumeToZero(string m_exposedParameter){ audioMixer.SetFloat(m_exposedParameter, 0f);}

    public void SetCutOffToZero(){
        lowpass = 0;
        audioMixer.SetFloat("Master_CutOff", lowpass);
    }

    public IEnumerator IncreasePauseCutOff(){
        increaseLowpassForPause = true;
        yield return new WaitForSeconds(5f);
        increaseLowpassForPause = false;
    }

    public IEnumerator DecreasePauseCutOff(){
        decreaseLowpassForPause = true;
        yield return new WaitForSeconds(5f);
        decreaseLowpassForPause = false;
    }

    public IEnumerator ReduceMasterCutOff(float m_time){
        changeLowpassFloat = 22000 / (m_time * 60);
        increaseLowpass = false;
        reduceLowpass = true;
        yield return new WaitForSeconds(m_time + 2f);
        reduceLowpass = false;
    }

    public IEnumerator IncreaseMasterCutOff(float m_time){
        Debug.Log("inc master cutoff");
        changeLowpassFloat = 22000 / (m_time * 60);
        reduceLowpass = false;
        increaseLowpass = true;
        yield return new WaitForSeconds(m_time + 2f);
        increaseLowpass = false;
    }

    public void PlayMusic(string m_musicType){
        musicSource.loop = true;
        if (m_musicType == "combat"){
            musicSource.clip = combatMusicClip;
            musicSource.volume = 0.2f;
        } else if (m_musicType == "stealth"){
            musicSource.clip = stealthMusicClip;
            musicSource.volume = 0.05f;
        } else if (m_musicType == "menu"){
            musicSource.clip = menuMusicClip;
        }
        musicSource.Play();
    }

    public IEnumerator FadeMusic(){
        reduceMusicVol = true;
        yield return new WaitForSeconds(3f);
        musicSource.Stop();
        reduceMusicVol = false;
        musicVol = 0;
        audioMixer.SetFloat("Music_Vol", musicVol);
    }

    public IEnumerator FadeOutReverbAndReflections(){
        reduceRoomVol = true;
        yield return new WaitForSeconds(3f);
        reduceRoomVol = false;
    }

    public IEnumerator FadeInReverbAndReflections(float m_roomMaxVol){
        audioMixer.GetFloat("Room_Vol", out roomVol);
        roomMaxVol = m_roomMaxVol;
        increaseRoomVol = true;
        yield return new WaitForSeconds(5f);
        increaseRoomVol = false;
    }

    public IEnumerator ReduceExposedParameter(string m_exposedParameter, float m_time){
        exposedParameter = m_exposedParameter;
        audioMixer.GetFloat(exposedParameter, out exposedParameterVol);
        reduceExposedParameter = true;
        yield return new WaitForSeconds(m_time);
        reduceExposedParameter = false;
    }

    public IEnumerator SetDarkVsLightAmbience(){
        yield return new WaitForSeconds(4f);
        float playerDarkness = -60 + (darkVsLight.playerDarkness * 10);
        audioMixer.SetFloat("DarkVsLightAmbience_Vol", playerDarkness);
    }

    IEnumerator WaitThenIncrease(){
        SetCutOffToZero();
        if (!increaseCutOffAtStart) yield break;
        yield return new WaitForSeconds(5f);
        StartCoroutine(IncreaseMasterCutOff(20f)); // TODO - deal with this bullsheesh
    }

    void Start(){
        StartCoroutine(SetDarkVsLightAmbience());
        StartCoroutine(WaitThenIncrease());
    }
}
