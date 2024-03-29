using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretSequence : SequenceBase
{
    [SerializeField] AudioSourceContainer audioSourceContainer;
    public AudioClip[] finnClips, milesClips, protagClips;
    [SerializeField] SaveAndLoadEncampment saveAndLoadEncampment;
    [SerializeField] OcclusionObject doorObject;
    [SerializeField] GameObject bunkerObject;
    
    // The player can walk back towards Finn’s office. If they do, they hear a secret conversation between Finn and an inaudible friend. The convo is low passed. 
    public override IEnumerator Sequence(){
        yield return new WaitForSeconds(0.7f);
        if (finished == 0 && active == 1){
            if (triggered == 1 ){
                doorObject.volumeReduction = 10;
                doorObject.frequencyReduction = 21000;
                Vector3 finnVector3 = new Vector3(
                    audioSourceContainer.finnSource.gameObject.transform.position.x,
                    audioSourceContainer.finnSource.gameObject.transform.position.y,
                    audioSourceContainer.finnSource.gameObject.transform.position.z
                );
                audioSourceContainer.finnSource.gameObject.transform.position = new Vector3(0, 0.3f, 10f);
                audioSourceContainer.milesSource.gameObject.SetActive(true);
                audioSourceContainer.protagActionSource.PlayOneShot(cutsceneEnterClip);
                yield return new WaitForSeconds(cutsceneEnterClip.length);
                // Finn 
                // Yeah, and I don’t trust him either, but he’s capable. More than any of us, we can’t go on like we have been, Miles. 
                audioSourceContainer.finnSource.PlayOneShot(finnClips[0]);
                yield return new WaitForSeconds(finnClips[0].length);

                // Miles 
                // Well, we can’t let him run the show, that’s for sure. I’ll be watching him. 
                audioSourceContainer.milesSource.PlayOneShot(milesClips[0]);
                yield return new WaitForSeconds(milesClips[0].length);
                audioSourceContainer.milesSource.gameObject.SetActive(false);

                // Finn 
                // Listen, I don’t want to hear it… He’s gonna be meeting Archie, if anything happens, he’ll let us know, and we’ll have got what we wanted. That’s the end of it. 
                audioSourceContainer.finnSource.PlayOneShot(finnClips[1]);
                yield return new WaitForSeconds(finnClips[1].length);
                audioSourceContainer.finnSource.gameObject.transform.position = finnVector3;

                // The protag’s tone is distrust worthy for the first sentence, and then cunning for the second. 
                // Protag 
                // The first lesson I ever learned; trust is always temporary. 
                audioSourceContainer.protagSource.PlayOneShot(protagClips[0]);
                yield return new WaitForSeconds(protagClips[0].length);
                Finished();
            } else {
                yield return new WaitForSeconds(0.7f);
                StartCoroutine(Sequence());
            }
        }
    }

    void Finished(){
        bunkerObject.SetActive(false);
        audioSourceContainer.protagActionSource.PlayOneShot(cutsceneExitClip);
        
        finished = 1;
        saveAndLoadEncampment.SaveEncampment();
    }

    void Start(){active = 0;}
}
