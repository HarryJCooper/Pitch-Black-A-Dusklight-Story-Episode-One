using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInstantiate : MonoBehaviour

{
    public AudioSource _as;

    public bool isPlaying;
    public GameObject[] ToBeMadeArray;
    public GameObject ToBeDestroyed;

    void OnTriggerEnter()
    {
        Instantiate(ToBeMadeArray[Random.Range(0, ToBeMadeArray.Length)]);
        _as.Play();
        _as.volume = 1;
        isPlaying = true;
    }

    void OnTriggerExit()
    {
        Destroy(ToBeDestroyed);
    }

}