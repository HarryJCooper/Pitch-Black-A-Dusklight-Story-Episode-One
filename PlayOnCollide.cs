using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnCollide : MonoBehaviour
{
    public AudioSource Megapede;
    public AudioClip Ambience;



    // Start is called before the first frame update
    void Start()
    {
        Megapede = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Megapede.clip)
            {
                Megapede.Play();
            }
            else
            {
                Megapede.PlayOneShot(Ambience);
            }
        }
    }
 }
