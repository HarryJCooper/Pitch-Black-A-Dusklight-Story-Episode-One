using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OccludedObject : MonoBehaviour
{
    public float volumeReduction, frequencyReduction, initialVolume, reducedVolume, initialFrequency = 22000, reducedFrequency, volumeReducer = 1, volumeIncreaser = 0;
    public bool occluded, hasSetVolume;
    public AudioSource audioSource;
    AudioMixerGroup audioMixerGroup;
    AudioMixer audioMixer;
    string occlusionVolumeString, occlusionFrequencyString;
    
    void Awake(){
        audioSource = GetComponent<AudioSource>();
        audioMixerGroup = audioSource.outputAudioMixerGroup;
        audioMixer = audioSource.outputAudioMixerGroup.audioMixer;
        occlusionVolumeString = audioMixerGroup.name + "_OcclusionVol";
        occlusionFrequencyString = audioMixerGroup.name + "_OcclusionCutOff";
    }

    void Update(){
        if (occluded){
            reducedVolume = initialVolume - volumeReduction;
            reducedFrequency = initialFrequency - frequencyReduction;
            volumeIncreaser = 0;
            if (volumeReducer >= 0) volumeReducer -= Time.deltaTime;
            audioMixer.SetFloat(occlusionVolumeString, Mathf.Lerp(reducedVolume, initialVolume, volumeReducer));
            audioMixer.SetFloat(occlusionFrequencyString, Mathf.Lerp(reducedFrequency, initialFrequency, volumeReducer));
            return;
        }
        volumeReduction = 0;
        frequencyReduction = 0;
        volumeIncreaser += Time.deltaTime;
        audioMixer.SetFloat(occlusionFrequencyString, Mathf.Lerp(reducedFrequency, initialFrequency, volumeIncreaser));
        audioMixer.SetFloat(occlusionVolumeString, Mathf.Lerp(reducedVolume, initialVolume, volumeIncreaser));
        volumeReducer = 1;  
    }
}
