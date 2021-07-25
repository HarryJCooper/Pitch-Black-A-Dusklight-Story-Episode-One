using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasReached : MonoBehaviour
{
    public RepeatUntilReach repeatUntilReach;
    bool hasReached;

    void OnTriggerEnter() 
    {
        if (!hasReached)
        {
            StartCoroutine(repeatUntilReach.Repeater());
            hasReached = true;
        }
    }
}
