using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWhenActivated : MonoBehaviour
{
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    IEnumerator FindTargetsWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.Play();
    }
}
