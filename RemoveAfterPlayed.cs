using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RemoveAfterPlayed : MonoBehaviour
{
    public AudioSource ThisObject;
    public GameObject Ambience1;
    public GameObject Ambience2;
    public GameObject Ambience3;
    public GameObject Ambience4;
    public bool hasPlayed;
    public float timeToWait1;
    public float timeToWait2;
    public float timeToWait3;
    public float timeToWait4;

    // Start is called before the first frame update
    void Start()
    {
        ThisObject = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ThisObject.isPlaying)
        {
            hasPlayed = true;
        }
        if (!ThisObject.isPlaying)
        {
            if (hasPlayed)
            {
                StartCoroutine(Wait());
                StartCoroutine(Wait2());
                StartCoroutine(Wait3());
                StartCoroutine(Wait4());
            }
        }

    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(timeToWait1);
        Ambience1.SetActive(false);
    }
    IEnumerator Wait2()
    {
        yield return new WaitForSeconds(timeToWait2);
        Ambience2.SetActive(false);
    }
    IEnumerator Wait3()
    {
        yield return new WaitForSeconds(timeToWait3);
        Ambience3.SetActive(false);
    }
    IEnumerator Wait4()
    {
        yield return new WaitForSeconds(timeToWait4);
        Ambience4.SetActive(false);
    }
}