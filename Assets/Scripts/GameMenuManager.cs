using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour {
    public Canvas gameMenu, popUps;
    public GameObject dialogBoxNew, dialogBoxLoad, dialogBoxDelete;
    public Dropdown dialogBoxDropdown;

    private GameObject characterArea;
    private PlayerData currentPlayer;
    private Text nameField, scoreField;

    public void Start()
    {
        // Runs after Awake, letting GameManager parent load everything first.
        characterArea = GameObject.FindGameObjectWithTag("CPCharacterArea");
        nameField = GameObject.FindGameObjectWithTag("PPNameField").GetComponent<Text>();
        scoreField = GameObject.FindGameObjectWithTag("PPScoreField").GetComponent<Text>();

        // Updates Fields based on the current player.
        currentPlayer = this.gameObject.GetComponent<GameManager>().GetCurrentPlayer();
        nameField.text = currentPlayer.playerName;
        scoreField.text = currentPlayer.highScore.ToString();   
    }

    public void RefreshPlayerModel(Sprite[] nextSprites)
    {
        RefreshPlayerModel("eyes", nextSprites[0]);
        RefreshPlayerModel("head", nextSprites[1]);
        RefreshPlayerModel("mouth", nextSprites[2]);
        RefreshPlayerModel("torso", nextSprites[3]);
        RefreshPlayerModel("legs", nextSprites[4]);
    }

    public void RefreshPlayerModel(string area, Sprite nextSprite)
    {
        GameObject areaToRefresh = characterArea.transform.Find(area).gameObject;
        areaToRefresh.GetComponent<SpriteRenderer>().sprite = nextSprite;
    }

    public void TogglePopPanel(GameObject panel)
    {
        // Checks panel for it's current "active" status and flips it.
        bool newStatus = !panel.activeSelf;

        // Graphic Raycasters also use this state to define their status.
        panel.SetActive(newStatus);
        popUps.GetComponent<GraphicRaycaster>().enabled = newStatus;
        gameMenu.GetComponent<GraphicRaycaster>().enabled = !newStatus;
    }

    public void SetPopUpGFXRaycaster(bool set)
    {
        // Used to make sure load player popup is inactive during dropdown 
        popUps.GetComponent<GraphicRaycaster>().enabled = set;
    }
}
