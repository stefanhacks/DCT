using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adds all dependencies.
[RequireComponent(typeof(CharacterManager), typeof(GameMenuManager), typeof(PlayerData))]
public class GameManager : MonoBehaviour {

    [Header("Gameplay Balance Settings")]
    
    [Range(0f, 2f)]
    [Tooltip("Interval between the appearance of every two obstacles.")]
    public float initialSpawnInterval = 1;

    [Range(1, 10)]
    [Tooltip("How many obstacles need to spawn before increasing gamespeed.")]
    public int obstacleTreshold = 6;

    [Range(1f, 2f)]
    [Tooltip("Value for which to divide obstacle interval for speeding up gameplay.")]
    public float intervalDivisor = 1;

    [Header("GameManager Variables")]
    public GameObject playerObject;

    private int gamePoints, obstacleCount;
    private float playingDuration, innerTimer, currentSpawnInterval;

    private PlayerData currentPlayer;
    private PlayerScript playerScript;
    private CharacterManager cmInstance;
    private GameMenuManager mManagerInstance;

    // TODO: Set to private
    public GameState currentState;
    public enum GameState {GameMenu, Prepare, Playing, Paused, GameOver};

	void Start () {
        cmInstance = this.GetComponent<CharacterManager>();
        mManagerInstance = this.GetComponent<GameMenuManager>();
        playerScript = playerObject.GetComponent<PlayerScript>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case GameState.GameMenu:
                // During player alteration. Changes on "Play".
                break;
            case GameState.Prepare:
                // After play is hit, changes to playing after everything runs once
                this.PrepareGame();
                currentState = GameState.Playing;
                break;
            case GameState.Playing:
                this.DoGameLoop();
                this.CheckPlayerInput();
                // Core game loop.
                // Must instantiate objects.
                // Must run obstacles.
                // Must verify if player object was hit.
                // Must check for game over.
                break;
            case GameState.Paused:
                // Able to happen during Playing.
                // Opens up customization menu.
                // Cannot go to any other state but Playing.
                break;
            case GameState.GameOver:
                // Shows points made.
                // Evaluates if it was a highscore.
                // Goes back to GameMenu or Starting.
                break;
            default:
                break;
        }
    }

    private void PrepareGame()
    {
        // Must alter player object, setting appearance.
        currentPlayer = cmInstance.GetCurrentPlayer();
        this.RefreshPlayerObject(cmInstance.GetPlayerSprites());
        playerObject.SetActive(true);

        // Must activate proper game hud.
        mManagerInstance.ToggleHUD();

        // Reset game variables.
        playingDuration = 0;
        gamePoints = 0;
        obstacleCount = 0;
        currentSpawnInterval = initialSpawnInterval;
        
        // TODO: Reset Player position.
    }
    
    private void DoGameLoop()
    {
        playingDuration += Time.deltaTime;
        innerTimer += Time.deltaTime;

        if (innerTimer > currentSpawnInterval)
        {
            // Spawn
            innerTimer = 0;
            obstacleCount++;
            if (obstacleCount > obstacleTreshold)
            {
                currentSpawnInterval /= intervalDivisor;
                obstacleCount = 0;
            }
        }
    }

    private void CheckPlayerInput()
    {
        // Do Input managing.
        // Could define and use a input axes but gameplay is fairly simple 
        // and will not care for values distinct from these in this case.
        if (Input.GetKeyDown("up") || Input.GetKeyDown("w") ||
            Input.GetKeyDown("mouse 0") || Input.GetKeyDown("space"))
        {
            playerScript.TryJump(new Vector2(0, 80));
        } else if (Input.GetKeyDown("escape") || Input.GetKeyDown("backspace"))
        {
            PauseButton();
        }
    }

    public void RefreshPlayerObject(Sprite[] nextSprites)
    {
        playerObject.transform.Find("eyes").GetComponent<SpriteRenderer>().sprite = nextSprites[0];
        playerObject.transform.Find("head").GetComponent<SpriteRenderer>().sprite = nextSprites[1];
        playerObject.transform.Find("mouth").GetComponent<SpriteRenderer>().sprite = nextSprites[2];
        playerObject.transform.Find("torso").GetComponent<SpriteRenderer>().sprite = nextSprites[3];
        playerObject.transform.Find("legs").GetComponent<SpriteRenderer>().sprite = nextSprites[4];
    }

    public void PauseButton()
    {
        Debug.Log("Paused");
    }
}
