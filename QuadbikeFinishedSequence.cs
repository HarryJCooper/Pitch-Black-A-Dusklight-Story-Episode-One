using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuadbikeFinishedSequence : SequenceBase
{
    [SerializeField] AudioSourceContainer audioSourceContainer;
    [SerializeField] AudioClip[] protagClips, donnieClips, donnieLoopClips, loopClips, quadbikeLoopClips;
    [SerializeField] AudioClip mysteriousVoiceClip, getOnQuadbikeClip, quadbikeAcceleratingClip;
    [SerializeField] AudioController audioController;
    [SerializeField] SaveAndLoadEncampment saveAndLoadEncampment;
    [SerializeField] Controls controls;
    [SerializeField] PBFootstepSystem pBFootstepSystem;
    bool startedLoopRevs;
    int i = 0;

    void Awake(){active = 0;}

    IEnumerator LoopRevs(){
        if (finished == 1) yield break;
        startedLoopRevs = true;
        audioSourceContainer.donnieSource.PlayOneShot(quadbikeLoopClips[Random.Range(0, quadbikeLoopClips.Length)]);
        yield return new WaitForSeconds(quadbikeLoopClips[0].length/2);
        StartCoroutine(LoopRevs());
    }

    public override IEnumerator Sequence(){
        yield return new WaitForSeconds(0.5f);
        if (finished == 0 && active == 1){
            if (triggered == 1){
                audioSourceContainer.protagSource.PlayOneShot(cutsceneEnterClip);
                yield return new WaitForSeconds(cutsceneEnterClip.length);
                // Player walks to Donnie to get the Quadbike 
                // Donnie 
                // Hey loudmouth! Quadbike’s ready; she’s fitted with a nav device, it’ll point you in the right direction just do what she says, know that might be hard for you. 
                audioSourceContainer.donnieSource.PlayOneShot(donnieClips[0]);
                yield return new WaitForSeconds(donnieClips[0].length);
                controls.inCutscene = true;
                pBFootstepSystem.canRotate = false;

                // Protag mutters under their breath. 
                // Protag 
                // Idiot.
                audioSourceContainer.protagSource.PlayOneShot(protagClips[0]);
                yield return new WaitForSeconds(protagClips[0].length);

                // Donnie replies in an awkward tone. 
                // Donnie 
                // Yeah, I heard that, again… I heard that again. Dammit. 
                audioSourceContainer.donnieSource.PlayOneShot(donnieClips[1]);
                yield return new WaitForSeconds(donnieClips[1].length);

                // Player then follows the sound of the nearby engine, maybe a car unlocked beep triggers too, and interacts to get on it. They then drive to the objective using the sat nav audio prompts. The player reaches a point, and the protag speaks to denote a fade. 
                // Protag 
                // Alright, got a while till the pumping station, then I gotta follow the rest of his trail. This place better be worth it… 
                audioSourceContainer.protagSource.PlayOneShot(getOnQuadbikeClip);
                yield return new WaitForSeconds(getOnQuadbikeClip.length);
                audioSourceContainer.protagSource.PlayOneShot(quadbikeAcceleratingClip);
                yield return new WaitForSeconds(5f);
                audioSourceContainer.protagSource.PlayOneShot(protagClips[1]);
                yield return new WaitForSeconds(protagClips[1].length);

                audioController.ReduceMasterCutOff(7.5f);
                yield return new WaitForSeconds(7.5f);
                // Sonic alterations suggest a time warp, or at least some sort of vision into the past. The player is then subject to a mysterious, well-spoken voice who appears to be reciting something. After the voice has finished, the same sounds signify a return to reality. These sequences repeat at the end of each level but with different segments being read out.
                // Mysterious Voice
                // ‘The Heavens are in turmoil; the almighty has awakened from his repose. He has deployed legions of angels throughout the world who only wait for his signal to do his will.’ I, my friend, am one of those angels, and his signal was signed long ago.’
                audioSourceContainer.protagSource.PlayOneShot(mysteriousVoiceClip);
                yield return new WaitForSeconds(mysteriousVoiceClip.length);
                Finished();
            } else {
                if (!startedLoopRevs) StartCoroutine(LoopRevs());
                audioSourceContainer.donnieSource.PlayOneShot(donnieLoopClips[i]);
                yield return new WaitForSeconds(15f);
                if (i < 2) i += 1; else i = 0;
                StartCoroutine(Sequence());
            }
        }
    }

    void Finished(){
        finished = 1;
        saveAndLoadEncampment.SaveEncampment();
        saveAndLoadEncampment.FinishedEncampment();
        SceneManager.LoadScene("ThePumpingStation");
    }
}
