using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Zooms : MonoBehaviour
{
    public Transform other;
    public float dist;
    public float Range;
    private float inverseY;
    private float inverseX;
    public AudioSource zoomforward;
    public AudioSource zoombackward;
    public bool canMove;
    public float constantSpeed;
    public float interactionRayLength;
    public float DistanceForward;
    public float WallDist;
    public float volume;
    public AudioMixerSnapshot Zoomed;
    public AudioMixerSnapshot Standard;
    public float timeToReach;

    void Start()
    {
        //Set the speed of the GameObject
        float dist = Vector3.Distance(other.position, transform.position);
        canMove = true;
    }

    void Update()
    {   
        InteractRaycastForward();
        float dist = Vector3.Distance(other.position, transform.position);
        ZoomOut();

        if (transform.localPosition.z < Range)
        {
            if (DistanceForward > 1)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    canMove = false;
                    //Move the Rigidbody forwards constantly at speed you define (the blue arrow axis in Scene view)

                    transform.Translate(Vector3.forward * Time.deltaTime * constantSpeed);

                    zoomforward.volume = 1.0f;

                    if (!zoomforward.isPlaying)
                    {
                        zoomforward.Play();
                        zoombackward.Stop();
                    }

                    if (!canMove)
                    {
                        Zoomed.TransitionTo(timeToReach);
                    }
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    StartCoroutine("FadeSound");
                }

                if (transform.localPosition.z > Range)
                {
                    if (zoomforward.isPlaying)
                    {
                        StartCoroutine("FadeSound");
                    }
                }
            }
        }
    }
    void InteractRaycastForward()
    {
        Vector3 playerPosition = transform.position;
        Vector3 forwardDirection = transform.forward;
        Ray interactionRay = new Ray(playerPosition, forwardDirection);
        RaycastHit interactionRayHit;

        Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength);
        bool hitFound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength);

        if (hitFound)
        {
            GameObject hitGameObject = interactionRayHit.transform.gameObject;
            string hitFeedback = hitGameObject.name;
            DistanceForward = interactionRayHit.distance;
            Debug.Log(DistanceForward);
        }
    }

    void ZoomOut()
    {
        if (transform.localPosition.z > 0f)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                transform.Translate((Vector3.forward * Time.deltaTime) + (Vector3.back * 2 * Time.deltaTime * constantSpeed));
                zoombackward.volume = 1.0f;

                if (!zoombackward.isPlaying)
                {
                    zoombackward.Play();
                    zoomforward.Stop();
                }

                if (canMove)
                {
                    Standard.TransitionTo(timeToReach);
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                StartCoroutine("FadeSound");
            }
        }
        if (transform.localPosition.z < 1.0f)
        {
            StartCoroutine("FadeOut");
            canMove = true;
        }
    }

    public IEnumerator FadeSound()
    {
        while (zoomforward.volume > 0.03f)
        {
            zoomforward.volume -= Time.deltaTime / 1.0f;
            yield return null;
        }
        while (zoombackward.volume > 0.03f)
        {
            zoombackward.volume -= Time.deltaTime / 1.0f;
            yield return null;
        }
    }
    public IEnumerator FadeOut()
    {
        while (zoombackward.volume > 0.03f)
        {
            zoombackward.volume -= Time.deltaTime / 1.0f;
            yield return null;
        }
        zoombackward.volume = 0f;
        zoombackward.Stop();
    }
}

