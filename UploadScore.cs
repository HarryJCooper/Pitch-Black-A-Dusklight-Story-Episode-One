using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UploadScore : MonoBehaviour
{
    //THIS ISN'T NEEDED ANYMORE



    /*
    
    public int playerScore, alphcount1, alphcount2, alphcount3;


    private void Awake()
    {


        Load();
        SaveSystem.Init();

        SaveObject saveObject = new SaveObject
        {
            playerScore = 3,
            alphcount1 = 4,
            alphcount2 = 3,
            alphcount3 = 6
        };

        string json = JsonUtility.ToJson(saveObject);
        Debug.Log(json);

        SaveObject loadedSaveObject = JsonUtility.FromJson<SaveObject>(json);
        Debug.Log((loadedSaveObject.playerScore, loadedSaveObject.alphcount1, loadedSaveObject.alphcount2, loadedSaveObject.alphcount3));
    }

    public void Update()
    {
        playerScore = GameObject.Find("Leaderboard").GetComponent<Leaderboard>().endlessScore;
        alphcount1 = GameObject.Find("Leaderboard").GetComponent<Leaderboard>().alphcount1;
        alphcount2 = GameObject.Find("Leaderboard").GetComponent<Leaderboard>().alphcount2;
        alphcount3 = GameObject.Find("Leaderboard").GetComponent<Leaderboard>().alphcount3;

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("s");
            Save();
        }
    }

    private void Save()
    {
        SaveObject saveObject = new SaveObject
        {
            playerScore = playerScore,
            alphcount1 = alphcount1,
            alphcount2 = alphcount2,
            alphcount3 = alphcount3
        };

        string json = JsonUtility.ToJson(saveObject);

        SaveSystem.Save(json);
    }

    private void Load()
    {
        // THIS BIT NEEDS TO ADD EACH SAVE FILE AND TURN INTO AN ARRAY
        string saveString = SaveSystem.Load();
        if (saveString != null)
        {
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);
        }
    }

    public class SaveObject
    {
        public int playerScore, alphcount1, alphcount2, alphcount3;
    }

    */

}
