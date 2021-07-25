using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePOI : MonoBehaviour
{

    public GameObject notPOI;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            notPOI.SetActive(false);
        }
        else
        {
            notPOI.SetActive(true);
        }
    }
}
