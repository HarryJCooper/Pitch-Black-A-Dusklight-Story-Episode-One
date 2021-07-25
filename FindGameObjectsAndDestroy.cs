using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindGameObjectsAndDestroy : MonoBehaviour
{

    public GameObject[] desertcreature;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        desertcreature = GameObject.FindGameObjectsWithTag("Desert Creatures");
        foreach (GameObject r in desertcreature)
        {
            Destroy(r.gameObject);
        }
    }
}
