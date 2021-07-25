using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Laugher : MonoBehaviour
{
    public GameObject Megapedeprefab;
    public Vector3 center;
    public Vector3 size;
    // Start is called before the first frame update
    void Start()
    {
        SpawnMegapede();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnMegapede()
    {
        Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
        Instantiate(Megapedeprefab, pos, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        center = transform.position;
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
}
