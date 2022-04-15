using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLostFightSequence : SequenceBase
{
    [SerializeField] AudioSource protagSource, protagReverbSource, guardSource, sirenSource, debrisSource, flashbackSource;
    [SerializeField] AudioSource[] explosionSources, desertAmbienceSources;
    [SerializeField] TurnOffOnEnter tannoyOne, tannoyTwo, tannoyThree;
    [SerializeField] AudioClip[] protagClips, guardClips, explosionClips;
    [SerializeField] AudioClip flashbackClip, batonClip;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] AudioController audioController;
    [SerializeField] ChangeFootsteps changeFootsteps;
    [SerializeField] RemoveReverbTrigger removeReverbTrigger;
    [SerializeField] EnableFromArray enableFromArray;
    [SerializeField] AmbienceRepeater desertAmbienceRepeater;
    bool guardWalksAway;

    void PlayOneShotWithVerb(AudioClip clip){
        protagSource.PlayOneShot(clip);
        protagReverbSource.PlayOneShot(clip);
    }

    void FixedUpdate(){
        if (guardWalksAway) guardSource.gameObject.transform.position = Vector3.MoveTowards(guardSource.gameObject.transform.position, protagSource.gameObject.transform.position, -0.1f); 
    }

    public override IEnumerator Sequence(){
        if (finished == 0){
            if (active == 1){
                darkVsLight.playerDarkness += 1;
                controls.inCutscene = true;
                StartCoroutine(audioController.ReduceMasterCutOff(7.5f));
                yield return new WaitForSeconds(7.5f);
                foreach (AudioSource audioSource in desertAmbienceSources) audioSource.Stop();
                desertAmbienceRepeater.stopped = true;
                changeFootsteps.ChangePlayerFootsteps();
                removeReverbTrigger.TriggerRemotely();
                protagSource.gameObject.transform.position = new Vector3(72.4f, 0.3f, 460.9f);
                guardSource.gameObject.transform.position = new Vector3(70f, 0, 475.4f);
                StartCoroutine(audioController.IncreaseMasterCutOff(7.5f));
                yield return new WaitForSeconds(7.5f);
                // The audio fades back in and the guard speaks to the protag. The protag is now in a holding cell. 
                // Guard 
                // Nightlanders - your leader talks frequently of being trapped - Look up the definition of irony in your next life. 
                guardSource.PlayOneShot(guardClips[0]);
                yield return new WaitForSeconds(guardClips[0].length);

                // Protag
                // You don’t know the half of it.
                PlayOneShotWithVerb(protagClips[0]);
                yield return new WaitForSeconds(protagClips[0].length);

                // Guard
                // Enjoy your stay in the holding cell… rebel. Patrol mode initiated. *sigh of relief*
                guardSource.PlayOneShot(guardClips[1]); 
                yield return new WaitForSeconds(guardClips[1].length);
                guardSource.PlayOneShot(batonClip);
                guardWalksAway = true;
                // The guard laughs as he walks away. He also clangs his baton on metal bars to signify the protag is in a holding a cell. If the player chose to fight (i.e., engaged the guard or was caught on foot) then a mental flashback occurs around twenty seconds in. 
                // Protag 
                // Aghh! dammit, it’s happening again. Mmmm! 
                PlayOneShotWithVerb(protagClips[1]);
                yield return new WaitForSeconds(protagClips[1].length);
                StartCoroutine(audioController.ReduceMasterCutOff(7.5f));
                yield return new WaitForSeconds(7.5f);
                guardSource.gameObject.SetActive(false);
                // FLASHBACK 
                flashbackSource.PlayOneShot(flashbackClip);
                yield return new WaitForSeconds(flashbackClip.length);
                StartCoroutine(audioController.IncreaseMasterCutOff(7.5f));
                yield return new WaitForSeconds(7.5f);
                // Audio then returns to normal via a textural fade. As the audio fades in/out, the player suddenly hears a siren and debris falling in the near distance. 

                // Protag
                // Gotta get out, and quick! Right, gotta be a way to get this door open… 
                PlayOneShotWithVerb(protagClips[2]);
                yield return new WaitForSeconds(protagClips[2].length);
                foreach (AudioSource explosionSource in explosionSources) explosionSource.PlayOneShot(explosionClips[Random.Range(0, explosionClips.Length)]);
                yield return new WaitForSeconds(5f);
                // After another five seconds of impending audio and panic, the gate is destroyed by debris. The protagonist is thrown backwards. They then get up and proceed as normal. 
                // Protag 
                // Blast blew the door right open. What do they say? Luck of the… Nightlanders is it? Lets see if that luck can find us those docs.  
                enableFromArray.EnableAllInArray();
                PlayOneShotWithVerb(protagClips[3]);
                yield return new WaitForSeconds(protagClips[3].length);
                StartCoroutine(Finished());
            }
        }
    }

    IEnumerator Finished(){
        controls.inCutscene = false;
        controls.canZoom = true;
        darkVsLight.SetTrappedWorkerVol();
        StartCoroutine(tannoyOne.Sequence());
        StartCoroutine(tannoyTwo.Sequence());
        StartCoroutine(tannoyThree.Sequence());
        finished = 1;
        yield break;
    }
}
