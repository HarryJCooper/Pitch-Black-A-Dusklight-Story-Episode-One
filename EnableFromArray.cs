using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableFromArray : MonoBehaviour
{
    [SerializeField] GameObject[] gameObjects;
    [SerializeField] bool trigger;

    public void EnableAllInArray(){ foreach (GameObject gameObject in gameObjects) gameObject.SetActive(true);}
    void OnTriggerEnter(Collider other){ if (other.gameObject.name == "Player" && trigger) EnableAllInArray();}
}
