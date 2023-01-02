using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [SerializeField] NewCombatSystem newCombatSystem;
    [SerializeField] Controls controls;
    public bool inGrapple;
    [SerializeField] int grappleInt = 50;
    
    void GrappleSound(){

    }

    void Grappling(){
        if (grappleInt < 1) return;
        if (controls.tap) GrappleSound();
    }
    
    public void StartGrapple(){

    }

    void Update(){
        if (inGrapple) Grappling();
    }
}
