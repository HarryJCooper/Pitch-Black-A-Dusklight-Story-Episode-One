using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastClosestWall : MonoBehaviour

{
    public float interactionRayLength;
    public float DistanceForward;


    // Update is called once per frame
    void Update()
    {
        InteractRaycastForward();
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
        }
    }
}
