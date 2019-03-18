using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour {
    public Canvas gameMenu, popUps, inGameHUD;
    public Text nameField, scoreField;
    public Dropdown dialogBoxDropdown;
    public InputField nameInputField;
    public GameObject characterArea, newPlayerDialog, nameInputWarning, loadPlayerDialog, loadPlayerWarning, deletePlayerDialog;


    private CharacterManager cmInstance;
    
    public void Start()
    {
        // Runs after Awake, letting Character Manager load everything first.
        cmInstance = this.gameObject.GetComponent<CharacterManager>();

        // Updates Fields based on the current player.
        // If there are no players on base, force creating one.
        if (cmInstance.GetCurrentPlayer() != null)
        {
            nameField.text = cmInstance.GetCurrentPlayer().playerName;
            scoreField.text = cmInstance.GetCurrentPlayer().highScore.ToString();
            RefreshPlayerModel(cmInstance.GetPlayerSprites());
        } else
        {
            TogglePopPanel(newPlayerDialog);
        }
    }

    public void RefreshPlayer()
    {
        // Loads all detail from the player to the appropriate fields.
        nameField.text = cmInstance.GetCurrentPlayer().playerName;
        scoreField.text = cmInstance.GetCurrentPlayer().highScore.ToString();
        RefreshPlayerModel(cmInstance.GetPlayerSprites());
    }

    public void RefreshPlayerModel(string area, Sprite nextSprite)
    {
        // Finds the game object that has the sprite to be changed and alters it.
        GameObject areaToRefresh = characterArea.transform.Find(area).gameObject;
        areaToRefresh.GetComponent<SpriteRenderer>().sprite = nextSprite;
    }

    public void RefreshPlayerModel(Sprite[] nextSprites)
    {
        RefreshPlayerModel("eyes", nextSprites[0]);
        RefreshPlayerModel("head", nextSprites[1]);
        RefreshPlayerModel("mouth", nextSprites[2]);
        RefreshPlayerModel("torso", nextSprites[3]);
        RefreshPlayerModel("legs", nextSprites[4]);
    }

    public void TogglePopPanel(GameObject panel)
    {
        // Generic function for togling pop up panels.
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
        if (cmInstance.GetCurrentPlayer() == null)
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
            // Creates dropdown list of options, but runs only when opening the menu.
            dialogBoxDropdown.ClearOptions();
            dialogBoxDropdown.options.Insert(0, new Dropdown.OptionData("Select player"));
            dialogBoxDropdown.AddOptions(cmInstance.GetPlayerNames());
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
        // Function that runs during typing a new player name.
        // Toggles warning off if it exists, meaning that if player tried
        // submitting an existing player name and failed, it's trying again.
        if (nameInputWarning.activeSelf) nameInputWarning.SetActive(false);

        // Checks for possible collision with other player names and flags text red if so.
        Text inputText = nameInputField.transform.Find("Text").GetComponent<Text>();

        if (cmInstance.CheckPlayerKey(nameInputField.text)) inputText.color = Color.red;
        else inputText.color = new Color(0.2f, 0.2f, 0.2f);
    }

    public void MakeNewPlayer()
    {
        // If player already exists but user tried to create it even so, call warning box.
        if (cmInstance.CheckPlayerKey(nameInputField.text))
        {
            nameInputWarning.SetActive(true);
        } else if (nameInputField.text.Length > 0)
        {
            // If it doesn't, creates player and updates menu.
            cmInstance.CreateNewPlayer(nameInputField.text);
            RefreshPlayer();
            ToggleNewPlayerDialog();
        }
    }

    public void LoadPlayer()
    {
        // Checks if value selected on dropdown differs from initial label one.
        if (dialogBoxDropdown.value == 0)
        {
            loadPlayerWarning.SetActive(true);
        } else
        {
            // If it is, loads selected player and updates everything.
            cmInstance.LoadPlayerWithKey(dialogBoxDropdown.captionText.text);
            RefreshPlayer();
            ToggleLoadPlayerDialog();
        }
    }

    public void DeletePlayer()
    {
        // Deletes current player selected and turns off pop up.
        cmInstance.DeleteCurrentPlayer();
        TogglePopPanel(deletePlayerDialog);

        // If Character Manager Instance was able to find another player to load, refresh.
        // If it wasn't, must force the creation of a new player.
        if (cmInstance.GetCurrentPlayer() != null)
            RefreshPlayer();
        else
            TogglePopPanel(newPlayerDialog);
    }

    public void ToggleHUD()
    {
        // Hides Game Menu, brings up HUD.
        bool newStatus = !inGameHUD.enabled;

        inGameHUD.enabled = newStatus;
        gameMenu.enabled = !newStatus;
    }
}
