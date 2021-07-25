using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateFromArray : MonoBehaviour
{
    public GameObject[] ToBeMadeArray;
    public GameObject ToBeDestroyed;

    void OnTriggerEnter()
    {
        Instantiate(ToBeMadeArray[Random.Range(0, ToBeMadeArray.Length)]);
    }

    void OnTriggerExit()
    {
        Destroy(ToBeDestroyed);
    }
}
