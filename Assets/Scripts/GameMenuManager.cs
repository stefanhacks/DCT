using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour {
    [Header("Menu Objects")]
    public GameObject gameMenu;
    public GameObject popUps, changePlayerMenu, inGameHUD;

    [Header("Player Data fields")]
    public Text nameField;
    public Text scoreField, ingameScoreField;
    public Dropdown dialogBoxDropdown;
    public InputField nameInputField;
    public GameObject characterArea;

    [Header("Panels and Dialogs")]
    public GameObject nameInputWarning;
    public GameObject loadPlayerWarning, newPlayerDialog, loadPlayerDialog, deletePlayerDialog, gameOverDialog;
    public Text scoreNames, scorePoints;

    private CharacterManager cmInstance;
    private GameManager gmInstance;
    
    public void Start()
    {
        // Runs after Awake, letting Character Manager load everything first.
        cmInstance = this.gameObject.GetComponent<CharacterManager>();
        gmInstance = this.gameObject.GetComponent<GameManager>();

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
            BackToTitle();
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

    public void ToggleGameHUD()
    {
        // Toggles between Game Menu and HUD.
        bool newStatus = !inGameHUD.activeSelf;

        inGameHUD.SetActive(newStatus);
        gameMenu.SetActive(!newStatus);
    }

    public void TogglePauseMenu()
    {
        // Toggles between both HUD and New/Load/Delete player, and Game Menu.
        changePlayerMenu.SetActive(!inGameHUD.activeSelf);
        ToggleGameHUD();
    }

    public void ToggleGameOverPanel()
    {
        // Toggles between HUD and GameOver Dialog.
        bool newStatus = !inGameHUD.activeSelf;

        // Graphic Raycaster also use this state to define theirs.
        inGameHUD.SetActive(newStatus);
        gameOverDialog.SetActive(!newStatus);
        popUps.GetComponent<GraphicRaycaster>().enabled = !newStatus;

        UpdateHighScores();
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
    
    public void BackToTitle()
    {
        Time.timeScale = 1;
        GameObject.FindGameObjectWithTag("SceneSwapper").GetComponent<SceneSwapper>().AnimateExit("00_TitleScene");
    }

    public void PlayButton()
    {
        gameOverDialog.SetActive(false);
        gmInstance.StartGame(cmInstance.GetCurrentPlayer(), cmInstance.GetPlayerSprites());
        ToggleGameHUD();
    }

    public void PauseGameButton()
    {
        RefreshPlayer();
        gmInstance.PauseGame();
        TogglePauseMenu();
    }

    public void ChangeCharacterButton()
    {
        RefreshPlayer ();
        changePlayerMenu.SetActive(true);
        gameMenu.SetActive(true);
        TogglePopPanel(gameOverDialog);
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

    public void UpdateScore(int gamePoints)
    {
        ingameScoreField.text = gamePoints.ToString();
    }

    public void UpdateHighScores()
    {
        // Gets Highscores
        KeyValuePair<string, Dictionary<string, int>>[] highscores = cmInstance.GetHighScores();

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
