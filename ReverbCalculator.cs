using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ReverbCalculator : MonoBehaviour
{
    public float forwardRoomDistance, backwardRoomDistance, leftRoomDistance, rightRoomDistance, maxDistance, totalRoomSize, decayTime;
    public float forwardDistance, leftDistance, rightDistance;
    public float reflectionTime, reverbVolume;
    public string decayTimeString, roomReverbVolumeString, reverbVolumeString;
    public string reflectionTimeStringLeft, reflectionTimeStringRight;
    public string overallReflectionVolumeString, leftReflectionVolumeString, rightReflectionVolumeString;
    public AudioMixer audioMixer0, audioMixer1;
    public LayerMask wallsLayerMask;
    public bool reverbCalculator, leftSide, rightSide;
    public float distanceAdjuster, volumeAdjuster, maxWallDistance;
    public Collider[] walls;
    public List<Collider> wallsList = new List<Collider>();
    public Vector2 angleToWallVector;
    public float angleToWall, correctedAngleToWall, rightVolume, leftVolume;
    public Collider nearestWall;
    float distanceToWall;
    public float distanceToClosestWall;
    Vector3 targetDir;

    void Start()
    {
        distanceToClosestWall = 100;
        InvokeRepeating("WallChecker", 0f, 0.5f);
        
    }

    void WallChecker()
    {
        distanceToClosestWall = Mathf.Infinity;
        walls = Physics.OverlapSphere(transform.position, maxWallDistance, wallsLayerMask);
        foreach (Collider wall in walls)
        {
            distanceToWall = Vector3.Distance(transform.position, wall.transform.position);
            if (distanceToWall < distanceToClosestWall)
            {
                targetDir = wall.transform.position - transform.position;
                distanceToClosestWall = distanceToWall;
                nearestWall = wall;
                angleToWall = Vector3.SignedAngle(transform.forward, targetDir, Vector3.up);
                correctedAngleToWall = Vector3.SignedAngle(-transform.forward, targetDir, Vector3.up);
            }
        }
        
        // SHOULD GET DISTANCE TO THE CLOSEST WALL USING THIS RATHER THAN EXTRA RAYCAST?
    }

    void Update()
    {
        
        if (reverbCalculator && (distanceToClosestWall < maxWallDistance))
        {

            // GETS DISTANCE FROM WALLS FOR LATE REVERB
            Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hitRoomForward, maxDistance, wallsLayerMask, QueryTriggerInteraction.Ignore);
            forwardRoomDistance = hitRoomForward.distance;

            Physics.Raycast(transform.position, Vector3.left, out RaycastHit hitRoomLeft, maxDistance, wallsLayerMask, QueryTriggerInteraction.Ignore);
            leftRoomDistance = hitRoomLeft.distance;

            Physics.Raycast(transform.position, Vector3.right, out RaycastHit hitRoomRight, maxDistance, wallsLayerMask, QueryTriggerInteraction.Ignore);
            rightRoomDistance = hitRoomRight.distance;
            
            // CALCULATES TOTAL ROOM SIZE FOR LATE REVERB
            totalRoomSize = forwardRoomDistance + backwardRoomDistance + leftRoomDistance + rightRoomDistance;
            decayTime = totalRoomSize / 200;

            // SETS DECAY TIME ON MASTER REVERB TO ROOMSIZE/MULTIPLIER
            audioMixer0.SetFloat(decayTimeString, decayTime);
            audioMixer1.SetFloat(decayTimeString, decayTime);

            // MODIFIES REFLECTION TIME
            reflectionTime = distanceToClosestWall / distanceAdjuster;
            // SETS EARLY REFLECTION TIME BASED ON DISTANCE TO THE WALL
            
            // DOES FIRST REFLECTION! 
            audioMixer1.SetFloat(reflectionTimeStringLeft, reflectionTime);
            audioMixer1.SetFloat(reflectionTimeStringRight, reflectionTime);
            
            if (angleToWall < 90 && angleToWall > -90)
            {
                rightVolume = angleToWall / 5;
                leftVolume = angleToWall / 5 * -1;
            }
            else
            {
                rightVolume = correctedAngleToWall / 5 * -1;
                leftVolume = correctedAngleToWall / 5;
            }


            audioMixer1.SetFloat(rightReflectionVolumeString, rightVolume);
            audioMixer1.SetFloat(leftReflectionVolumeString, leftVolume);

            //Set the Quaternion rotation from the GameObject's position to the next GameObject's position

            //Rotate the GameObject towards the second GameObject
            reverbVolume = 0;

            audioMixer1.SetFloat(reverbVolumeString, reverbVolume);
            audioMixer1.SetFloat(roomReverbVolumeString, reverbVolume);

        }
        else
        {
            rightVolume = -80.0f;
            leftVolume = -80.0f;

            reverbVolume = -10000.0f;

            audioMixer1.SetFloat(rightReflectionVolumeString, rightVolume);
            audioMixer1.SetFloat(leftReflectionVolumeString, leftVolume);

            audioMixer1.SetFloat(reverbVolumeString, reverbVolume);
            audioMixer1.SetFloat(roomReverbVolumeString, reverbVolume);

        }
    }
}
