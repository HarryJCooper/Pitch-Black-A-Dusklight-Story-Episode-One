using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SilenceMusic : MonoBehaviour
{
    public AudioMixer audioMixer;
    float newMusicVol = -80, timer = 0;
    public string musicName;
    bool isTriggered;

    void OnTriggerEnter()
    {
        isTriggered = true;
    }

    private void Update()
    {
        if (isTriggered)
        {
            timer += Time.deltaTime * 2;
            audioMixer.SetFloat(musicName, Mathf.Lerp(0, newMusicVol, timer));
        }
    }
}
