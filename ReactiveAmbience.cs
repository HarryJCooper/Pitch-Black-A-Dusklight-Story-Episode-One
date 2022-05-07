using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveAmbience : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public float distanceFromPlayer, speedMultiplier;
    public Transform playerTransform;
    public bool beenTriggered, chosenRotation;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            chosenRotation = true;
            beenTriggered = true;
            audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
        }
    }

    void Update()
    {


        distanceFromPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceFromPlayer < 50)
        {
            if (beenTriggered)
            {
                transform.position += Vector3.forward * Time.deltaTime * speedMultiplier;
            }

            if (chosenRotation)
            {
                transform.Rotate(0, Random.Range(0, 360), 0);
                chosenRotation = false;
            }
        }

        if (distanceFromPlayer > 50)
        {
            beenTriggered = false;
            chosenRotation = false;
        }

    }
}
