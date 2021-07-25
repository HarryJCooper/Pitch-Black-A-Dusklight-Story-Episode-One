using UnityEngine;
using System.Collections;

public class Zoom : MonoBehaviour
{

    public float movementSpeed = 5.0f;
    public Transform other;
    public float dist;
    public Transform theParent;
    public Transform theChild;
    


    void Update()
    {
        theParent.rotation = theChild.rotation;
        theChild.rotation = Quaternion.identity;
        float dist = Vector3.Distance(other.position, transform.position);
        print("Distance to other: " + dist);
        {
            
            {

                if (Input.GetKey(KeyCode.E))
                {
                    transform.position += transform.forward * Time.deltaTime * movementSpeed;
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    transform.position -= transform.forward * Time.deltaTime * movementSpeed;
                }
            }
        }
    }
}