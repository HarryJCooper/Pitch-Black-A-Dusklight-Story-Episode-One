using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicSequence : SequenceBase
{
    // Maybe it finds repeating ambience on the AudioSource and cuts it?

    public AudioSourceContainer audioSourceContainer;
    public AudioClip[] protagClips, donnieClips, loopClips;
    [SerializeField] AuditoryZoomSequence auditoryZoomSequence;
    [SerializeField] SaveAndLoadEncampment saveAndLoadEncampment;
    
    //   For when the player finds the mechanic, instructed by Finn, but the vehicle isn’t ready quite yet. The mechanic informs the player of this. He is fixing things in his shed. Donnie sounds like an aggressive, OCD riddled man; stressed at every turn and somewhat pathetic because of it. 
    public override IEnumerator Sequence(){  
        yield return new WaitForSeconds(0.6f);
        if (finished == 0){
            if (triggered == 1){
                audioSourceContainer.protagSource.PlayOneShot(cutsceneEnterClip);
                yield return new WaitForSeconds(cutsceneEnterClip.length);
                // Protag 
                // Hey… HEY! 
                audioSourceContainer.protagSource.PlayOneShot(protagClips[0]);
                yield return new WaitForSeconds(protagClips[0].length);

                // Mechanic shop loop should cut here, should still have some shed ambience. 
                // Donnie
                // What?!
                audioSourceContainer.donnieSource.PlayOneShot(donnieClips[0]);
                yield return new WaitForSeconds(donnieClips[0].length);

                // Protag
                // Finn sent me, said the quad’s ready to go.
                audioSourceContainer.protagSource.PlayOneShot(protagClips[1]);
                yield return new WaitForSeconds(protagClips[1].length);

                // Donnie
                // Oh yeah? well it ain’t! These things take time you know. You think quadbikes grow on trees man?
                audioSourceContainer.donnieSource.PlayOneShot(donnieClips[1]);
                yield return new WaitForSeconds(donnieClips[1].length);

                // Protag
                // No.
                audioSourceContainer.protagSource.PlayOneShot(protagClips[2]);
                yield return new WaitForSeconds(protagClips[2].length);

                // Donnie 
                // Exactly, even if trees did still exist, I’d be surprised if fully functional quadbikes grew on them. Then again, I don’t know for sure… j-just go wait somewhere else and I’ll call you when it's ready. 
                audioSourceContainer.donnieSource.PlayOneShot(donnieClips[2]);
                yield return new WaitForSeconds(donnieClips[2].length);

                // Protag
                // Try not to hurt yourself. 
                audioSourceContainer.protagSource.PlayOneShot(protagClips[3]);
                yield return new WaitForSeconds(protagClips[3].length);

                // Donnie
                // I heard that!
                audioSourceContainer.donnieSource.PlayOneShot(donnieClips[3]);
                yield return new WaitForSeconds(donnieClips[3].length);

                // Protag utters to himself. 
                // Protag 
                // No wonder the Nightlanders aren’t getting anywhere, good job I won't be needing them for long.   
                audioSourceContainer.protagSource.PlayOneShot(protagClips[4]);
                yield return new WaitForSeconds(protagClips[4].length);

                Finished();
            } else {
                audioSourceContainer.donnieSource.PlayOneShot(loopClips[Random.Range(0, loopClips.Length)]);
                yield return new WaitForSeconds(16f);
                StartCoroutine(Sequence());
            }
        }
        yield return new WaitForSeconds(0.1f);
    }

    void Finished(){
        audioSourceContainer.protagSource.PlayOneShot(cutsceneExitClip);
        finished = 1;
        auditoryZoomSequence.CheckIfShouldStart();
        saveAndLoadEncampment.SaveEncampment();
    }
}
