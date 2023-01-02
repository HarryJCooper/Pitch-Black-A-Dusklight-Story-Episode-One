using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DearVR;

public class AudioSourceContainer : MonoBehaviour
{
    public AudioSource finnSource, protagSource, protagActionSource, laraSource, milesSource, nightlanderOneSource,
    nightlanderTwoSource, nightlanderThreeSource, nightlanderFourSource,
    radioSource, glassSource, doorSource, donnieSource, tableSource, pumpingStationSource, dusklightSource, trainSource, guitarSource;

    public DearVRSource finnVRSource, protagVRSource, laraVRSource, milesVRSource, nightlanderOneVRSource,
    nightlanderTwoVRSource, nightlanderThreeVRSource, nightlanderFourVRSource,
    radioVRSource, glassVRSource, doorVRSource, donnieVRSource, tableVRSource, 
    pumpingStationVRSource, dusklightVRSource, trainVRSource, guitarVRSource; 

    void Awake(){
        finnVRSource = finnSource.GetComponent<DearVRSource>(); 
        protagVRSource = protagSource.GetComponent<DearVRSource>(); 
        laraVRSource = laraSource.GetComponent<DearVRSource>(); 
        milesVRSource = milesSource.GetComponent<DearVRSource>(); 
        nightlanderOneVRSource = nightlanderOneSource.GetComponent<DearVRSource>();
        nightlanderTwoVRSource = nightlanderTwoSource.GetComponent<DearVRSource>(); 
        nightlanderThreeVRSource = nightlanderThreeSource.GetComponent<DearVRSource>(); 
        nightlanderFourVRSource = nightlanderFourSource.GetComponent<DearVRSource>();
        radioVRSource = radioSource.GetComponent<DearVRSource>(); 
        glassVRSource = glassSource.GetComponent<DearVRSource>(); 
        doorVRSource = doorSource.GetComponent<DearVRSource>(); 
        donnieVRSource = donnieSource.GetComponent<DearVRSource>();
        tableVRSource = tableSource.GetComponent<DearVRSource>();
        pumpingStationVRSource = pumpingStationSource.GetComponent<DearVRSource>(); 
        dusklightVRSource = dusklightSource.GetComponent<DearVRSource>();
        trainVRSource = trainSource.GetComponent<DearVRSource>();
        guitarVRSource = guitarSource.GetComponent<DearVRSource>();
    }

}
