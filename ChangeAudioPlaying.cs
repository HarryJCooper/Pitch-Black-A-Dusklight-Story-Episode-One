using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAudioPlaying : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip changeToThis;
    public float waitTime;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        audioSource.loop = false;
        StartCoroutine(PlayAfterFinished());
    }

    IEnumerator PlayAfterFinished()
    {
        yield return new WaitForSeconds(waitTime);
        audioSource.loop = true;
        audioSource.clip = changeToThis;
        audioSource.Play();
    }
}
