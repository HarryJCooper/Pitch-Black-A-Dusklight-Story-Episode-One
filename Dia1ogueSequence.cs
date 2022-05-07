using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dia1ogueSequence : MonoBehaviour
{
    [SerializeField] AudioSource protagSource;
    [SerializeField] AudioClip protagClip;
    [SerializeField] Controls controls;
    public int finished;

    IEnumerator Sequence(){
        yield return new WaitForSeconds(15f);
        if (finished == 1 || protagSource.isPlaying || !controls.inExplore) yield break;
        // Protag
        // *Agghh, mmmm*. Gotta focus. Ok, new land, new perspective. Mission starts here. Let’s see if those docs set me on the right path.
        protagSource.PlayOneShot(protagClip);
        yield return new WaitForSeconds(protagClip.length);
        finished = 1;
    }

    void Start(){StartCoroutine(Sequence());}
}
