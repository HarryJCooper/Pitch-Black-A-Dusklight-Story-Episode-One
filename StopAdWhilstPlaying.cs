using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAdWhilstPlaying : MonoBehaviour
{
    public GameObject ThisObject;
    public AudioSource ThatObject;
    public GameObject Ambience1;
    public ChangeAfterFocus ArcadeAd;
    public AudioSource Arcade_Ad;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()

    {
        if (ArcadeAd.hit)
        {
            Ambience1.SetActive(false);
        }
        if (!ThatObject.isPlaying)
        {
            ThisObject.SetActive(true);
        }
    }
}
    
