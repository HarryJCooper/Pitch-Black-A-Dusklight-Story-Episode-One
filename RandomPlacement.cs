using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlacement : MonoBehaviour
{
    
    public float min, max, maxDist, playerDist;
    float xPlantPos, zPlantPos;
    public GameObject player;

    private void Start()
    {
        xPlantPos = player.transform.position.x + Random.Range(min, max);
        zPlantPos = player.transform.position.z + Random.Range(min, max);
        transform.position = new Vector3(xPlantPos, 0, zPlantPos);
    }

    private void Update()
    {
        playerDist = Vector3.Distance(player.transform.position, transform.position);

        // MOVE PLANTS
        if (playerDist > maxDist)
        {
            float x = Random.Range(0, 1.0f);
            if (x > 0.5f)
            {
                xPlantPos = player.transform.position.x - Random.Range(max, max - 10);
            }
            if (x <= 0.5f)
            {
                xPlantPos = player.transform.position.x + Random.Range(max, max - 10);
            }

            float z = Random.Range(0, 1.0f);
            if (z > 0.5f)
            {
                zPlantPos = player.transform.position.z - Random.Range(max, max - 35);
            }
            if (z <= 0.5f)
            {
                zPlantPos = player.transform.position.z + Random.Range(max, max - 35);
            }

            if (playerDist > maxDist)
            {
                transform.position = new Vector3(xPlantPos, 0, zPlantPos);
            }
        }   
    }
}
