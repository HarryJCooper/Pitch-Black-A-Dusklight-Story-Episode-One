

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class IncreaseSourceVolume : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioMixer audioMixer;
    public string inViewName;
    public bool inView, directBehind, hasIncreased;
    public float initialVolume = -5, increasedVolume = 0, reducedVolume = -10;
    public float increaseTimer, reduceTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioMixer = audioSource.outputAudioMixerGroup.audioMixer;
        inViewName = audioSource.outputAudioMixerGroup.name + "_InViewVol";
    }

    private void Update()
    {
        if (inView)
        {
            reduceTimer = 0;
            increaseTimer += Time.deltaTime * 2;
            audioMixer.SetFloat(inViewName, Mathf.Lerp(initialVolume, increasedVolume, increaseTimer));
        }

        if (!inView && !directBehind)
        {
            reduceTimer = 0;
            increaseTimer = 0;
            audioMixer.SetFloat(inViewName, initialVolume);
        }

        if (directBehind)
        {
            increaseTimer = 0;
            reduceTimer += Time.deltaTime * 2;
            audioMixer.SetFloat(inViewName, Mathf.Lerp(initialVolume, reducedVolume, reduceTimer));
        }
    }
}
