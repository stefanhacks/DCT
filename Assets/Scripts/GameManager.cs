using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class responsible for all the main game loop logic and gameplay effects.
/// Holds references to several other scripts and requires them to be added
/// to it's game object in order to properly run.
/// </summary>
// Adds all dependencies.
[RequireComponent(typeof(CharacterManager))]
[RequireComponent(typeof(GameMenuManager))]
[RequireComponent(typeof(PlayerData))]
[RequireComponent(typeof(GameAreaAnimator))]
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
    [Tooltip("How long player must play before new obstacles appearing.")]
    public int difficultyRamp = 40;

    [Range(1, 120)]
    [Tooltip("Force applied to character jump.")]
    public int jumpForce = 80;

    [Header("GameManager Variables and Object References")]
    public GameObject playerObject;
    public GameObject groundObject, spawnPoint;
    public GameObject[] obstaclePrefabs;

    private int gamePoints, obstacleCount, currentDifficulty;
    private float playingDuration, innerTimer, currentSpawnInterval;

    private PlayerData currentPlayer;
    private PlayerPawn playerPawn;
    private Vector3 originalPlayerPosition;
    private GameMenuManager mManagerInstance;
    private GameAreaAnimator gAreaAnimator;
    
    private GameState currentState;
    private enum GameState {GameMenu, Playing, Paused, GameOver};

	void Start () {
        mManagerInstance = this.GetComponent<GameMenuManager>();
        gAreaAnimator = this.GetComponent<GameAreaAnimator>();
        playerPawn = playerObject.GetComponent<PlayerPawn>();
        originalPlayerPosition = playerPawn.transform.position;
        // Second value is arbitrary and decided based on game feel.
        gAreaAnimator.SetReferences(groundObject, difficultyRamp * 4);
    }

    private void Update()
    {
        // Core game loop.
        switch (currentState)
        {
            case GameState.GameMenu:
                // During player alteration. Changes on "Play".
                break;
            case GameState.Playing:
                // Core game loop.
                this.UpdateHUD();
                this.gAreaAnimator.UpdatePlayArea();
                this.DoSpawning();
                this.CheckPlayerInput();
                this.CheckGameOver();
                break;
            case GameState.Paused:
                // Opens up customization menu.
                // Goes back to Playing once toggled off.
                break;
            case GameState.GameOver:
                // Showing points made.
                // Goes back to GameMenu or Playing.
                break;
            default:
                break;
        }
    }

    public void StartGame(PlayerData player, Sprite[] playerSprites)
    {
        PrepareGame(playerSprites);
        currentPlayer = player;
    }
    
    public void PauseGame()
    {
        Time.timeScale = 0;
        playerObject.SetActive(false);
        spawnPoint.SetActive(false);
        currentState = GameState.Paused;
    }

    /// <summary>
    /// Resets all relevant game variables, updates player appearance and enables needed objects for gameplay.
    /// </summary>
    private void PrepareGame(Sprite[] playerSprites)
    {
        RefreshPlayerObject(playerSprites);
        playerObject.SetActive(true);
        spawnPoint.SetActive(true);

        if (currentState == GameState.Paused)
        {
            Time.timeScale = 1;
        } else
        {
            // Reset game variables.
            currentDifficulty = 0;
            playingDuration = 0;
            gamePoints = 0;
            obstacleCount = 0;
            currentSpawnInterval = initialSpawnInterval;

            // Reset Player and Ground.
            playerPawn.SetWasHit(false);
            playerObject.transform.position = originalPlayerPosition;
            gAreaAnimator.Reset();
        }

        currentState = GameState.Playing;
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

    /// <summary>
    /// Houses the Game Spawning logic.
    /// </summary>
    private void DoSpawning()
    {
        playingDuration += Time.deltaTime;
        innerTimer += Time.deltaTime;

        if (innerTimer > currentSpawnInterval)
        {
            // Checks for how long player has been playing and if gameplay can get harder.
            if (playingDuration > difficultyRamp * currentDifficulty && currentDifficulty < 4)
            {
                currentDifficulty += 1;
                currentSpawnInterval = initialSpawnInterval;
            }

            // maxRange equals = 2, 4, 6 or 8
            // Then m/2 equals = 0, 0, 1, 1, 2, 2 or 3
            // Increasing the chance for harder obstacles to be spawned.
            int maxRange = 2 + (currentDifficulty-1 * 2);
            int spawnChoice = UnityEngine.Random.Range(0, maxRange) / 2;

            GameObject obstacle = Instantiate(obstaclePrefabs[spawnChoice], spawnPoint.transform);
            if (spawnChoice == 3 && UnityEngine.Random.value > 0.5f)
                obstacle.GetComponent<ObstaclePawn>().SetFade();

            // If game is the in the last difficulty, adds a chance for the "Fake" obstacle to be thrown again.
            if (currentDifficulty == 4 && UnityEngine.Random.value < 0.2f)
                Instantiate(obstaclePrefabs[3], spawnPoint.transform).GetComponent<ObstaclePawn>().SetFade();

            innerTimer = 0;
            obstacleCount++;
            if (obstacleCount > obstacleTreshold)
            {
                currentSpawnInterval /= intervalDivisor;
                obstacleCount = 0;
            }
        }
    }
 
    /// <summary>
    /// Do Input managing. Although this will result in a rigidbody 
    /// being altered in regular Update, this is left as is to avoid 
    /// input loss and not being a continuous button press check.
    /// </summary>
    private void CheckPlayerInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            playerPawn.TryJump(new Vector2(0, jumpForce));
        }
        else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            playerPawn.TryJump(new Vector2(0, jumpForce));
        }
        else if (Input.GetButton("Cancel"))
        {
            mManagerInstance.PauseGameButton();
        }
    }

    /// <summary>
    /// Checks if game should end, and if it should, if player's current
    /// game points are enough to replace it's highscore.
    /// </summary>
    private void CheckGameOver()
    {
        if (!playerPawn.GetWasHit()) return;

        if (gamePoints > currentPlayer.GetComponent<PlayerData>().highScore)
        {
            currentPlayer.GetComponent<PlayerData>().highScore = gamePoints;
            DataManager.SavePlayerData(currentPlayer, DataManager.LoadPlayers().allPlayers);
        }

        foreach (Transform child in spawnPoint.transform) Destroy(child.gameObject);
        playerObject.SetActive(false);
        spawnPoint.SetActive(false);

        currentState = GameState.GameOver;
        mManagerInstance.ToggleGameOverPanel();
    }
            
    public void ObstacleDodged(int pointsValue)
    {
        gamePoints += pointsValue;
    }
}
