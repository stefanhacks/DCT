using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour {
    public Canvas gameMenu, popUps;
    public Dropdown dialogBoxDropdown;
    public InputField nameInputField;
    public GameObject newPlayerDialog, nameInputWarning, loadPlayerDialog, loadPlayerWarning, deletePlayerDialog;

    private GameManager gmInstance;

    private GameObject characterArea;
    private Text nameField, scoreField;

    public void Start()
    {
        // Runs after Awake, letting GameManager parent load everything first.
        gmInstance = this.gameObject.GetComponent<GameManager>();
        characterArea = GameObject.FindGameObjectWithTag("CPCharacterArea");
        nameField = GameObject.FindGameObjectWithTag("PPNameField").GetComponent<Text>();
        scoreField = GameObject.FindGameObjectWithTag("PPScoreField").GetComponent<Text>();

        // Updates Fields based on the current player.
        // If there are no players on base, force creating one.
        if (gmInstance.GetCurrentPlayer() != null)
        {
            nameField.text = gmInstance.GetCurrentPlayer().playerName;
            scoreField.text = gmInstance.GetCurrentPlayer().highScore.ToString();
            RefreshPlayerModel(gmInstance.GetPlayerSprites());
        } else
        {
            TogglePopPanel(newPlayerDialog);
        }
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

    public void RefreshPlayer()
    {
        nameField.text = gmInstance.GetCurrentPlayer().playerName;
        scoreField.text = gmInstance.GetCurrentPlayer().highScore.ToString();
        RefreshPlayerModel(gmInstance.GetPlayerSprites());
    }

    public void TogglePopPanel(GameObject panel)
    {
        // Checks panel for it's current "active" status and flips it.
        bool newStatus = !panel.activeSelf;

        // Graphic Raycasters also use this state to define theirs.
        panel.SetActive(newStatus);
        popUps.GetComponent<GraphicRaycaster>().enabled = newStatus;
        gameMenu.GetComponent<GraphicRaycaster>().enabled = !newStatus;
    }

    public void ToggleNewPlayerDialog()
    {
        // In the case for new player pop up, returns to title if 
        // there's no current player (first time running the game)
        // but if there is, then must clear input field.
        if (gmInstance.GetCurrentPlayer() == null)
        {
            // TODO BETTER
            UnityEngine.SceneManagement.SceneManager.LoadScene("00_TitleScene");
        }
        else
        {
            nameInputField.text = "";
            nameInputWarning.SetActive(false);
            TogglePopPanel(newPlayerDialog);
        }
    }

    public void ToggleLoadPlayerDialog()
    {
        if (!loadPlayerDialog.activeSelf)
        {
            dialogBoxDropdown.ClearOptions();
            dialogBoxDropdown.options.Insert(0, new Dropdown.OptionData("Select player"));
            dialogBoxDropdown.AddOptions(gmInstance.GetPlayerNames());
            loadPlayerWarning.SetActive(false);
        }

        TogglePopPanel(loadPlayerDialog);
    }

    public void SetPopUpGFXRaycaster(bool set)
    {
        // Used to make sure load player popup is inactive during dropdown. 
        popUps.GetComponent<GraphicRaycaster>().enabled = set;
    }

    public void CheckForExistingName()
    {
        // Toggles warning off if it exists.
        if (nameInputWarning.activeSelf) nameInputWarning.SetActive(false);

        // Checks for possible collision with other player names and flags text red if so.
        Text inputText = nameInputField.transform.Find("Text").GetComponent<Text>();

        if (gmInstance.CheckPlayerKey(nameInputField.text)) inputText.color = Color.red;
        else inputText.color = new Color(0.2f, 0.2f, 0.2f);
    }

    public void MakeNewPlayer()
    {
        // If player already exists but user tried to create it even so, call warning box.
        if (gmInstance.CheckPlayerKey(nameInputField.text))
        {
            nameInputWarning.SetActive(true);
        } else if (nameInputField.text.Length > 0)
        {
            // Creates player and updates menu.
            gmInstance.CreateNewPlayer(nameInputField.text);
            RefreshPlayer();
            ToggleNewPlayerDialog();
        }
    }

    public void LoadPlayer()
    {
        // Checks if value selected differs from initial one.
        if (dialogBoxDropdown.value == 0)
        {
            loadPlayerWarning.SetActive(true);
        } else
        {
            // Load selected player.
            gmInstance.LoadPlayerWithKey(dialogBoxDropdown.captionText.text);
            RefreshPlayer();
            ToggleLoadPlayerDialog();
        }
    }

    public void DeletePlayer()
    {
        gmInstance.DeleteCurrentPlayer();
        TogglePopPanel(deletePlayerDialog);

        if (gmInstance.GetCurrentPlayer() != null)
            RefreshPlayer();
        else
            TogglePopPanel(newPlayerDialog);
    }
}
