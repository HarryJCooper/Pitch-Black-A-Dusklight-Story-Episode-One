using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunkerIntroSequence : MonoBehaviour
{
    
    public float finnMoveTime, finnMoveMulti;
    public AudioSource nonSpatial, directPlayerDoor;
    public AudioSource finnDialogue0, finnFootsteps0, finnDialogue1, finnFootsteps1, finnTable, finnTable2, finnDoorBang;
    public AudioClip[] dialogue;
    public AudioClip whistleLoop;
    public AudioClip whistleIntro;
    public AudioClip cutsceneSound, finishedCutscene;
    public AudioClip introMusic;
    public int dialogueCounter, footstepCounter;
    public bool stopIntroLoop, cutscene1, cutscene2, cutscene3, cutscene4, cutscene5, cutscene6, cutscene7, introSkipper, finnMoving, playerTriedToLeave, hasFinishedCutscene;
    public AudioClip[] xSubscene1, xSubscene2, xSubscene3, xSubscene4, malfunctions, finnFootClips;
    public Transform Finn0, Finn1;
    public Beacon beacon;
    public Transform player;
    public PBFootstepSystem pBFootstepSystem;
    public GameObject finnIntro;
    public KillTheIntro killTheIntro;


    void Start()
    {
        if (!introSkipper)
        {
            nonSpatial.PlayOneShot(introMusic);
            StartCoroutine(waitForWhistle());
        }
        else
        {
            dialogueCounter = 1;
        }
    }
    
    private void Update()
    {
        if (dialogueCounter == 0)
        {
            cutscene1 = false;
        }
        else if (dialogueCounter == 1)
        {
            cutscene1 = true;
        }
        else if (dialogueCounter == 2 && Input.anyKeyDown && !cutscene2)
        {
            cutscene1 = false;
            cutscene2 = true;
            StartCoroutine(FinnDialogue());
            pBFootstepSystem.canMove = true;
        }
        else if (dialogueCounter == 3 && Input.anyKeyDown && !cutscene3)
        {
            cutscene2 = false;
            cutscene3 = true;
            StartCoroutine(FinnDialogue());
            pBFootstepSystem.canMove = true;
        }

        else if (dialogueCounter == 4 && Input.GetKeyDown(KeyCode.Alpha1) && !cutscene4)
        {
            nonSpatial.PlayOneShot(malfunctions[0], 0.6f);
            cutscene3 = false;
            cutscene4 = true;
            StartCoroutine(FinnDialogue());
            pBFootstepSystem.canMove = false;
        }

        else if (dialogueCounter == 5 && Input.GetKeyDown(KeyCode.I) && !cutscene5)
        {
            nonSpatial.PlayOneShot(malfunctions[1], 0.6f);
            cutscene4 = false;
            cutscene5 = true;
            StartCoroutine(FinnDialogue());
            pBFootstepSystem.canMove = false;
        }
        else if (dialogueCounter == 6 && Input.GetKeyDown(KeyCode.Space) && !cutscene6)
        {
            cutscene5 = false;
            cutscene6 = true;
            StartCoroutine(FinnDialogue());
        }
        else if (dialogueCounter == 7 && !Input.GetKey(KeyCode.Space) && !cutscene7)
        {
            cutscene6 = false;
            cutscene7 = true;
            StartCoroutine(FinnDialogue());
        }
        if (!finnDialogue1.isPlaying && cutscene7 && !hasFinishedCutscene)
        {
            hasFinishedCutscene = true;
            nonSpatial.PlayOneShot(finishedCutscene);
        }

        if (finnMoving)
        {
            if (finnMoveTime < 1)
            {
                if (footstepCounter == 0)
                {
                    FinnWalk0();
                    finnMoveTime += Time.deltaTime * 0.4f;
                }
                if (footstepCounter == 1)
                {
                    FinnWalk1();
                    finnMoveTime += Time.deltaTime * 0.4f;
                }
                if (footstepCounter == 2)
                {
                    FinnWalk2();
                    finnMoveTime += Time.deltaTime * 0.18f;
                }
                if (footstepCounter == 3)
                {
                    FinnWalk3();
                    finnMoveTime += Time.deltaTime * 0.4f;
                }
                if (footstepCounter == 4)
                {
                    FinnWalk4();
                }
                if (footstepCounter == 5)
                {
                    FinnWalk4();
                }
            }
            else
            {
                finnMoveTime = 1;
                finnMoving = false;
            }
        }
    }

    IEnumerator waitForWhistle()
    {
        yield return new WaitForSeconds(31.852f);
        // Finn Whistle and Ambience Fades In
        finnDialogue0.PlayOneShot(whistleIntro, 0.4f);
        yield return new WaitForSeconds(whistleIntro.length + 2.6f);
        // Player Follows Finn's Whistle Through Corridor
        StartCoroutine(FinnDialogueIntro());
        StartCoroutine(FinnWalking());
        StartCoroutine(ExtraClip1());
    }


    public IEnumerator FinnDialogue()
    {
        if (cutscene1)
        {
            Debug.Log("Start Cutscene 1" + dialogueCounter);

            pBFootstepSystem.canMove = false;
            yield return new WaitForSeconds(.4f);
            nonSpatial.PlayOneShot(cutsceneSound);
            yield return new WaitForSeconds(.4f);
            finnDialogue1.PlayOneShot(dialogue[dialogueCounter]);
            StartCoroutine(ExtraClip2());
        }
        else if (cutscene2)
        {
            Debug.Log("Start Cutscene 2" + dialogueCounter);
            yield return new WaitForSeconds(0.4f);
            nonSpatial.PlayOneShot(cutsceneSound);
            pBFootstepSystem.canMove = false;
            yield return new WaitForSeconds(0.5f);
            finnDialogue1.PlayOneShot(dialogue[dialogueCounter]);
            StartCoroutine(ExtraClip3());
        }
        else if (cutscene3)
        {
            Debug.Log("Start Cutscene 3" + dialogueCounter);
            yield return new WaitForSeconds(0.4f);
            pBFootstepSystem.canMove = false;
            nonSpatial.PlayOneShot(cutsceneSound);
            yield return new WaitForSeconds(0.5f);
            finnDialogue1.PlayOneShot(dialogue[dialogueCounter]);
            StartCoroutine(ExtraClip4());
        }
        else if (cutscene4)
        {
            Debug.Log("Start Cutscene 4" + dialogueCounter);
            yield return new WaitForSeconds(0.4f);
            nonSpatial.PlayOneShot(cutsceneSound);
            pBFootstepSystem.canMove = false;
            yield return new WaitForSeconds(0.5f);
            finnDialogue1.PlayOneShot(dialogue[dialogueCounter]);
        }
        else if (cutscene5)
        {
            Debug.Log("Start Cutscene 5" + dialogueCounter);
            yield return new WaitForSeconds(0.5f);
            pBFootstepSystem.canMove = false;
            finnDialogue1.PlayOneShot(dialogue[dialogueCounter]);
        }
        else if (cutscene6)
        {
            Debug.Log("Start Cutscene 6" + dialogueCounter);
            yield return new WaitForSeconds(0.5f);
            pBFootstepSystem.canMove = false;
            finnDialogue1.PlayOneShot(dialogue[dialogueCounter]);
        }
        else if (cutscene7)
        {
            Debug.Log("Start Cutscene 7" + dialogueCounter);
            finnDialogue1.PlayOneShot(dialogue[dialogueCounter]);
            beacon.turnedOn = true;
            pBFootstepSystem.canMove = true;
        }
        yield return new WaitForSeconds(dialogue[dialogueCounter].length);
        
        yield return new WaitForSeconds(0.1f);
        dialogueCounter += 1;
        
    }

    public IEnumerator FinnDialogueIntro()
    {
        if (!finnDialogue1.isPlaying)
        {
            Debug.Log("Start Finn Dialogue Intro");
            finnDialogue0.PlayOneShot(dialogue[dialogueCounter]);
        }
        yield return new WaitForSeconds(dialogue[dialogueCounter].length);
        Debug.Log("Finished Finn Dialogue Intro");

    }

    #region
    public IEnumerator FinnWalking()
    {
        yield return new WaitForSeconds(15.064f);
        if (!finnDialogue1.isPlaying)
        {
            Debug.Log("Finn Walking - 0");

            finnFootsteps0.PlayOneShot(finnFootClips[0]);
            finnMoving = true;
            footstepCounter = 0;
            finnMoveTime = 0;
        }
        else
        {
            StopCoroutine(FinnWalking());
            Debug.Log("Stopped Finn Walking - 0");
        }
        yield return new WaitForSeconds(22.00f);
        if (!finnDialogue1.isPlaying)
        {
            Debug.Log("Finn Walking - 1");
            finnFootsteps0.PlayOneShot(finnFootClips[1]);
            finnMoving = true;
            footstepCounter = 1;
            finnMoveTime = 0;
        }
        else
        {
            StopCoroutine(FinnWalking());
            Debug.Log("Stopped Finn Walking - 1");
        }
        yield return new WaitForSeconds(17.00f);
        if (!finnDialogue1.isPlaying)
        {
            Debug.Log("Finn Walking - 2");
            finnFootsteps0.PlayOneShot(finnFootClips[2]);
            finnMoving = true;
            footstepCounter = 2;
            finnMoveTime = 0;
        }
        else
        {
            StopCoroutine(FinnWalking());
            Debug.Log("Stopped Finn Walking - 2");
        }
    }

    IEnumerator FinnWalking2()
    {
        Debug.Log("Finn Walking 2");
        yield return new WaitForSeconds(28.793f);
        finnFootsteps1.PlayOneShot(xSubscene2[0]);
        finnMoving = true;
        footstepCounter = 3;
        finnMoveTime = 0;
    }

    IEnumerator FinnWalking3()
    {
        Debug.Log("Finn Walking 3");
        finnMoving = true;
        yield return new WaitForSeconds(1.9f);
        finnMoving = false;
        footstepCounter = 4;
        finnMoveTime = 0;
    }

    IEnumerator FinnWalking4()
    {
        Debug.Log("Finn Walking 4");
        finnMoving = true;
        yield return new WaitForSeconds(1.5f);
        footstepCounter = 5;
        finnMoving = false;
        finnMoveTime = 0;
    }
    
    #endregion

    public IEnumerator ExtraClip1()
    {
        Debug.Log("Start Extra Clip 1");
        yield return new WaitForSeconds(15.064f);
        yield return new WaitForSeconds(8.531f);
        if (!finnDialogue1.isPlaying)
        {
            finnDialogue0.PlayOneShot(xSubscene1[1]);
        }
        else
        {
            StopCoroutine(ExtraClip1());
            Debug.Log("Stopped Extra Clip 1 - 0");
        }
        yield return new WaitForSeconds(1.552f);
        if (!finnDialogue1.isPlaying)
        {
            finnDialogue0.PlayOneShot(xSubscene1[2]);
        }
        else
        {
            StopCoroutine(ExtraClip1());
            Debug.Log("Stopped Extra Clip 1 - 1");
        }
        yield return new WaitForSeconds(12.073f);
        if (!finnDialogue1.isPlaying)
        {
            finnTable.PlayOneShot(xSubscene1[3]);
        }
        else
        {
            StopCoroutine(ExtraClip1());
            Debug.Log("Stopped Extra Clip 1 - 2");
        }
        yield return new WaitForSeconds(17.026f);
        if (!finnDialogue1.isPlaying)
        {
            finnDoorBang.PlayOneShot(xSubscene1[4]);
        }
        else
        {
            StopCoroutine(ExtraClip1());
            Debug.Log("Stopped Extra Clip 1 - 3");
        }
        yield return new WaitForSeconds(0.701f);
        if (!finnDialogue1.isPlaying)
        {
            finnDoorBang.PlayOneShot(xSubscene1[5]);
        }
        else
        {
            StopCoroutine(ExtraClip1());
            Debug.Log("Stopped Extra Clip 1 - 4");
        }
        yield return new WaitForSeconds(10.934f);
        if (!finnDialogue1.isPlaying)
        {
            finnDoorBang.PlayOneShot(xSubscene1[6]);
        }
        else
        {
            StopCoroutine(ExtraClip1());
            Debug.Log("Stopped Extra Clip 1 - 5");
        }
        yield return new WaitForSeconds(0.892f);
        if (!finnDialogue1.isPlaying)
        {
            finnDialogue0.PlayOneShot(xSubscene1[7]);
        }
        else
        {
            StopCoroutine(ExtraClip1());
            Debug.Log("Stopped Extra Clip 1 - 6");
        }
    }

    public IEnumerator ExtraClip2()
    {
        StartCoroutine(FinnWalking2());
        yield return new WaitForSeconds(28.793f);
        finnFootsteps1.PlayOneShot(xSubscene2[0]);
        yield return new WaitForSeconds(1.832f);
        finnTable2.PlayOneShot(xSubscene2[1]);
        yield return new WaitForSeconds(74.464f);
        finnTable2.PlayOneShot(xSubscene2[2]);
    }

    public IEnumerator ExtraClip3()
    {
        StartCoroutine(FinnWalking3());
        yield return new WaitForSeconds(0.125f);
        finnFootsteps1.PlayOneShot(xSubscene3[0]);
        yield return new WaitForSeconds(2.817f);
        nonSpatial.PlayOneShot(xSubscene3[1]);
    }

    public IEnumerator ExtraClip4()
    {
        StartCoroutine(FinnWalking4());
        yield return new WaitForSeconds(0.0f);
        finnFootsteps1.PlayOneShot(xSubscene4[0]);
        yield return new WaitForSeconds(26.174f);
        nonSpatial.PlayOneShot(xSubscene4[1]);
    }


    #region FinnWalks
    void FinnWalk0()
    {
        if (!finnDialogue1.isPlaying)
        {
            Finn0.position = new Vector3(Mathf.Lerp(249.1f, 259.1f, finnMoveTime), Finn0.position.y, Mathf.Lerp(78.9f, 58.3f, finnMoveTime));
        }
    }

    void FinnWalk1()
    {
        if (!finnDialogue1.isPlaying)
        {
            Finn0.position = new Vector3(Mathf.Lerp(259.1f, 237.3f, finnMoveTime), Finn0.position.y, Finn0.position.z);
        }
    }

    void FinnWalk2()
    {
        if (!finnDialogue1.isPlaying)
        {
            Finn0.position = new Vector3(Mathf.Lerp(237.3f, 249.1f, finnMoveTime), Finn0.position.y, Mathf.Lerp(58.3f, 87.9f, finnMoveTime));
        }
    }

    void FinnWalk3()
    {
        Finn1.position = new Vector3(Mathf.Lerp(249.1f, 235.8f, finnMoveTime), Finn1.position.y, Mathf.Lerp(87.9f, 71.8f, finnMoveTime));
    }
    
    void FinnWalk4()
    {
        Finn1.position = Vector3.MoveTowards(Finn1.position, player.position, Time.deltaTime * 3f);
    }

    void FinnWalk5()
    {
        Finn1.position = Vector3.MoveTowards(Finn1.position, player.position, Time.deltaTime * 2f);
    }
    #endregion
}
