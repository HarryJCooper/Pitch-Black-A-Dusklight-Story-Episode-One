using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableFromArray : MonoBehaviour
{
    public GameObject ToBeEnabled;
    public GameObject[] ToBeEnabledArray;
    public GameObject ToBeDisabled;
    bool hasBeenEnabled;

    void OnTriggerEnter()
    {
        if (!hasBeenEnabled)
        {
            ToBeEnabled = ToBeEnabledArray[Random.Range(0, ToBeEnabledArray.Length)];
            ToBeEnabled.SetActive(true);
            hasBeenEnabled = true;
        }
    }

    void OnTriggerExit()
    {
        ToBeDisabled.SetActive(false);
    }
}
