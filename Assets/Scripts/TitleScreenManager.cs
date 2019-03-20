using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject rootTitle, rootTitleMenu, rootHighScores, rootBG, rootTut;
    public Text scoreNames, scorePoints;
    private GameData data;

    public void Awake()
    {
        #if UNITY_IOS
            // Forces different BinaryFormatter path for iOS.
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        #endif

        // Loads data object, may be null if first time running.
        data = DataManager.LoadPlayers();
    }

    // UI Code
    public void ShowTitleMenu()
    {
        rootTut.SetActive(false);
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
        KeyValuePair<string, Dictionary<string, int>>[] highscores = (data == null)
            ? new KeyValuePair<string, Dictionary<string, int>>[0]
            : data.GiveHighScores();

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

    public void PlayButton()
    {
        GameObject.FindWithTag("SceneSwapper").GetComponent<SceneSwapper>().AnimateExit("01_Game");
    }
}
