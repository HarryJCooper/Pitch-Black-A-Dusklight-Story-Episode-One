using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public int playerScore;
    public int computerScore;
    public int maxScore = 5;

    public AudioSource announcer;

    public AudioClip pOne;
    public AudioClip pTwo;
    public AudioClip pThree;
    public AudioClip pFour;
    public AudioClip pFive;
    public AudioClip cOne;
    public AudioClip cTwo;
    public AudioClip cThree;
    public AudioClip cFour;
    public AudioClip cFive;
    public AudioClip playerWins;
    public AudioClip computerWins;

    public bool pOneHasPlayed;
    public bool pTwoHasPlayed;
    public bool pThreeHasPlayed;
    public bool pFourHasPlayed;
    public bool pFiveHasPlayed;
    public bool cOneHasPlayed;
    public bool cTwoHasPlayed;
    public bool cThreeHasPlayed;
    public bool cFourHasPlayed;
    public bool cFiveHasPlayed;

    public GameObject ball;
    public float topBounds;
    public float bottomBounds;

    public bool justScored = true;

    void Start()
    {
        playerScore = 0;
        computerScore = 0;
    }

    

    void Update()
    {
        // PLAYER SCORE ANNOUNCEMENTS
        {
            if (playerScore == 1 && !pOneHasPlayed)
            {
                announcer.PlayOneShot(pOne);
                pOneHasPlayed = true;
            }
            if (playerScore == 2 && !pTwoHasPlayed)
            {
                announcer.PlayOneShot(pTwo);
                pTwoHasPlayed = true;
            }
            if (playerScore == 3 && !pThreeHasPlayed)
            {
                announcer.PlayOneShot(pThree);
                pThreeHasPlayed = true;
            }
            if (playerScore == 4 && !pFourHasPlayed)
            {
                announcer.PlayOneShot(pFour);
                pFourHasPlayed = true;
            }
            if (playerScore == 5 && !pFiveHasPlayed)
            {
                announcer.PlayOneShot(pFive);
                pFiveHasPlayed = true;
            }
            if (playerScore == maxScore)
            {
                announcer.PlayOneShot(playerWins);
            }
        }

        // COMPUTER SCORE ANNOUNCEMENTS
        {
            if (computerScore == 1 && !cOneHasPlayed)
            {
                announcer.PlayOneShot(cOne);
                cOneHasPlayed = true;
            }
            if (computerScore == 2 && !cTwoHasPlayed)
            {
                announcer.PlayOneShot(cTwo);
                cTwoHasPlayed = true;
            }
            if (computerScore == 3 && !cThreeHasPlayed)
            {
                announcer.PlayOneShot(cThree);
                cThreeHasPlayed = true;
            }
            if (computerScore == 4 && !cFourHasPlayed)
            {
                announcer.PlayOneShot(cFour);
                cFourHasPlayed = true;
            }
            if (computerScore == 5 && !cFiveHasPlayed)
            {
                announcer.PlayOneShot(cFive);
                cFiveHasPlayed = true;
            }
            if (computerScore == maxScore)
            {
                announcer.PlayOneShot(computerWins);
            }
        }

       

        if (ball.transform.localPosition.z > topBounds)
        {
            playerScore += 1;
            justScored = true;
        }
        if (ball.transform.localPosition.z < bottomBounds)
        {
            computerScore += 1;
            justScored = true;
        }



    }
}
