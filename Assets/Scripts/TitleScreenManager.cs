using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject RootTitle, RootTitleMenu, RootHighScores, RootBG;

    public void Awake()
    {
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
    }
}
