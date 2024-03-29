﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public GameObject Megapedeprefab;
    public Vector3 center;
    public Vector3 size;

    void Start()
    {
        SpawnMegapede();   
    }

    public void SpawnMegapede()
    {
            Vector3 pos = center + new Vector3(Random.Range(-size.x / 2 , size.x /2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
            Instantiate(Megapedeprefab, pos, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        center = transform.position;
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
}
