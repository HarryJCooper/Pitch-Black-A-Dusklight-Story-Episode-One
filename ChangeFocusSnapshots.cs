using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ChangeFocusSnapshots : MonoBehaviour
{
    public POISnapshot pOISnapshotController;
    public AudioMixerSnapshot standard;
    public AudioMixerSnapshot focused;


    private void OnTriggerEnter(Collider other)
    {
        pOISnapshotController.Focused = focused;
        pOISnapshotController.Standard = standard;
    }
}
