using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adds all dependencies.
[RequireComponent(typeof(CharacterManager), typeof(GameMenuManager), typeof(PlayerData))]
public class GameManager : MonoBehaviour {

    [Header("Gameplay Balance Settings")]
    
    [Range(0f, 3f)]
    [Tooltip("Interval between the appearance of every two obstacles.")]
    public float initialSpawnInterval = 1;

    [Range(1, 10)]
    [Tooltip("How many obstacles need to spawn before increasing gamespeed.")]
    public int obstacleTreshold = 6;
    
    [Range(1f, 1.5f)]
    [Tooltip("Value for which to divide obstacle interval for speeding up gameplay.")]
    public float intervalDivisor = 1.07f;

    [Range(1, 120)]
    [Tooltip("How long player must play before increase in difficulty.")]
    public int difficultyRamp = 40;

    [Range(1, 120)]
    [Tooltip("Force applied to character jump.")]
    public int jumpForce = 80;

    [Header("GameManager Variables and Object References")]
    public GameObject playerObject;
    public GameObject spawnPoint;
    public GameObject[] obstaclePrefabs;

    private int gamePoints, obstacleCount, currentDifficulty;
    private float playingDuration, innerTimer, currentSpawnInterval;

    private PlayerData currentPlayer;
    private PlayerScript playerScript;
    private CharacterManager cmInstance;
    private GameMenuManager mManagerInstance;

    // TODO: Set to private
    private GameState currentState;
    private enum GameState {GameMenu, Prepare, Playing, Paused, GameOver};

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
                // Core game loop.
                this.UpdateHUD();
                this.DoGameLoop();
                this.CheckPlayerInput();
                this.CheckGameOver();
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
        RefreshPlayerObject(cmInstance.GetPlayerSprites());
        currentPlayer = cmInstance.GetCurrentPlayer();
        playerObject.SetActive(true);

        // Must activate proper game hud.
        mManagerInstance.ToggleGameHUD();

        // Reset game variables.
        currentDifficulty = 0;
        playingDuration = 0;
        gamePoints = 0;
        obstacleCount = 0;
        currentSpawnInterval = initialSpawnInterval;

        // Reset Player.
        playerScript.SetWasHit(false);
    }
    
    private void DoGameLoop()
    {
        // Applies frame interval to timers.
        playingDuration += Time.deltaTime;
        innerTimer += Time.deltaTime;

        // Checks if obstacle needs to be spawned and runs gameplay timer logic.
        if (innerTimer > currentSpawnInterval)
        {
            // Checks for how long player has been playing,
            if (playingDuration > difficultyRamp * currentDifficulty)
            {
                currentDifficulty += 1;
                currentSpawnInterval = initialSpawnInterval;
            }

            // Spawns object according to appropriate difficulty.
            switch (currentDifficulty)
            {
                case 1:
                    Instantiate(obstaclePrefabs[0], spawnPoint.transform);
                    break;
                case 2:
                    Instantiate(obstaclePrefabs[1], spawnPoint.transform);
                    break;
                case 3:
                    Instantiate(obstaclePrefabs[2], spawnPoint.transform);
                    break;
                default:
                    break;
            }
            
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
        // Do Input managing. Although this will result in a rigidbody 
        // being altered, this is left handled by regular Update as 
        // to avoid input loss, not being a continuous button press check.
        if (Input.GetButtonDown("Jump"))
        {
            playerScript.TryJump(new Vector2(0, jumpForce));
        } else if (Input.GetButton("Cancel"))
        {
            PauseButton();
        }
    }

    private void CheckGameOver()
    {
        if (playerScript.GetWasHit())
        {
            currentState = GameState.GameOver;
        }
    }

    private void RefreshPlayerObject(Sprite[] nextSprites)
    {
        playerObject.transform.Find("eyes").GetComponent<SpriteRenderer>().sprite = nextSprites[0];
        playerObject.transform.Find("head").GetComponent<SpriteRenderer>().sprite = nextSprites[1];
        playerObject.transform.Find("mouth").GetComponent<SpriteRenderer>().sprite = nextSprites[2];
        playerObject.transform.Find("torso").GetComponent<SpriteRenderer>().sprite = nextSprites[3];
        playerObject.transform.Find("legs").GetComponent<SpriteRenderer>().sprite = nextSprites[4];
    }

    private void UpdateHUD()
    {
        mManagerInstance.UpdateScore(gamePoints);
    }

    public void StartGame()
    {
        currentState = GameState.Prepare;
    }

    public void PauseButton()
    {
        Debug.Log("Paused");
    }
    
    public void ObstacleDodged(int pointsValue)
    {
        gamePoints += pointsValue;
    }
}
