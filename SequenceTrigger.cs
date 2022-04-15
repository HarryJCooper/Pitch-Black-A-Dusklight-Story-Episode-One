using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceTrigger : SequenceBase
{
    void OnTriggerEnter(Collider other){
        if (other.gameObject.name != "Player") return;
        var script = this.transform.parent.gameObject.GetComponent(this.name.Split('_')[0]) as SequenceBase;
        if (script.active == 1) script.triggered = 1;
    }
}
