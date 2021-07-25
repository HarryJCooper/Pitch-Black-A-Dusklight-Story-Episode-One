using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LeaveAudioSignal : MonoBehaviour
{
    public bool turnedOn;
    public GameObject AudioCue1;
    public GameObject AudioCue2;
    public GameObject AudioCue3;
    public GameObject AudioCue4;
    public GameObject AudioCue5;
    public Transform Player;
    public Transform audioCue1Trans;
    public Transform audioCue2Trans;
    public Transform audioCue3Trans;
    public Transform audioCue4Trans;
    public Transform audioCue5Trans;
    public AudioMixer AudioSignPosts;
    public float newVolume;
    public float initialVolume;
    public float transitionSpeed;
    public string PlayerMechanics;
    public bool Reduced;
    public float timeToWait;
    public float timer1 = 3.0f;
    public float timer2 = 3.0f;
    public float timer3 = 3.0f;
    public float timer4 = 3.0f;
    public float timer5 = 3.0f;
    public AudioSource audioCue1;
    public AudioSource audioCue2;
    public AudioSource audioCue3;
    public AudioSource audioCue4;
    public AudioSource audioCue5;

    void Update()
    {
        if (turnedOn)
        {
            // Timers
            if (Input.GetKey(KeyCode.Alpha1))
            {
                timer1 -= Time.deltaTime;
                if (timer1 < 0)
                {
                    AudioCue1.SetActive(false);
                }
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                timer2 -= Time.deltaTime;
                if (timer2 < 0)
                {
                    AudioCue2.SetActive(false);
                }
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                timer3 -= Time.deltaTime;
                if (timer3 < 0)
                {
                    AudioCue3.SetActive(false);
                }
            }

            if (Input.GetKey(KeyCode.Alpha4))
            {
                timer4 -= Time.deltaTime;
                if (timer4 < 0)
                {
                    AudioCue4.SetActive(false);
                }
            }

            if (Input.GetKey(KeyCode.Alpha5))
            {
                timer5 -= Time.deltaTime;
                if (timer5 < 0)
                {
                    AudioCue5.SetActive(false);
                }
            }


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                LeaveCue1();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                LeaveCue2();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                LeaveCue3();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                LeaveCue4();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                LeaveCue5();
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                ReduceVol();
            }
        }
    }

    void ReduceVol()
    {
        if (!Reduced)
        {
            AudioSignPosts.SetFloat(PlayerMechanics, Mathf.Lerp(initialVolume, newVolume, transitionSpeed));
            StartCoroutine(Wait());
        }
        if (Reduced)
        {
            AudioSignPosts.SetFloat(PlayerMechanics, Mathf.Lerp(newVolume, initialVolume, transitionSpeed));
            Reduced = false;
        }

    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(timeToWait);
        Reduced = true;
    }

    void LeaveCue1()
    {
        timer1 = 3;
        AudioCue1.SetActive(true);
        audioCue1Trans.position = Player.position;
        audioCue1.Play();
    }
    void LeaveCue2()
    {
        timer2 = 3;
        AudioCue2.SetActive(true);
        audioCue2Trans.position = Player.position;
        audioCue2.Play();
    }
    void LeaveCue3()
    {
        timer3 = 3;
        AudioCue3.SetActive(true);
        audioCue3Trans.position = Player.position;
        audioCue3.Play();
    }
    void LeaveCue4()
    {
        timer4 = 3;
        AudioCue4.SetActive(true);
        audioCue4Trans.position = Player.position;
        audioCue4.Play();
    }
    void LeaveCue5()
    {
        timer5 = 3;
        AudioCue5.SetActive(true);
        audioCue5Trans.position = Player.position;
        audioCue5.Play();
    }
}
