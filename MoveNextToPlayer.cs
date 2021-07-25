using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNextToPlayer : MonoBehaviour
{

    public Transform PlayerTransform;
    public Transform AudioCueTransform;
    public GameObject audioCue;

    void Awake()
    {

        AudioCueTransform = PlayerTransform;
    }
}
