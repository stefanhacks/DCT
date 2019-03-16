using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject rootTitle, rootTitleMenu, rootHighScores, rootBG;
    public Text scoreNames, scorePoints;
    private GameData data;

    public void Awake()
    {
        #if UNITY_IOS
            // Forces different BinaryFormatter path for iOS.
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        #endif
        
        // If save file doesn't exist yet, must create it with a default player.
        if (DataManager.LoadPlayers() == null){
            GameObject emptyPlayer = new GameObject("emptyPlayer");
            emptyPlayer.AddComponent<PlayerData>();

            DataManager.SavePlayerData(
                emptyPlayer.GetComponent<PlayerData>(), 
                new Dictionary<string, Dictionary<string, int>>()
                );

            Destroy(emptyPlayer);
        }

        // Loads data object.
        data = DataManager.LoadPlayers();
        Invoke("ShowTitleMenu", 1);
    }

    // UI Code
    public void ShowTitleMenu()
    {
        rootTitle.SetActive(true);
        rootHighScores.SetActive(false);
        rootTitleMenu.SetActive(true);
    }

    public void ShowHighScores()
    {
        rootTitle.SetActive(false);
        rootTitleMenu.SetActive(false);
        rootHighScores.SetActive(true);
        LoadHighScores();
    }

    // Sets high score screen.
    public void LoadHighScores()
    {
        // Uses LINQ to sort the player data by highscore and get the top three.
        KeyValuePair<string, Dictionary<string, int>>[] highscores = 
            data.allPlayers.OrderByDescending(s => s.Value["highscore"]).Take(3).ToArray();

        // Sets up the actual text in menu.
        scoreNames.text = "";
        scorePoints.text = "";
        for (int i = 0; i < 3; i++)
        {
            if (i < highscores.Length)
            {
                scoreNames.text += highscores[i].Key + "\n";
                scorePoints.text += highscores[i].Value["highscore"] + "\n";
            }
            else
            {
                scoreNames.text += "-\n";
                scorePoints.text += "-\n";
            }            
        }
    }
}
