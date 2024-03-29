using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DearVR;

public class AroundTableSequence : SequenceBase
{
    public AudioSourceContainer audioSourceContainer;
    [SerializeField] AmbienceRepeater ambienceRepeater;
    public AudioClip[] nightlanderOneClips, nightlanderTwoClips, nightlanderThreeClips, nightlanderFourClips, protagClips, loopClips;
    [SerializeField] AudioSource musicSource;
    [SerializeField] float maxDistanceFromPlayer;
    [SerializeField] AuditoryZoomSequence auditoryZoomSequence;
    [SerializeField] SaveAndLoadEncampment saveAndLoadEncampment;
    [SerializeField] AudioClip sitDownClip, awkwardCutleryClip, slamAndGetUpClip, musicClip;
    [SerializeField] Controls controls;
    [SerializeField] AudioController audioController;
    [SerializeField] DarkVsLight darkVsLight;
    public bool inBunker;
    //     Protag will encounter Nightlander agents around a table, eating. If the protag gets close enough, he is invited for din dins. Player can hover around the area and listen in. 

    bool MovedAwayChecker(){
        if (Vector3.Distance(audioSourceContainer.nightlanderOneSource.transform.position, audioSourceContainer.protagSource.transform.position) > maxDistanceFromPlayer){
            return true;
        }
        return false;
    }

    IEnumerator MovedAway(){
        yield return new WaitForSeconds(5f);
        if (MovedAwayChecker()){
            // IF player walks towards nightlanders then
            // Nightlander 1 
            // Suit yourself… 
            audioSourceContainer.nightlanderOneVRSource.DearVRPlayOneShot(nightlanderOneClips[5]);
            yield return new WaitForSeconds(nightlanderOneClips[5].length);

            // Protag
            // That church stole everything from me. Nightlanders wanna sit around and talk politics then they can, but I came for answers and I get the feeling they don’t have any. 
            audioSourceContainer.protagSource.PlayOneShot(protagClips[7]);
            yield return new WaitForSeconds(protagClips[7].length);
            darkVsLight.playerDarkness += 1;
            Finished();
        } else {
            StartCoroutine(SequenceTwo());
        }
        
    }

    public override IEnumerator Sequence(){
        yield return new WaitForSeconds(0.4f);
        if (finished == 0){
            // Nightlander 1
            // That’s not right man, you can’t say that.
            if(triggered == 0 && !inBunker){
                audioSourceContainer.nightlanderOneVRSource.DearVRPlayOneShot(nightlanderOneClips[0]);
                yield return new WaitForSeconds(nightlanderOneClips[0].length);
            }

            // Nightlander 2
            // Hey, it’s a fact. Since when are facts offensive?
            if(triggered == 0 && !inBunker){
                audioSourceContainer.nightlanderTwoVRSource.DearVRPlayOneShot(nightlanderTwoClips[0]);
                yield return new WaitForSeconds(nightlanderTwoClips[0].length);
            }

            // Nightlander 1 
            // Whenever they come out of your mouth.
            if(triggered == 0 && !inBunker){
                audioSourceContainer.nightlanderOneVRSource.DearVRPlayOneShot(nightlanderOneClips[1]);
                yield return new WaitForSeconds(nightlanderOneClips[1].length);
            }

            // Nightlander 2
            // If you worship The Light, you’re a bad guy… simple.
            if(triggered == 0 && !inBunker){
                audioSourceContainer.nightlanderTwoVRSource.DearVRPlayOneShot(nightlanderTwoClips[1]);
                yield return new WaitForSeconds(nightlanderTwoClips[1].length);
            }

            // Nightlander 1
            // And the people of Dusklight, don’t they think we’re the bad guys?
            if(triggered == 0 && !inBunker){
                audioSourceContainer.nightlanderOneVRSource.DearVRPlayOneShot(nightlanderOneClips[2]);
                yield return new WaitForSeconds(nightlanderOneClips[2].length);
            }

            // Nightlander 3
            // Woah man, keep it down, don’t let Finn catch you speaking like that.
            if(triggered == 0 && !inBunker){
                audioSourceContainer.nightlanderThreeVRSource.DearVRPlayOneShot(nightlanderThreeClips[0]);
                yield return new WaitForSeconds(nightlanderThreeClips[0].length);
            }

            // Nightlander 1
            // I’m just saying; yeah, I think we’re doing the right thing and all, but that’s not to say all of them are doing wrong.
            if(triggered == 0 && !inBunker){
                audioSourceContainer.nightlanderTwoVRSource.DearVRPlayOneShot(nightlanderOneClips[3]);
                yield return new WaitForSeconds(nightlanderOneClips[3].length);
            }

            // Nightlander 3
            // We’re not politicians, we’re fighters. Let’s just stick to that. 
            if(triggered == 0 && !inBunker){
                audioSourceContainer.nightlanderThreeVRSource.DearVRPlayOneShot(nightlanderThreeClips[1]);
                yield return new WaitForSeconds(nightlanderThreeClips[1].length);
            }

            if (triggered == 1){
                audioSourceContainer.protagActionSource.PlayOneShot(cutsceneEnterClip);
                yield return new WaitForSeconds(cutsceneEnterClip.length);
                // Nightlander 1 addresses player. 
                // Nightlander 1
                // Hey, you, come take a seat. Free grub, that’s what we all came for right? 
                audioSourceContainer.nightlanderOneVRSource.DearVRPlayOneShot(nightlanderOneClips[4]);
                yield return new WaitForSeconds(nightlanderOneClips[4].length);
                if (MovedAwayChecker()){
                    StartCoroutine(MovedAway());
                    yield break;
                } else {   
                    StartCoroutine(SequenceTwo());
                }
            } else {
                yield return new WaitForSeconds(19f);
                StartCoroutine(Sequence());
            }
        }
    }

    IEnumerator SequenceTwo(){
        controls.inCutscene = true;
        musicSource.volume = 1;
        audioSourceContainer.guitarVRSource.DearVRStop();
        musicSource.PlayOneShot(musicClip, 0.07f);
        // Protag 
        // For you, maybe.
        audioSourceContainer.protagSource.PlayOneShot(protagClips[0]);
        yield return new WaitForSeconds(protagClips[0].length);

        audioSourceContainer.protagActionSource.PlayOneShot(sitDownClip, 0.5f);
        yield return new WaitForSeconds(sitDownClip.length);

        audioController.SetVolumeToNothing("Footsteps_Vol");
        audioSourceContainer.protagSource.transform.position = new Vector3(-63.3f, 0.3f, 51.2f);
        // Nightlander 2
        // We were just talking about the Church. Cal here thinks the moths ‘can’t all be that bad’. 
        audioSourceContainer.nightlanderTwoVRSource.DearVRPlayOneShot(nightlanderTwoClips[2]);
        yield return new WaitForSeconds(nightlanderTwoClips[2].length);

        // Protag
        // Moths? 
        audioSourceContainer.protagSource.PlayOneShot(protagClips[1]);
        yield return new WaitForSeconds(protagClips[1].length); 

        // Nightlander 1
        // Yeah, you not heard that already? You’ve been here for like two months man, what have you been doing? 
        audioSourceContainer.nightlanderOneVRSource.DearVRPlayOneShot(nightlanderOneClips[6]);
        yield return new WaitForSeconds(nightlanderOneClips[6].length);

        // Protag 
        // Good question. 
        audioSourceContainer.protagSource.PlayOneShot(protagClips[2]);
        yield return new WaitForSeconds(protagClips[2].length);

        // Nightlander 1
        // Moths are the people of Dusklight… They worship the light…
        audioSourceContainer.nightlanderOneVRSource.DearVRPlayOneShot(nightlanderOneClips[7]);
        yield return new WaitForSeconds(nightlanderOneClips[7].length);

        // Nightlander 2
        // I’m sure you can figure it out. 
        audioSourceContainer.nightlanderTwoVRSource.DearVRPlayOneShot(nightlanderTwoClips[3]);
        yield return new WaitForSeconds(nightlanderTwoClips[3].length);

        // Nightlander 3
        // Listen, I’m just saying, you get told that something’s right all your life, takes a whole lotta thinking to think otherwise. 
        audioSourceContainer.nightlanderThreeVRSource.DearVRPlayOneShot(nightlanderThreeClips[2]);
        yield return new WaitForSeconds(nightlanderThreeClips[2].length);

        // Nightlander 1
        // Yeah, like if anyone actually praised you for being a good agent, you might start believing it. 
        audioSourceContainer.nightlanderOneVRSource.DearVRPlayOneShot(nightlanderOneClips[7]);
        yield return new WaitForSeconds(nightlanderOneClips[7].length);

        ambienceRepeater.StopAllSources();

        // Protag
        // Well, I may have come for the free food, but I didn’t come for conversation. 
        audioSourceContainer.protagSource.PlayOneShot(protagClips[3]);
        yield return new WaitForSeconds(protagClips[3].length + 1f);

        // Suddenly, the conversation takes a stark turn as Nightlander 5, who hasn’t said anything at this point, suddenly speaks in a bitter tone.

        audioSourceContainer.nightlanderFourSource.gameObject.SetActive(true);
        // Nightlander 4
        // We all know why you came. 
        audioSourceContainer.nightlanderFourVRSource.DearVRPlayOneShot(nightlanderFourClips[0]);
        yield return new WaitForSeconds(nightlanderFourClips[0].length);

        // The audio dulls down as people stop eating from feeling awkward, subtle ‘tinks’ fade as people stop. 
        audioSourceContainer.tableVRSource.DearVRPlayOneShot(awkwardCutleryClip);
        yield return new WaitForSeconds(awkwardCutleryClip.length);

        // Protag
        // You don’t know a single thing about me.
        audioSourceContainer.protagSource.PlayOneShot(protagClips[4]);
        yield return new WaitForSeconds(protagClips[4].length);

        audioSourceContainer.nightlanderFourSource.gameObject.SetActive(true);
        // Nightlander 5
        // Exactly… exactly. Now, everyone here is very explicit as to why they joined, the people of Dusklight need to be freed. But you, you haven’t been explicit about anything. Ulterior motives, that’s all I need to know. 
        audioSourceContainer.nightlanderFourVRSource.DearVRPlayOneShot(nightlanderFourClips[1]);
        yield return new WaitForSeconds(nightlanderFourClips[1].length);

        // Protag
        // You want to know my story? Why I’m here? That church stole everything from me. The End. 
        audioSourceContainer.protagSource.PlayOneShot(protagClips[5]);
        yield return new WaitForSeconds(protagClips[5].length);
        // The protagonist stands up violently, spilling things and knocking plates to the floor. One Nightlander can be heard whispering in frustration of the Nightlander that spoke up. 
        audioSourceContainer.protagActionSource.PlayOneShot(slamAndGetUpClip, 0.4f);
        yield return new WaitForSeconds(0.2f);
        musicSource.Stop();
        yield return new WaitForSeconds(slamAndGetUpClip.length - 1.2f);
        // This is just the prologue, brother.  
        audioSourceContainer.protagSource.PlayOneShot(protagClips[6]);
        yield return new WaitForSeconds(protagClips[6].length);
        Finished();
    }

    void Finished(){
        controls.inCutscene = false;
        audioSourceContainer.protagActionSource.PlayOneShot(cutsceneExitClip);
        finished = 1;
        ambienceRepeater.PlayAllSources();
        saveAndLoadEncampment.SaveEncampment();
        audioController.SetVolumeToZero("Footsteps_Vol");
        auditoryZoomSequence.CheckIfShouldStart();
        audioSourceContainer.nightlanderTwoSource.gameObject.SetActive(false);
        audioSourceContainer.nightlanderThreeSource.gameObject.SetActive(false);
        audioSourceContainer.nightlanderFourSource.gameObject.SetActive(false);
        audioSourceContainer.guitarVRSource.DearVRPlay();
        StartCoroutine(PlayAfterScene());
    }

    IEnumerator PlayAfterScene(){
        if (Vector3.Distance(audioSourceContainer.nightlanderOneSource.transform.position, 
        audioSourceContainer.protagSource.transform.position) > 15f){
            audioSourceContainer.nightlanderOneVRSource.DearVRPlayOneShot(nightlanderOneClips[9]);
            yield return new WaitForSeconds(nightlanderOneClips[9].length);
            audioSourceContainer.nightlanderOneSource.gameObject.SetActive(false);
            yield break;
        } else {
            yield return new WaitForSeconds(1f);
            StartCoroutine(PlayAfterScene());
        }
        
    }
}
