using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Trigger_Cutscene : MonoBehaviour
{
    public AudioSource _as;
    public AudioClip[] audioClipArray;
    public bool isPlaying;
    public GameObject ToBeDestroyed;
    public AudioMixerSnapshot Standard;
    public AudioMixerSnapshot Lowpassed;
    public float timeToReach;
    public float timeToWait;
    public float timeToReturn;

    public PBFootstepSystem pBFootstepSystem;

    // Start is called before the first frame update
    void Start()
    {
        _as.clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
    }

    void OnTriggerEnter()
    {
        New();
        _as.Play();
        _as.volume = 1;
        isPlaying = true;
        StartCoroutine(Wait());
        pBFootstepSystem.canMove = false;
    }

    void New()
    {
        Lowpassed.TransitionTo(timeToReach);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(timeToWait + timeToReach);
        Standard.TransitionTo(timeToReturn);
        if (ToBeDestroyed)
        {
            pBFootstepSystem.canMove = true;
            ToBeDestroyed.SetActive(false);
        }
        
    }
}
