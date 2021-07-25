using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Zoom_Simple : MonoBehaviour
{

    public AudioMixerSnapshot Standard;
    public AudioMixerSnapshot Zoomed;
    public float timeToReach;
    public float timeToReturn;
    public float timeToWait;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Zoomed.TransitionTo(timeToReach);
            StartCoroutine(Wait());
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Standard.TransitionTo(timeToReturn);
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(timeToWait);
        Standard.TransitionTo(timeToReturn);
    }
}
