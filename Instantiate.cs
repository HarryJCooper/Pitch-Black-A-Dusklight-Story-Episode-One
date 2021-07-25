using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate : MonoBehaviour

{
    public GameObject ToBeMade;
    public GameObject ToBeDestroyed;

    void OnTriggerEnter()
    {
        MakeThis();   
    }

    void MakeThis()
    {
        Instantiate(ToBeMade);
        Destroy(ToBeDestroyed);
    }

    void OnTriggerExit()
    {
        Destroy(ToBeDestroyed);
    }
}
