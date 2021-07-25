using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableReflections : MonoBehaviour
{
    public ReverbCalculator reverbCalculator;

    void OnTriggerEnter()
    {
        reverbCalculator.reverbCalculator = true;
    }
}
