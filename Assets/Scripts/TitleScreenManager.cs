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
        
        if (DataManager.LoadPlayers() == null){
            GameObject emptyPlayer = new GameObject("emptyPlayer");
            emptyPlayer.AddComponent<Player>();

            DataManager.SavePlayerData(
                emptyPlayer.GetComponent<Player>(), 
                new Dictionary<string, Dictionary<string, int>>()
                );
        }

        data = DataManager.LoadPlayers();
        Invoke("ShowTitleMenu", 1);
    }

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

    public void LoadHighScores()
    {
        KeyValuePair<string, Dictionary<string, int>>[] highscores = 
            data.allPlayers.OrderByDescending(s => s.Value["highscore"]).Take(3).ToArray();

        string names = "", scores = "";
        for (int i = 0; i < 3; i++)
        {
            if (i < highscores.Length)
            {
                names += highscores[i].Key + "\n";
                scores += highscores[i].Value["highscore"] + "\n";
            } else
            {
                names += "-\n";
                scores += "-\n";
            }
            
        }
        RootHighScores.transform.Find("Name").GetComponent<Text>().text = names;
        RootHighScores.transform.Find("Scores").GetComponent<Text>().text = scores;
    }
}
