using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerDestroy : MonoBehaviour
{

    public GameObject gameobject;

    void OnTriggerEnter(Collider other)
    {
        gameobject.SetActive(false);   
    }

}
