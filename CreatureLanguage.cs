using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureLanguage : MonoBehaviour
{

    public AudioSource creatureLanguage;
    public AudioClip[] creatureLanguageArray;
    public AudioClip[] creatureLanguageArray1;
    public AudioClip[] creatureLanguageArray2;
    public AudioClip[] creatureLanguageArray3;
    public AudioClip[] creatureLanguageArray4;

    // Start is called before the first frame update
    void Start()
    {
        creatureLanguage = GetComponent<AudioSource>();
    }

    void OnTriggerEnter()
    {
        Invoke("PlayAudio", 0.0f);
        Invoke("PlayAudio1", 0.8f);
        Invoke("PlayAudio2", 1.6f);
        Invoke("PlayAudio3", 2.4f);
        Invoke("PlayAudio4", 3.2f);
    }

    void PlayAudio()
    {
        creatureLanguage.clip = creatureLanguageArray[Random.Range(0, creatureLanguageArray.Length)];
        creatureLanguage.Play();
    }

    void PlayAudio1()
    {
        creatureLanguage.clip = creatureLanguageArray1[Random.Range(0, creatureLanguageArray1.Length)];
        creatureLanguage.Play();
    }

    void PlayAudio2()
    {
        creatureLanguage.clip = creatureLanguageArray2[Random.Range(0, creatureLanguageArray2.Length)];
        creatureLanguage.Play();
    }

    void PlayAudio3()
    {
        creatureLanguage.clip = creatureLanguageArray3[Random.Range(0, creatureLanguageArray3.Length)];
        creatureLanguage.Play();
    }

    void PlayAudio4()
    {
        creatureLanguage.clip = creatureLanguageArray4[Random.Range(0, creatureLanguageArray4.Length)];
        creatureLanguage.Play();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
