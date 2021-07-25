using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopLowpassAdjuster : MonoBehaviour
{
    public GameObject toBeLowPassed;

    private void OnTriggerEnter(Collider other)
    {
       toBeLowPassed.GetComponent<LowPassAutomator>().enteredDesert = true;
    }
}
