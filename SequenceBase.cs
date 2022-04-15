using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceBase : MonoBehaviour
{
    public int active = 1, triggered = 0, finished = 0;
    public AudioClip cutsceneEnterClip, cutsceneExitClip;

    public virtual IEnumerator Loop(){
        yield break;
    }

    public virtual IEnumerator Sequence(){
        yield break;
    }

    void Start(){
        if (active == 1 && finished == 0){
            StartCoroutine(Sequence());
            StartCoroutine(Loop());
        }
    }
}
