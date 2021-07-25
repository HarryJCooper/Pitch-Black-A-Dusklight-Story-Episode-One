using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Introduction : MonoBehaviour
{
    public AudioMixerSnapshot Standard;
    public AudioMixerSnapshot Lowpassed;
    public AudioMixerSnapshot Standard_2;
    public AudioMixerSnapshot Lowpassed_2;
    public float timeToReach;
    public float timeToWait;
    public float timeToReturn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
        Lowpassed.TransitionTo(timeToReach);
        Lowpassed_2.TransitionTo(timeToReach);
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(timeToWait + timeToReach);
        Standard.TransitionTo(timeToReturn);
        Standard_2.TransitionTo(timeToReturn);
    }
}
