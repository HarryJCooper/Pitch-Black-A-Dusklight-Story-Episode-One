using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public bool paused, increaseCutOffAtStart = true;
    bool reduceLowpass, increaseLowpass, reduceMusicVol, reduceRoomVol, reduceExposedParameterVol, increaseExposedParameterVol, 
    reduceExposedParameterCutOff, increaseExposedParameterCutOff, increaseRoomVol, increaseLowpassForPause, decreaseLowpassForPause;
    float lowpass = 22000, pauseLowpass = 22000, musicVol, exposedParameterVol, exposedParameterCutOff, reverbVol, roomVol, roomMaxVol, changeLowpassFloat;
    public AudioSource musicSource;
    string exposedParameter;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioClip combatMusicClip, stealthMusicClip, menuMusicClip;
    [SerializeField] DarkVsLight darkVsLight;

    public void SetParameter(string exposedParameter, float newValue){
        audioMixer.SetFloat(exposedParameter, newValue);
    }

    void IncreaseLowpassForPause(){
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
        if (reduceExposedParameterVol && exposedParameterVol > -80f){
            audioMixer.SetFloat(exposedParameter, exposedParameterVol);
            exposedParameterVol -= 1;
        }
        if (increaseExposedParameterVol && exposedParameterVol < 0){
            audioMixer.SetFloat(exposedParameter, exposedParameterVol);
            exposedParameterVol += 1;
        }
        if (reduceExposedParameterCutOff && exposedParameterCutOff > 0){
            audioMixer.SetFloat(exposedParameter, exposedParameterCutOff);
            exposedParameterCutOff -= 100;
        }
        if (increaseExposedParameterCutOff && exposedParameterCutOff < 22000){
            audioMixer.SetFloat(exposedParameter, exposedParameterCutOff);
            exposedParameterCutOff += 100;
        }
    }

    public void SetVolumeToNothing(string m_exposedParameter){ audioMixer.SetFloat(m_exposedParameter, -80f);}
    public void SetVolumeToZero(string m_exposedParameter){ audioMixer.SetFloat(m_exposedParameter, 5f);}

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
        changeLowpassFloat = 22000 / (m_time * 60);
        reduceLowpass = false;
        increaseLowpass = true;
        yield return new WaitForSeconds(m_time + 2f);
        increaseLowpass = false;
    }

    public void PlayMusic(string m_musicType, float volume){
        musicSource.loop = true;
        if (m_musicType == "combat"){
            musicSource.clip = combatMusicClip;
            musicSource.volume = volume;
        } else if (m_musicType == "stealth"){
            musicSource.clip = stealthMusicClip;
            musicSource.volume = volume;
        } else if (m_musicType == "menu"){
            musicSource.clip = menuMusicClip;
            musicSource.volume = volume;
        }
        musicSource.Play();
    }

    public void SetMusicToZero(){
        reduceMusicVol = false;
        audioMixer.SetFloat("Music_Vol", 0);
    }

    public IEnumerator FadeMusic(){
        reduceMusicVol = true;
        yield return new WaitForSeconds(3f);
        musicSource.Stop();
        reduceMusicVol = false;
        musicVol = 0;
        audioMixer.SetFloat("Music_Vol", 0);
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

    public IEnumerator ReduceExposedParameterVol(string m_exposedParameter, float m_time){
        exposedParameter = m_exposedParameter;
        audioMixer.GetFloat(exposedParameter, out exposedParameterVol);
        reduceExposedParameterVol = true;
        yield return new WaitForSeconds(m_time);
        reduceExposedParameterVol = false;
    }

    public IEnumerator IncreaseExposedParameterVol(string m_exposedParameter, float m_time){
        exposedParameter = m_exposedParameter;
        audioMixer.GetFloat(exposedParameter, out exposedParameterVol);
        increaseExposedParameterVol = true;
        yield return new WaitForSeconds(m_time);
        increaseExposedParameterVol = false;
    }

    public IEnumerator ReduceExposedParameterCutOff(string m_exposedParameter, float m_time){
        exposedParameter = m_exposedParameter;
        audioMixer.GetFloat(exposedParameter, out exposedParameterCutOff);
        reduceExposedParameterCutOff = true;
        yield return new WaitForSeconds(m_time);
        reduceExposedParameterCutOff = false;
    }

    public IEnumerator IncreaseExposedParameterCutOff(string m_exposedParameter, float m_time){
        exposedParameter = m_exposedParameter;
        audioMixer.GetFloat(exposedParameter, out exposedParameterCutOff);
        increaseExposedParameterCutOff = true;
        yield return new WaitForSeconds(m_time);
        increaseExposedParameterCutOff = false;
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
        StartCoroutine(IncreaseMasterCutOff(12f));
    }

    void Start(){
        StartCoroutine(SetDarkVsLightAmbience());
        StartCoroutine(WaitThenIncrease());
    }
}
