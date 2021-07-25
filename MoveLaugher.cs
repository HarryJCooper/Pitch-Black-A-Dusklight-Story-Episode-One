using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLaugher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(transform.position.x + 321, transform.position.y + 11, transform.position.z + 280);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
