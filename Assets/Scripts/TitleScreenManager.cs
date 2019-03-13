using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject RootTitle, RootTitleMenu, RootHighScores, RootBG;
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
            emptyPlayer.AddComponent<Player>();

            DataManager.SavePlayerData(
                emptyPlayer.GetComponent<Player>(), 
                new Dictionary<string, Dictionary<string, int>>()
                );
        }

        // Loads data object.
        data = DataManager.LoadPlayers();
        Invoke("ShowTitleMenu", 1);
    }

    // UI Code
    public void ShowTitleMenu()
    {
        RootTitle.SetActive(true);
        RootHighScores.SetActive(false);
        RootTitleMenu.SetActive(true);
    }

    public void ShowHighScores()
    {
        RootTitle.SetActive(false);
        RootTitleMenu.SetActive(false);
        RootHighScores.SetActive(true);
        LoadHighScores();
    }

    // Sets high score screen.
    public void LoadHighScores()
    {
        // Uses LINQ to sort the player data by highscore and get the top three.
        KeyValuePair<string, Dictionary<string, int>>[] highscores = 
            data.allPlayers.OrderByDescending(s => s.Value["highscore"]).Take(3).ToArray();

        // Sets up the actual text in menu.
        Text nameTextObj = RootHighScores.transform.Find("Name").GetComponent<Text>();
        Text scoreTextObj = RootHighScores.transform.Find("Scores").GetComponent<Text>();
        nameTextObj.text = "";
        scoreTextObj.text = "";

        for (int i = 0; i < 3; i++)
        {
            if (i < highscores.Length)
            {
                nameTextObj.text += highscores[i].Key + "\n";
                scoreTextObj.text += highscores[i].Value["highscore"] + "\n";
            }
            else
            {
                nameTextObj.text += "-\n";
                scoreTextObj.text += "-\n";
            }            
        }
    }
}
