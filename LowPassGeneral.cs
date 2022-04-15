using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LowPassGeneral : MonoBehaviour
{
    string lowpassCutOffString;
    string highpassCutOffString;
    float lowpassCutOffFloat;
    float highpassCutOffFloat;
    public float maxHighPass, minLowPass, lowpassMultiplier = 200000, highpassReduction = 200;
    AudioMixer audioMixer;
    public Transform player;
    bool hit;
    float increaseTimer, decreaseTimer;

    void Start(){
        lowpassCutOffString = GetComponent<AudioSource>().outputAudioMixerGroup.name + "_LowpassCutOff";
        highpassCutOffString = GetComponent<AudioSource>().outputAudioMixerGroup.name + "_HighpassCutOff";
        audioMixer = GetComponent<AudioSource>().outputAudioMixerGroup.audioMixer;
    }

    void Update(){
        hit = GetComponent<ChangeAfterFocus>().hit;

        float distanceFromPlayer = Vector3.Distance(transform.position, player.position);

        lowpassCutOffFloat = 1 / distanceFromPlayer * lowpassMultiplier;
        highpassCutOffFloat = distanceFromPlayer - highpassReduction;

        if (!hit){
            if (lowpassCutOffFloat > minLowPass){
                audioMixer.SetFloat(lowpassCutOffString, Mathf.Lerp(22000, lowpassCutOffFloat, decreaseTimer));
            } else {
                audioMixer.SetFloat(lowpassCutOffString, Mathf.Lerp(22000, lowpassCutOffFloat, decreaseTimer));
            }

            if (highpassCutOffFloat < maxHighPass) {
                audioMixer.SetFloat(highpassCutOffString, Mathf.Lerp(0, highpassCutOffFloat, decreaseTimer));
            }

            decreaseTimer += Time.deltaTime;
            increaseTimer = 0;
        } else {
            increaseTimer += Time.deltaTime;
            decreaseTimer = 0;
            audioMixer.SetFloat(lowpassCutOffString, Mathf.Lerp(lowpassCutOffFloat, 22000, increaseTimer));
            audioMixer.SetFloat(highpassCutOffString, Mathf.Lerp(highpassCutOffFloat, 0, increaseTimer));
        }
    }
}
