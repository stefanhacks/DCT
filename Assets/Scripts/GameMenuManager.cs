using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that controls the whole Menu Flow for the Game Scene.
/// Must hold reference to several Menu Objects, TextFields and Dialogs.
/// </summary>
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

    /// <summary>
    /// Function loads all details from the currentplayer to the appropriate fields.
    /// </summary>
    public void RefreshPlayer()
    {
        nameField.text = cmInstance.GetCurrentPlayer().playerName;
        scoreField.text = cmInstance.GetCurrentPlayer().highScore.ToString();
        RefreshPlayerModel(cmInstance.GetPlayerSprites());
    }
    
    /// <summary>
    /// Given a string area, finds the game object that holds the sprite to be changed and alters it.
    /// </summary>
    public void RefreshPlayerModel(string area, Sprite nextSprite)
    {
        GameObject areaToRefresh = characterArea.transform.Find(area).gameObject;
        areaToRefresh.GetComponent<SpriteRenderer>().sprite = nextSprite;
    }

    /// <summary>
    /// Fully replaces a character's appearance, based on a Array of Sprites.
    /// </summary>
    public void RefreshPlayerModel(Sprite[] nextSprites)
    {
        RefreshPlayerModel("eyes", nextSprites[0]);
        RefreshPlayerModel("head", nextSprites[1]);
        RefreshPlayerModel("mouth", nextSprites[2]);
        RefreshPlayerModel("torso", nextSprites[3]);
        RefreshPlayerModel("legs", nextSprites[4]);
    }

    /// <summary>
    /// Generic function for togling Panels on the Pop Up Canvas.
    /// It checks it's current status and flips it around.
    /// Also toggles Canvases GraphicRaycaster's, so inputs don't collide.
    /// </summary>
    public void TogglePopPanel(GameObject panel)
    {
        bool newStatus = !panel.activeSelf;
        
        panel.SetActive(newStatus);
        popUps.GetComponent<GraphicRaycaster>().enabled = newStatus;
        gameMenu.GetComponent<GraphicRaycaster>().enabled = !newStatus;
    }

    /// <summary>
    /// Relies in TogglePopPanel for it's functionality, but clears name field and forces
    /// Player to go back to the menu in the case that no player is selected nor was created.
    /// </summary>
    public void ToggleNewPlayerDialog()
    {
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
   
    /// <summary>
    /// Relies in TogglePopPanel for it's functionality, but creates a dropdown list of options
    /// with all the player names on it beforehand, allowing them to be properly selected and loaded.
    /// </summary>
    public void ToggleLoadPlayerDialog()
    {
        if (!loadPlayerDialog.activeSelf)
        {
            dialogBoxDropdown.ClearOptions();
            dialogBoxDropdown.options.Insert(0, new Dropdown.OptionData("Select player"));
            dialogBoxDropdown.AddOptions(cmInstance.GetPlayerNames());
            loadPlayerWarning.SetActive(false);
        }

        TogglePopPanel(loadPlayerDialog);
    }

    /// <summary>
    /// Toggles GameHud status, and sets the contrary to the Game Menu.
    /// </summary>
    public void ToggleGameHUD()
    {
        bool newStatus = !inGameHUD.activeSelf;

        inGameHUD.SetActive(newStatus);
        gameMenu.SetActive(!newStatus);
    }

    /// <summary>
    /// Relies on ToggleGameHud for it's functionality, but also deactivates the part
    /// of the Game Menu that allows creating, changing and deleting players.
    /// </summary>
    public void TogglePauseMenu()
    {
        // Toggles between both HUD and New/Load/Delete player, and Game Menu.
        changePlayerMenu.SetActive(!inGameHUD.activeSelf);
        ToggleGameHUD();
    }

    /// <summary>
    /// Allows GameOver PopUp to show, loading HighScores on the way and deactivating the GameHud.
    /// </summary>
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

    /// <summary>
    /// Button to finalize Player Creation. 
    /// Checks if the player already exists, if it does, warns the user, 
    /// if not, returns to the menu and updates everything.
    /// </summary>
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

    /// <summary>
    /// Button to finalize Player Selection.
    /// Checks if the user selected a name, if it did not, warns the user, 
    /// if it did, returns to menu and updates everything.
    /// </summary>
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

    /// <summary>
    /// Button to finalize Player Deletion.
    /// Checks if another player can be loaded, if it cannot, 
    /// force creating a new one.
    /// </summary>
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
        // Time Scale is altered here in the event this is being called by the pause menu.
        Time.timeScale = 1;
        GameObject.FindGameObjectWithTag("SceneSwapper").GetComponent<SceneSwapper>().AnimateExit("00_TitleScene");
    }

    /// <summary>
    /// Menu Button for starting the game.
    /// </summary>
    public void PlayButton()
    {
        gameOverDialog.SetActive(false);
        gmInstance.StartGame(cmInstance.GetCurrentPlayer(), cmInstance.GetPlayerSprites());
        ToggleGameHUD();
    }
   
    /// <summary>
    /// HUD Button for pausing the game.
    /// </summary>
    public void PauseGameButton()
    {
        RefreshPlayer();
        gmInstance.PauseGame();
        TogglePauseMenu();
    }

    /// <summary>
    /// GameOver Button for changing character.
    /// </summary>
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

    /// <summary>
    /// Function called during Player Creation, when user is typing a new name.
    /// Toggles warning off if it exists, meaning that if player tried
    /// submitting an existing player name and failed, it's trying again.
    /// </summary>
    public void CheckForExistingName()
    {
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
