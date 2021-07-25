using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class RayCastToWalls : MonoBehaviour
{
    public float interactionRayLength;
    public float DistanceLeft;
    public float DistanceRight;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractRaycastRight();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            InteractRaycastLeft();
        }
        Update2();
    }

    void InteractRaycastRight()
    {
        Vector3 playerPosition = transform.position;
        Vector3 rightDirection = transform.right;
        Ray interactionRay = new Ray(playerPosition, rightDirection);
        RaycastHit interactionRayHit;

        Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength);
        bool hitFound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength);

        if (hitFound)
        {
            GameObject hitGameObject = interactionRayHit.transform.gameObject;
            string hitFeedback = hitGameObject.name;
            DistanceRight = interactionRayHit.distance;
            Debug.Log(DistanceRight);
        }
    }


    void InteractRaycastLeft()
    {
        Vector3 playerPosition = transform.position;
        Vector3 leftDirection = -transform.right;
        Ray interactionRay = new Ray(playerPosition, leftDirection);
        RaycastHit interactionRayHit;

        Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength);
        bool hitFound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength);

        if (hitFound)
        {
            GameObject hitGameObject = interactionRayHit.transform.gameObject;
            string hitFeedback = hitGameObject.name;
            DistanceLeft = interactionRayHit.distance;
            Debug.Log(DistanceLeft);
        }
    }

    public AudioSource RightSound;
    public AudioSource LeftSound;
    public float DistanceDivider;
    public float WaitTime;

    void Update2()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine("PlayRightCoroutine");
            StartCoroutine("WaiterRight");
            AnalyticsEvent.Custom("SonarRight", new Dictionary<string, object>
        {
            {"play_time", Time.timeSinceLevelLoad}
        });
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine("PlayLeftCoroutine");
            StartCoroutine("WaiterLeft");
            AnalyticsEvent.Custom("SonarLeft", new Dictionary<string, object>
        {
            {"play_time", Time.timeSinceLevelLoad}
        });
        }
    }
    IEnumerator PlayRightCoroutine()
    {
        RightSound.Play();
        yield return new WaitForSeconds(DistanceRight / DistanceDivider);
        RightSound.Stop();
    }

    IEnumerator PlayLeftCoroutine()

    {
        LeftSound.Play();
        yield return new WaitForSeconds(DistanceLeft / DistanceDivider);
        LeftSound.Stop();
    }
    IEnumerator WaiterRight()
    {
        yield return new WaitForSeconds(WaitTime);
    }
    IEnumerator WaiterLeft()
    {
        yield return new WaitForSeconds(WaitTime);
    }
}


