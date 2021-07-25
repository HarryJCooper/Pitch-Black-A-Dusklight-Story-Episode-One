using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DisplayLeaderboard : MonoBehaviour
{ /*
     // MOVED EVERYTHING TO CONVERT FROM JSON

    //public List<SaveObject> pongScores2;
    //public List<SaveObject> pongScores3 = new List<SaveObject>();
    public AudioClip[] scoreClips, alphabetClips;
    //public SaveObject[] scores;
    private AudioSource initAnnouncer;
    public int posInt = 1;

    void Start()
    {
        //GetLeaderboard();
        scoreClips = GameObject.Find("Controller").GetComponent<PongController>().endlessAnnouncer;
        alphabetClips = GameObject.Find("Leaderboard").GetComponent<Leaderboard>().alphabetClips;
        initAnnouncer = GameObject.Find("Leaderboard").GetComponent<Leaderboard>().initAnnouncer;
    }

    
    void GetLeaderboard()
    {

        OrderLeaderboard();
    }

    public void OrderLeaderboard()
    {
        pongScores3 = pongScores2.OrderByDescending(p => p.score).ToList();
    }

    public void PlayTopScores()
    {
        GameObject.Find("Leaderboard").GetComponent<Leaderboard>().playedLeaderboard = true;
        StartCoroutine(wait());
    }
    
    IEnumerator wait()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(play1stInit());
    }

    IEnumerator play1stInit()
    {
        Debug.Log(pongScores3[posInt].init1 - 1);
        initAnnouncer.PlayOneShot(alphabetClips[pongScores3[posInt].init1 - 1]);
        
        yield return new WaitForSeconds(alphabetClips[pongScores3[posInt].init1 - 1].length);
        StartCoroutine(play2ndInit());
    }
    IEnumerator play2ndInit()
    {
        initAnnouncer.PlayOneShot(alphabetClips[pongScores3[posInt].init2 - 1]);
        yield return new WaitForSeconds(alphabetClips[pongScores3[posInt].init2 - 1].length);
        StartCoroutine(play3rdInit());
    }
    IEnumerator play3rdInit()
    {
        initAnnouncer.PlayOneShot(alphabetClips[pongScores3[posInt].init3 - 1]);
        yield return new WaitForSeconds(alphabetClips[pongScores3[posInt].init3 - 1].length);
        StartCoroutine(playScore());
    }
    IEnumerator playScore()
    {
        initAnnouncer.PlayOneShot(scoreClips[pongScores3[posInt].score]);
        yield return new WaitForSeconds(1);
        if (posInt < 5)
        {
            posInt += 1;
            StartCoroutine(play1stInit());
        }
        else
        {
            GameObject.Find("Leaderboard").GetComponent<Leaderboard>().playedLeaderboard = false;
            Debug.Log("finished leaderboard");
            posInt = 0;
        }
    }



    public class SaveObject
    {
        public int time, init1, init2, init3, score;
    }

    /* public string scoreString()
    {
        string saveString = File.ReadAllText("/score");

        return saveString;
    }
    */


}

