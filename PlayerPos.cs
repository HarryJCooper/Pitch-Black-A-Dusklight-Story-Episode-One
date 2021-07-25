using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PlayerPos : MonoBehaviour
{
    public GameObject introduction;
    public GameMaster gm;
    public AudioMixerSnapshot Standard;
    public float timeToReach;


    // Start is called before the first frame update
    void Start()
    {
        // gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        // transform.position = gm.lastCheckPointPos;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (introduction)
            {
                introduction.SetActive(false);
            }
            transform.position = gm.lastCheckPointPos;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            Standard.TransitionTo(timeToReach);
        }
    }
}
