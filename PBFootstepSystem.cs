﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBFootstepSystem : MonoBehaviour
{
    public AudioClip[] footTypeOne;
    public AudioClip[] footsteps;
    public GameObject[] sewerTiles;
    public float walkDist;
    public float maxDist;
    public float moveSpeed, sprintMoveSpeed, initialMoveSpeed, initialFootstepDelay;
    public float rotSpeed, rotationTimer;
    private Vector3 recentTransform;
    private Vector3 Player;
    public float fWallDist;
    public float bWallDist;
    public float maxWallDist;
    private float maxDistance = 200;
    public AudioSource footstepSound;
    public bool raycastToWall;
    public bool footstepsTypeOne;
    public GameObject[] Ts;
    public Vector3 currentPos;
    public bool canMove, firstLevel;
    public bool moveForward;
    public Controls controls;

    public bool canRotate = true;
    bool initialFootstepAllow;



    void Start()
    {
        Player = transform.localPosition;
        if (footstepsTypeOne)
        {
            footsteps = footTypeOne;
        }

        fWallDist = bWallDist = 5;
        controls = GameObject.Find("Controls").GetComponent<Controls>();
    }

    void Update()
    {
        if (controls.sprint)
        {
            moveSpeed = sprintMoveSpeed;
        }
        else
        {
            moveSpeed = initialMoveSpeed;
        }
        
        Vector3 Origin = transform.position;
        Vector3 fDirection = transform.forward;
        Vector3 bDirection = -transform.forward;

        if (raycastToWall)
        {
            Ray fRay = new Ray(Origin, fDirection);
            Ray bRay = new Ray(Origin, bDirection);

            if (Physics.Raycast(fRay, out RaycastHit fRaycastHit, maxDistance, -5, QueryTriggerInteraction.Ignore))
            {
                fWallDist = fRaycastHit.distance;
            }
            if (Physics.Raycast(bRay, out RaycastHit bRaycastHit, maxDistance, -5, QueryTriggerInteraction.Ignore))
            {
                bWallDist = bRaycastHit.distance;
            }
        }

        if (canRotate)
        {
            if (controls.turnLeft)
            {
                transform.Rotate(Vector3.up, -rotSpeed * Time.deltaTime);
            }

            if (controls.turnRight)
            {
                transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
            }
        }

        if (canMove)
        {
            if (!initialFootstepAllow)
            {
                initialFootstepDelay += Time.deltaTime;
            }

            if (initialFootstepDelay > 0.3)
            {
                initialFootstepAllow = true;
            }

            if (controls.firstMoveForward)
            {
                recentTransform = transform.position;
                if (initialFootstepAllow)
                {
                    footstepSound.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)], Random.Range(0.8f, 1f));
                    initialFootstepAllow = false;
                    initialFootstepDelay = 0;
                }
            }

            if (controls.firstMoveBackward)
            {
                recentTransform = transform.position;
                if (initialFootstepAllow)
                {
                    footstepSound.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)], Random.Range(0.8f, 1f));
                    initialFootstepAllow = false;
                    initialFootstepDelay = 0;
                }
            }

            

            if (controls.moveForward)
            {
                walkDist += Vector3.Distance(recentTransform, transform.position) * Time.deltaTime * 60;
            }

            else if (controls.moveBackward)
            {
                walkDist += Vector3.Distance(recentTransform, transform.position) * Time.deltaTime * 60;
            }
            else
            {
                walkDist = 0;
                recentTransform = transform.position;
            }

            if (controls.moveForward)
            {
                if (fWallDist > maxWallDist)
                {
                    transform.position += transform.forward * Time.deltaTime * moveSpeed;
                }
            }

            else if (controls.moveBackward)
            {
                if (bWallDist > maxWallDist)
                {
                    transform.position -= transform.forward * Time.deltaTime * moveSpeed;
                }
            }

            if (walkDist > maxDist)
            {
                footstepSound.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)], Random.Range(0.8f, 1f));
                walkDist = 0;
                recentTransform = transform.position;
            }
        }


        if (canRotate)
        {
            if (controls.turnRight && !controls.moveForward && !controls.moveBackward)
            {
                rotationTimer += Time.deltaTime;
            }

            if (controls.turnLeft && !controls.moveForward && !controls.moveBackward)
            {
                rotationTimer += Time.deltaTime;
            }

            if (rotationTimer > 0.60f)
            {          
                footstepSound.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)], Random.Range(0.8f, 1f));
                rotationTimer = 0;
            }
        }
    }
}
