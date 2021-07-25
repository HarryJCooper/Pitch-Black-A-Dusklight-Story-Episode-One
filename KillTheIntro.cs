using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class KillTheIntro : MonoBehaviour
{
    public GameObject FinnIntro;
    public AudioSource audioSource;
    public AudioMixer audioMixer;
    public BunkerIntroSequence bunkerIntroSequence;
    public string volume;
    public bool reduce;
    public float volumeTimer;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        volume = audioSource.outputAudioMixerGroup.name + "Vol";
        audioMixer = audioSource.outputAudioMixerGroup.audioMixer;
        audioMixer.GetFloat(volume, out volumeTimer);
    }
    
    public void Update()
    {
        if (reduce && volumeTimer > -80)
        {
            audioMixer.SetFloat(volume, volumeTimer);
            volumeTimer -= Time.deltaTime * 40;
        }
    }
}
