using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ChangeAfterFocus : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioSource audioSource;
    public bool hit, isPlaying, hasPlayed, reachHasPlayed, playedOneShot, hasInitialVolume;
    public AudioClip audioClip, reachAudio, hitPOI, unhitPOI;
    public float timeToReturn, newVolume, initialVolume, hitVolume, waitTime, waitTimeFade, currentVolume, volumeIncreaser;
    public string POI, fadingObject;
    public GameObject player;
    float transitionTimer;
    public bool hitPOIPlayed, unhitPOIPlayed, hasFocused, turnedUp, backedUp;
    public PBFootstepSystem pBFootstepSystem;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioMixer = audioSource.outputAudioMixerGroup.audioMixer;
        isPlaying = false;
        hasPlayed = false;
        POI = audioSource.outputAudioMixerGroup.name + "_HitVol";
    }
    
    
    void Update()
    {
        audioMixer.GetFloat(audioSource.outputAudioMixerGroup.name + "_InitialVol", out initialVolume);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hasFocused = true;
        }

        Focus();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            hit = false;
        }
        
        /*
        if (hit)
        {
            PointOfInterest.SetFloat(POI, Mathf.Lerp(newVolume, initialVolume, transitionSpeed));
            if (!reachHasPlayed && !hasPlayed && !playedOneShot)
            {
                if (audioClip != null)
                {
                    audioSource.PlayOneShot(audioClip);
                }
                isPlaying = true;
                audioSource.loop = true;
                playedOneShot = true;
            }
        }
        else
        {
            PointOfInterest.SetFloat(POI, Mathf.Lerp(initialVolume, newVolume, transitionSpeed));
        }

        if (!Input.GetKey(KeyCode.Space))
        {
            hit = false;
        }
        

        if (isPlaying)
        {
            hasPlayed = true;
        }

        if (!audioSource.isPlaying && reachHasPlayed)
        {
            StartCoroutine(Wait());         
        }
        */
    }

    #region When Reached

    void OnTriggerEnter()
    {
        FadeOut();
        
        audioSource.loop = false;
        if (audioSource.isPlaying)
        {
            reachHasPlayed = true;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        hasPlayed = false;
        reachHasPlayed = false;
    }

    void FadeOut()
    {
        audioMixer.SetFloat(POI, Mathf.Lerp(initialVolume, -80.0f, 1));
        StartCoroutine(PlayAfterFade());
    }

    IEnumerator PlayAfterFade()
    {
        if (!reachHasPlayed)
        {
            yield return new WaitForSeconds(waitTimeFade);
            audioSource.Stop();
            audioMixer.SetFloat(POI, Mathf.Lerp(-80.0f, initialVolume, 1));
            if (reachAudio != null)
            {
                audioSource.PlayOneShot(reachAudio);
            }
        }
    }

    #endregion

    void Focus()
    {
        if (pBFootstepSystem.canMove)
        {
            hitVolume = initialVolume * -1;

            if (hit)
            {
                transitionTimer = 0;
                if (!hitPOIPlayed)
                {
                    audioSource.PlayOneShot(hitPOI);
                    Debug.Log("played HitPOI");
                    hitPOIPlayed = true;
                    unhitPOIPlayed = false;
                }
                
                volumeIncreaser += Time.deltaTime;
                audioMixer.SetFloat(POI, Mathf.Lerp(0, hitVolume, volumeIncreaser)); 
            }

            if (!hit && !unhitPOIPlayed && hasFocused)
            {
                hitPOIPlayed = false;
                audioSource.PlayOneShot(unhitPOI);
                unhitPOIPlayed = true;
            }

            if (currentVolume > initialVolume && !hit)
            {
                volumeIncreaser = 0;

                hitPOIPlayed = false;

                if (!unhitPOIPlayed && hasFocused)
                {
                    Debug.Log("played UnHitPOI");
                    audioSource.PlayOneShot(unhitPOI);
                    unhitPOIPlayed = true;
                }
                if (transitionTimer <= 1)
                {
                    transitionTimer += Time.deltaTime;
                }
                audioMixer.SetFloat(POI, Mathf.Lerp(hitVolume, 0, transitionTimer));
            }
        }
    }
}

