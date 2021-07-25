using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class TempIntroTutorial : MonoBehaviour
{

    public AudioMixerSnapshot Standard;
    public AudioMixerSnapshot Lowpassed;
    public AudioSource audioSource;
    public AudioClip Intro;
    public AudioClip Focused;
    public float waitTime;
    public AudioMixer mixer;
    public string Introduction;
    public float initialVolume;
    public float newVolume;
    public float transitionSpeed;
    public float timeToReachStart;
    public bool hasFocused = false;

    void Start()
    {
        Standard.TransitionTo(timeToReachStart);
        audioSource.clip = Intro;
        audioSource.Play();
    }

    void Update()
    {
        if (!hasFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                audioSource.Stop();
                audioSource.clip = Focused;
                StartCoroutine("WaitBeforeClip");
                hasFocused = true;
            }
        }
    }

    IEnumerator WaitBeforeClip()
    {
        yield return new WaitForSeconds(waitTime);
        audioSource.Play();
    }
}
