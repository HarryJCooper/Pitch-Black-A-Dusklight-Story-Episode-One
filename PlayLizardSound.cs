using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayLizardSound : MonoBehaviour
{
    public float Volume;
    public bool alreadyPlayed = false;
    public AudioSource _as;
    public AudioClip[] audioClipArray;
    public GameObject Lizard;
    
    void Awake()
    {
        _as = GetComponent<AudioSource>();
        Lizard = GameObject.Find("Lizard");
    }

    void Start()
    {
        _as.clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
    }

    void OnTriggerEnter()
    {
        if (!alreadyPlayed)
        {
            _as.PlayOneShot(audioClipArray[Random.Range(0, audioClipArray.Length)], Volume);
            StartCoroutine(waiter());
            IEnumerator waiter()
            {
                int wait_time = 30;
                yield return new WaitForSeconds(wait_time);
            }
            alreadyPlayed = true;
        }
    }
    void Update()
    {
       if (alreadyPlayed)
        {
            StartCoroutine(waiter2());
            IEnumerator waiter2()
            {
                int wait_time = 30;
                yield return new WaitForSeconds(wait_time);
                alreadyPlayed = false;
                _as.clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
            }
            
        }
    }
}
