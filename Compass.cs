using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    int directionNESW;
    public bool compassTurnedOn;
    public AudioSource player;
    public AudioClip northClip, eastClip, southClip, westClip;

    void Update()
    {
        if (compassTurnedOn)
        {
            if (transform.localEulerAngles.y > -10 && transform.localEulerAngles.y < 10 && directionNESW != 1)
            {
                directionNESW = 1;
                player.PlayOneShot(northClip, 0.3f);
            }
            else if (transform.localEulerAngles.y > 80 && transform.localEulerAngles.y < 100 && directionNESW != 2)
            {
                directionNESW = 2;
                // player.PlayOneShot(eastClip, 0.3f);
            }
            else if (transform.localEulerAngles.y > 170 && transform.localEulerAngles.y < 190 && directionNESW != 3 || transform.localEulerAngles.y < -170 && transform.localEulerAngles.y < -190 && directionNESW != 3)
            {
                directionNESW = 3;
                player.PlayOneShot(southClip, 0.3f);
            }
            else if (transform.localEulerAngles.y > -100 && transform.localEulerAngles.y < -80 && directionNESW != 4 || transform.localEulerAngles.y > 260 && transform.localEulerAngles.y < 280 && directionNESW != 4)
            {
                directionNESW = 4;
                // player.PlayOneShot(westClip, 0.3f);
            }
        }
    }
}

