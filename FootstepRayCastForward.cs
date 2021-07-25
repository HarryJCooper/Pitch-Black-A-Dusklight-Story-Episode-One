using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FootstepRayCastForward : MonoBehaviour
{
    public float interactionRayLength;
    public float DistanceForward;
    public float VolumeAdjust;
    public AudioMixer FootstepMixer;
    public string Footstep_Slapback;
    public LayerMask walls;
    public float DistanceAway;
    public float introWaitTime;

    void Start()
    {
        FootstepMixer.SetFloat(Footstep_Slapback, -80);
        Wait();
    }
    void Update()
    {
        InteractRaycastForward();
        VolumeAdjust = DistanceForward * -1;
        if(VolumeAdjust > DistanceAway)
        {
            FootstepMixer.SetFloat(Footstep_Slapback, VolumeAdjust);
        }
        else
        {
            FootstepMixer.SetFloat(Footstep_Slapback, -80);
        }

    }

    void InteractRaycastForward()
    {
        Vector3 playerPosition = transform.position;
        Vector3 forwardDirection = transform.forward;
        Ray interactionRay = new Ray(playerPosition, forwardDirection);
        RaycastHit interactionRayHit;

        Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength, walls);
        bool hitFound = Physics.Raycast(interactionRay, out interactionRayHit, interactionRayLength, walls);

        if (hitFound)
        {
            GameObject hitGameObject = interactionRayHit.transform.gameObject;
            string hitFeedback = hitGameObject.name;
            DistanceForward = interactionRayHit.distance;
            Debug.Log(DistanceForward);
            
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(introWaitTime);
        FootstepMixer.SetFloat(Footstep_Slapback, -80);
    }
}
