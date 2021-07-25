using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatUntilReach : MonoBehaviour
{
    public bool hasReached, playedFinal;
    public int audioInt;
    public AudioClip[] audioClips;
    public AudioSource audioSource;
    public RepeatUntilReach repeatUntilReach;
    
    public IEnumerator Repeater()
    {

        if (!hasReached && audioInt == 0)
        {
            // PLAYS FOR THE FIRST REPEAT
            audioSource.PlayOneShot(audioClips[audioInt]);
        }
        if (!hasReached && audioInt == 1)
        {
            // REPEATS UNTIL REACHED
            audioSource.PlayOneShot(audioClips[audioInt]);
        }
        else if (hasReached)
        {
            // DETERMINED BY ON TRIGGER ENTER
            // PLAYS THE FINAL AUDIOCLIP
            audioInt = 2;
            audioSource.PlayOneShot(audioClips[audioInt]);
        }
        // WAITS FOR THE CLIP TO PLAY + 5 SECONDS
        yield return new WaitForSeconds(audioClips[audioInt].length + 5f);
        if (!hasReached)
        {
            // REPEATS THE STANDARD AUDIOCLIP BY CHANGED audioInt to 1
            audioInt = 1;
            StartCoroutine(Repeater());
        }
        else if (hasReached)
        {
            // STARTS THE NEXT PoI

            if (!playedFinal)
            {
                StartCoroutine(Repeater());
                playedFinal = true;
            }
            else if (playedFinal)
            {
                if (repeatUntilReach)
                {
                    StartCoroutine(repeatUntilReach.Repeater());
                }
            }
        }
    }


    void OnTriggerEnter()
    {
        hasReached = true;
        audioInt = 2;
    }
}
