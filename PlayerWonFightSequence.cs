using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWonFightSequence : SequenceBase
{
    [SerializeField] AudioSource protagSource, doorSource;
    [SerializeField] AudioClip[] protagClips;
    [SerializeField] AudioClip doorShutClip;
    [SerializeField] GuardCombatSequence guardCombatSequence;
    [SerializeField] TurnOffOnEnter tannoyOne, tannoyTwo, tannoyThree;
    [SerializeField] DarkVsLight darkVsLight;

    public override IEnumerator Sequence(){
        if (finished == 0){
            if (active == 1){
                darkVsLight.playerDarkness += 1;
                // If the player goes onto win the fight against the guard, at any stage, the protag, out of breath, remarks before entering the station. 
                // Protag 
                // Can see why they only needed one guard, guy’s a beast. Fighting in the dark’ll take some getting used to… Nightlander’s cure doesn’t come without its downfalls. Let’s head in, and then get the hell out… 
                protagSource.PlayOneShot(protagClips[0]);
                yield return new WaitForSeconds(protagClips[0].length);

                // TODO - add door shut clip
                
                // Player then heads through; the door shuts behind him. 
                // Protag 
                // Alright, gotta find the overseer’s office, straight down these stairs, then just follow the signs. 
                protagSource.PlayOneShot(protagClips[1]);
                yield return new WaitForSeconds(protagClips[1].length);
                StartCoroutine(Finished());
            }
        }
    }

    IEnumerator Finished(){
        guardCombatSequence.enabled = false;
        darkVsLight.SetTrappedWorkerVol();
        finished = 1;
        yield break;
    }
}
