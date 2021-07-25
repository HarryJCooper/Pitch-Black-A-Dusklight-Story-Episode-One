using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReusLesson : MonoBehaviour
{
    float cheeseCake = 5;
    void Start()
    {
        if(cheeseCake > 4)
        {
            Debug.Log("yay lots of cheesecake");
        }
        else 
        {
            Debug.Log("I wish we had more cheesecake");
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            
        }
    }
}
