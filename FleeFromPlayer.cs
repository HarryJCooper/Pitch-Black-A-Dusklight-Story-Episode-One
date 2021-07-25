using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeFromPlayer : MonoBehaviour
{

    public float enemyRunDirection;

    public GameObject Player;

    private bool hasEscaped;

    public float EnemyDistanceRun = 10.0f;

    void Start()
    {
       
        Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, Player.transform.position);

        if (distance > 3 * EnemyDistanceRun)
        {
            hasEscaped = false;
        }

        // Run away from player
        if (distance < EnemyDistanceRun)

            

        {
            if (!hasEscaped)
            {
                enemyRunDirection = Random.Range(0, 360);

                transform.Rotate(Vector3.up, enemyRunDirection);

                hasEscaped = true;
            }

            Vector3 dirToPlayer = transform.position - Player.transform.position;

            Vector3 newPos = transform.position + dirToPlayer;

            

            transform.position += transform.forward * Time.deltaTime * 10;

        } 
    }
}
