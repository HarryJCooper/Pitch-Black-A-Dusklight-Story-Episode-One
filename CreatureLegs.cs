using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureLegs : MonoBehaviour
{

    public AudioSource creatureLegs;
    public AudioClip[] creatureLegsArray;
    public AudioClip[] creatureLegsArray1;
    public AudioClip[] creatureLegsArray2;
    public AudioClip[] creatureLegsArray3;
    public AudioClip[] creatureLegsArray4;

    // Start is called before the first frame update
    void Start()
    {
        creatureLegs = GetComponent<AudioSource>();
    }

    void OnTriggerEnter()
    {
        Invoke("PlayAudio", 1.3f);
        Invoke("PlayAudio1", 2.6f);
        Invoke("PlayAudio2", 3.9f);
        Invoke("PlayAudio3", 5.2f);
        Invoke("PlayAudio4", 6.5f);
    }

    void PlayAudio()
    {
        creatureLegs.clip = creatureLegsArray[Random.Range(0, creatureLegsArray.Length)];
        creatureLegs.Play();
    }

    void PlayAudio1()
    {
        creatureLegs.clip = creatureLegsArray1[Random.Range(0, creatureLegsArray1.Length)];
        creatureLegs.Play();
    }

    void PlayAudio2()
    {
        creatureLegs.clip = creatureLegsArray2[Random.Range(0, creatureLegsArray2.Length)];
        creatureLegs.Play();
    }

    void PlayAudio3()
    {
        creatureLegs.clip = creatureLegsArray3[Random.Range(0, creatureLegsArray3.Length)];
        creatureLegs.Play();
    }

    void PlayAudio4()
    {
        creatureLegs.clip = creatureLegsArray4[Random.Range(0, creatureLegsArray4.Length)];
        creatureLegs.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
