using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dia7ogueSequence : MonoBehaviour
{
    [SerializeField] AudioSource protagSource;
    [SerializeField] AudioClip protagClip;
    [SerializeField] Controls controls;
    public int finished;

    void OnTriggerEnter(Collider other){ if (other.gameObject.name == "Player") StartCoroutine(Sequence());}

    IEnumerator Sequence(){
        if (finished == 0 && !protagSource.isPlaying && controls.inExplore){
            finished = 1;
            // Protag 
            // Can hear Lambton Energy from here, we heard rumours back home, but… I never knew it’d be like this… good job the Nightlander’s cure makes you listen, wouldn’t wanna see that name plastered everywhere.
            protagSource.PlayOneShot(protagClip);
            yield return new WaitForSeconds(protagClip.length);    
        }
    }
}
