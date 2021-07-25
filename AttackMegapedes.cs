using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMegapedes : MonoBehaviour
{

    private UnityEngine.AI.NavMeshAgent _agent;

    public GameObject Player;

    public float EnemyDistanceRun = 10.0f;

    void Start()
    {
        _agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        Player = GameObject.Find("Megapede 1");
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, Player.transform.position);

        // Run away from player
        if (distance < EnemyDistanceRun)

        {
            Vector3 dirToPlayer = transform.position + Player.transform.position;

            Vector3 newPos = transform.position - dirToPlayer;

            _agent.SetDestination(newPos);
        }
    }
}
