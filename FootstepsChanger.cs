using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsChanger : MonoBehaviour
{
    public bool hasEntered;

    private void OnTriggerStay(Collider other)
    {
        hasEntered = true;
    }
    private void OnTriggerExit(Collider other)
    {
        hasEntered = false;
    }
}
