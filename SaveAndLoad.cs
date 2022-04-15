using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveAndLoad : MonoBehaviour
{
    public int tutorial, bunkerAndEncampment, pumpingStation;
    [SerializeField] GameObject player;
    [SerializeField] DarkVsLight darkVsLight;
    
    void SavePlayer(){
        PlayerPrefs.SetFloat("player_x", player.transform.position.x);
        PlayerPrefs.SetFloat("player_z", player.transform.position.z);
        PlayerPrefs.SetInt("DarkAndLight", darkVsLight.playerDarkness);
    }

    void LoadPlayer(){
        player.transform.position = new Vector3(PlayerPrefs.GetFloat("player_x"), 0.3f, PlayerPrefs.GetFloat("player_z"));
        darkVsLight.playerDarkness = PlayerPrefs.GetInt("DarkAndLight", 0);
    }

    public void PretendTutorialIsDone(){ PlayerPrefs.SetInt("tutorial", 1);}
    public void PretendEncampmentIsDone(){ PlayerPrefs.SetInt("bunkerAndEncampment", 1);}
    public void PretendPumpingStationIsDone(){ PlayerPrefs.SetInt("pumpingStation", 1);}
    
    void ResetEncampment(){
        PlayerPrefs.SetInt("finnInBunkerSequence", 0);
        PlayerPrefs.SetInt("encampmentCombatSequence", 0);
        PlayerPrefs.SetInt("aroundTableSequence", 0);
        PlayerPrefs.SetInt("auditoryZoomSequence", 0);
        PlayerPrefs.SetInt("mechanicSequence", 0);
        PlayerPrefs.SetInt("secretSequence", 0);
        PlayerPrefs.SetFloat("playerX", 0);
        PlayerPrefs.SetFloat("playerZ", -10);
    }
    
    void ResetPumpingStation(){
        PlayerPrefs.SetFloat("playerX", 0);
        PlayerPrefs.SetFloat("playerZ", 0);
        PlayerPrefs.SetInt("enteredDesertSequence", 0);
        PlayerPrefs.SetInt("playerLostFightSequence", 0);
        PlayerPrefs.SetInt("playerWonFightSequence", 0);
        PlayerPrefs.SetInt("explosion", 0);
        PlayerPrefs.SetInt("trappedWorkerSequence", 0);
        PlayerPrefs.SetInt("savedWorkerSequence", 0);
        PlayerPrefs.SetInt("leftWorkerSequence", 0);
    }

    public void ResetGame(){
        PlayerPrefs.SetInt("tutorial", 0);
        PlayerPrefs.SetInt("bunkerAndEncampment", 0);
        PlayerPrefs.SetInt("pumpingStation", 0);
        tutorial = 0;
        bunkerAndEncampment = 0;
        pumpingStation = 0;
        ResetEncampment();
        ResetPumpingStation();
    }

    public void StartGame(){
        PlayerPrefs.SetInt("inMainMenu", 0);
        if (tutorial == 0){ SceneManager.LoadScene("Tutorial"); return; }
        if (bunkerAndEncampment == 0){ SceneManager.LoadScene("BunkerAndEncampment"); return; }
        if (pumpingStation == 0){ SceneManager.LoadScene("ThePumpingStation"); return; }
        ResetGame();
        SceneManager.LoadScene("Tutorial");
    }

    void Awake(){
        PlayerPrefs.SetInt("inMainMenu", 1);
        tutorial = PlayerPrefs.GetInt("tutorial");
        bunkerAndEncampment = PlayerPrefs.GetInt("bunkerAndEncampment");
        pumpingStation = PlayerPrefs.GetInt("pumpingStation");
    }
}
