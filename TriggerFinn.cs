using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TriggerFinn : MonoBehaviour
{
    bool hasPlayed, faded;
    public AudioClip audioClip;
    public AudioSource audioSource;
    public AudioMixerSnapshot mixerSnapshot;
    public GameObject Finn, FinnIntro, IntroSequence;
    public float fadeTimer = 1;
    public BunkerIntroSequence bunkerIntroSequence;
    public AudioSource finnAudioSource;
    public AudioMixer audioMixer;
    public string volume;
    public bool reduce;
    public float volumeTimer;

    public void Start()
    {
        volume = finnAudioSource.outputAudioMixerGroup.name + "Vol";
        audioMixer = finnAudioSource.outputAudioMixerGroup.audioMixer;
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

    void OnTriggerEnter(Collider other)
    {
        audioSource = GetComponent<AudioSource>();

        if (!hasPlayed)
        {
            audioSource.PlayOneShot(audioClip);
            StartCoroutine(FinnWaiter());
            hasPlayed = true;
            Finn.GetComponent<LowPassAutomator>().doorOpen = true;
            mixerSnapshot.TransitionTo(2f);
            reduce = true;
        }
    }

    IEnumerator FinnWaiter()
    {
        yield return new WaitForSeconds(0.2f);
        StopCoroutine(bunkerIntroSequence.FinnDialogueIntro());
        bunkerIntroSequence.dialogueCounter = 1;
        yield return new WaitForSeconds(2f);
        StartCoroutine(bunkerIntroSequence.FinnDialogue());
        
    }
}
