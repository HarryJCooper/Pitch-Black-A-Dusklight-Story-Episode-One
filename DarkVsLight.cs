using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class DarkVsLight : MonoBehaviour
{
    public int playerDarkness;
    [SerializeField] AudioMixer audioMixer;

    public void SetTrappedWorkerVol(){
        float currentTrappedWorkerVol = 0;
        audioMixer.SetFloat("TrappedWorker_LightVsDarkVol", currentTrappedWorkerVol - playerDarkness * 5);
    }
}
