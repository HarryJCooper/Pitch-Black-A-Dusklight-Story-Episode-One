using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DearVR;

public class PlayerLostFightSequence : SequenceBase
{
    [SerializeField] AudioSource protagSource, protagActionSource, protagReverbSource, guardSource;
    DearVRSource protagReverbVRSource, guardVRSource;
    [SerializeField] AudioSource[] desertAmbienceSources;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] TurnOffOnEnter tannoyOne, tannoyTwo, tannoyThree;
    [SerializeField] AudioClip[] protagClips, guardClips, playerFootstepClips, guardFootstepClips;
    [SerializeField] AudioClip flashbackClip, batonClip, explosionClip;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    [SerializeField] DarkVsLight darkVsLight;
    [SerializeField] AudioController audioController;
    [SerializeField] DisableFromArray disableFromArray;
    [SerializeField] EnableFromArray afterLostFightEnableFromArray;
    [SerializeField] AmbienceRepeater desertAmbienceRepeater;
    [SerializeField] ReverbManager reverbManager;
    [SerializeField] EnemyFootsteps guardFootsteps;
    bool guardWalksAway, inFlashback, transitioning;
    float adjuster;
    [SerializeField] GameObject doorObject, changeFootstepsObject, boundaryObject;

    void Start(){
        protagReverbVRSource = protagReverbSource.GetComponent<DearVRSource>();
        protagReverbVRSource.performanceMode = true;
    }

    void PlayOneShotWithVerb(AudioClip clip){
        protagSource.PlayOneShot(clip);
        protagReverbSource.volume = 0.4f;
        protagReverbVRSource.DearVRPlayOneShot(clip);
    }

    void FixedUpdate(){
        if (guardWalksAway) guardSource.gameObject.transform.position = Vector3.MoveTowards(guardSource.gameObject.transform.position, protagSource.gameObject.transform.position, -0.1f); 
    }

    public void StartSequence(){
        StartCoroutine(Sequence());
    }

    public override IEnumerator Sequence(){
        if (finished == 0){
            if (active == 1){
                guardVRSource = guardSource.GetComponent<DearVRSource>();
                boundaryObject.SetActive(false);
                darkVsLight.playerDarkness += 1;
                controls.inCutscene = true;
                StartCoroutine(audioController.ReduceMasterCutOff(3f));
                yield return new WaitForSeconds(9f);
                audioController.SetCutOffToZero();
                reverbManager.turnedOn = true;
                foreach (AudioSource audioSource in desertAmbienceSources) if(audioSource.gameObject.activeSelf) audioSource.Stop();
                disableFromArray.DisableAllInArray();
                afterLostFightEnableFromArray.EnableAllInArray();
                desertAmbienceRepeater.stopped = true;
                pBFootstepSystem.footstepClips = playerFootstepClips;
                guardFootsteps.footstepClips = guardFootstepClips;
                protagSource.gameObject.transform.position = new Vector3(72.4f, 0.3f, 469.3f);
                guardSource.gameObject.transform.position = new Vector3(70f, 0, 475.4f);
                StartCoroutine(audioController.IncreaseMasterCutOff(7.5f));
                yield return new WaitForSeconds(9f);
                guardSource.gameObject.SetActive(true);
                // The audio fades back in and the guard speaks to the protag. The protag is now in a holding cell. 
                // Guard 
                // Nightlanders - your leader talks frequently of being trapped - Look up the definition of irony in your next life. 
                guardVRSource.DearVRPlayOneShot(guardClips[0]);
                yield return new WaitForSeconds(guardClips[0].length);

                // Protag
                // You don’t know the half of it.
                PlayOneShotWithVerb(protagClips[0]);
                yield return new WaitForSeconds(protagClips[0].length);

                // Guard
                // Enjoy your stay in the holding cell… rebel. Patrol mode initiated. *sigh of relief*
                guardVRSource.DearVRPlayOneShot(guardClips[1]); 
                yield return new WaitForSeconds(guardClips[1].length);
                guardVRSource.DearVRPlayOneShot(batonClip);
                guardWalksAway = true;
                // The guard laughs as he walks away. He also clangs his baton on metal bars to signify the protag is in a holding a cell. If the player chose to fight (i.e., engaged the guard or was caught on foot) then a mental flashback occurs around twenty seconds in. 
                // Protag 
                // Aghh! dammit, it’s happening again. Mmmm! 
                PlayOneShotWithVerb(protagClips[1]);
                yield return new WaitForSeconds(protagClips[1].length - 2f);
                StartCoroutine(audioController.ReduceExposedParameterCutOff("OtherSources_CutOff", 5f));
                guardSource.gameObject.SetActive(false);
                pBFootstepSystem.canRotate = false;
                protagSource.transform.rotation = Quaternion.identity;
                protagActionSource.PlayOneShot(flashbackClip);
                yield return new WaitForSeconds(flashbackClip.length + 5f);
                StartCoroutine(audioController.IncreaseExposedParameterCutOff("OtherSources_CutOff", 5f));
                yield return new WaitForSeconds(5f);
                // Audio then returns to normal via a textural fade. As the audio fades in/out, the player suddenly hears a siren and debris falling in the near distance. 
                // Protag
                // Gotta get out, and quick! Right, gotta be a way to get this door open… 
                PlayOneShotWithVerb(protagClips[2]);
                yield return new WaitForSeconds(protagClips[2].length);
                protagActionSource.PlayOneShot(explosionClip);
                yield return new WaitForSeconds(explosionClip.length - 10.5f);
                // StartCoroutine(tannoyOne.Sequence());
                // StartCoroutine(tannoyTwo.Sequence());
                // StartCoroutine(tannoyThree.Sequence());
                doorObject.SetActive(false);
                pBFootstepSystem.canRotate = true;
                controls.inCutscene = false;
                audioMixer.SetFloat("Footsteps_Vol", 0f);
                yield return new WaitForSeconds(5f);
                // After another five seconds of impending audio and panic, the gate is destroyed by debris. The protagonist is thrown backwards. They then get up and proceed as normal. 
                // Protag 
                // Blast blew the door right open. What do they say? Luck of the… Nightlanders is it? Lets see if that luck can find us those docs.  
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
        finished = 1;
        yield break;
    }
}
