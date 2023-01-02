using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class IncreaseSourceVolume : MonoBehaviour
{
    AudioSource audioSource;
    AudioMixer audioMixer;
    string inViewName;
    float increaseTimer, reduceTimer;
    public bool inView, directBehind, hasIncreased;
    const float initialVolume = -5, increasedVolume = 0, reducedVolume = -10;

    void Awake(){
        audioSource = GetComponent<AudioSource>();
        audioMixer = audioSource.outputAudioMixerGroup.audioMixer;
        inViewName = audioSource.outputAudioMixerGroup.name + "_InViewVol";
    }

    void InViewChangeVol(){
        reduceTimer = 0;
        increaseTimer += Time.deltaTime * 2;
        audioMixer.SetFloat(inViewName, Mathf.Lerp(initialVolume, increasedVolume, increaseTimer));
    }

    void NotInViewChangeVol(){
        reduceTimer = 0;
        increaseTimer = 0;
        audioMixer.SetFloat(inViewName, initialVolume);
    }

    void DirectBehindChangeVol(){
        increaseTimer = 0;
        reduceTimer += Time.deltaTime * 2;
        audioMixer.SetFloat(inViewName, Mathf.Lerp(initialVolume, reducedVolume, reduceTimer));
    }

    void Update(){
        if (inView) InViewChangeVol();
        if (!inView && !directBehind) NotInViewChangeVol();
        if (directBehind) DirectBehindChangeVol();
    }
}
