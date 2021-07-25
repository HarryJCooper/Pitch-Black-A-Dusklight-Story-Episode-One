using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LowPassAutomator : MonoBehaviour
{
    public AudioMixer audioMixer;
    public float lowPassFrequency0, lowPassFrequency1, distanceFromGuy, lerpedLowpass;
    public GameObject charOb, playerOb;
    public Vector3 character, player;
    public string lowPassPara0, lowPassPara1;
    public bool doorOpen, enteredDesert;
    float decreaseTiming, increaseTiming;
    public ChangeAfterFocus changeAfterFocus;

    // Start is called before the first frame update
    void Start()
    {
        doorOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        character = charOb.transform.position;
        player = playerOb.transform.position;

        distanceFromGuy = Vector3.Distance(character, player);

        if (changeAfterFocus)
        {
            if (!enteredDesert && !changeAfterFocus.hit)
            {
                if (!doorOpen)
                {
                    audioMixer.SetFloat(lowPassPara0, lowPassFrequency0);
                    audioMixer.SetFloat(lowPassPara1, lowPassFrequency0);
                    lowPassFrequency0 = 1 / distanceFromGuy * 50000;
                }

                else
                {
                    audioMixer.SetFloat(lowPassPara0, lowPassFrequency1);
                    audioMixer.SetFloat(lowPassPara1, lowPassFrequency1);
                    lowPassFrequency1 = 1 / distanceFromGuy * 500000;
                }
            }
            else if (changeAfterFocus.hit) // THIS IS OKAY BECAUSE ZOOM WILL ONLY HAPPEN ONCE THE DOOR IS OPEN
            {
                increaseTiming += 0.5f * Time.deltaTime;
                audioMixer.SetFloat(lowPassPara1, Mathf.Lerp(lowPassFrequency1, 22000, increaseTiming));
            }
            else
            {
                decreaseTiming += 0.5f * Time.deltaTime;
                lerpedLowpass = Mathf.Lerp(20000, 200, decreaseTiming);
                audioMixer.SetFloat(lowPassPara0, lerpedLowpass);
                audioMixer.SetFloat(lowPassPara1, lerpedLowpass);
            }
        }
    }
}
