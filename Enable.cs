using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enable : MonoBehaviour
{

    public GameObject ToBeEnabled;
    public GameObject ToBeEnabled1;
    public GameObject ToBeDisabled;

    void OnTriggerEnter()
    {
        MakeThis();
    }

    void MakeThis()
    {
        if (ToBeEnabled)
        {
            ToBeEnabled.SetActive(true);
            if (ToBeEnabled1)
            {
                ToBeEnabled1.SetActive(true);
            }
        }
        else
        {
            return;
        }
        ToBeDisabled.SetActive(false);
    }

    void OnTriggerExit()
    {
        if (ToBeDisabled)
        {
            ToBeDisabled.SetActive(false);
        }
        else
        {
            return;
        }
    }
}