using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [Tooltip("How long player must play before increase in difficulty.")]
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
        gAreaAnimator.SetReferences(groundObject, difficultyRamp * 4);
    }

    private void Update()
    {
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
        // Pausing stuff;
        Time.timeScale = 0;
        playerObject.SetActive(false);
        spawnPoint.SetActive(false);
        currentState = GameState.Paused;
    }

    private void PrepareGame(Sprite[] playerSprites)
    {
        // Must alter player object and activate spawn point.
        RefreshPlayerObject(playerSprites);
        playerObject.SetActive(true);
        spawnPoint.SetActive(true);

        // If previous game state was "Paused", needs to reset timeScale.
        // If it wasn't, needs to reset all relevant gameplay variables.
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

        // Change state.
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

    private void DoSpawning()
    {
        // Applies frame interval to timers.
        playingDuration += Time.deltaTime;
        innerTimer += Time.deltaTime;

        // Checks if obstacle needs to be spawned and runs gameplay timer logic.
        if (innerTimer > currentSpawnInterval)
        {
            // Checks for how long player has been playing,
            if (playingDuration > difficultyRamp * currentDifficulty && currentDifficulty < 3)
            {
                currentDifficulty += 1;
                currentSpawnInterval = initialSpawnInterval;
            }

            // MaxRange equals = 1, 3, 5, 7
            int maxRange = 1 + (currentDifficulty * 2);
            // When x/2 equals = 0, 0, 1, 1, 2, 2, 3
            int spawnChoice = UnityEngine.Random.Range(0, maxRange) / 2;

            GameObject obstacle = Instantiate(obstaclePrefabs[spawnChoice], spawnPoint.transform);
            if (spawnChoice == 3 && UnityEngine.Random.value > 0.5f)
                obstacle.GetComponent<ObstaclePawn>().SetFade();

            if (currentDifficulty == 3 && UnityEngine.Random.value < 0.8f)
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

    private void CheckPlayerInput()
    {
        // Do Input managing. Although this will result in a rigidbody 
        // being altered, this is left handled by regular Update as 
        // to avoid input loss, not being a continuous button press check.
        if (Input.GetButtonDown("Jump"))
        {
            playerPawn.TryJump(new Vector2(0, jumpForce));
        }
        // Since mouse play is enabled, checks if UI is being clicked.
        // If it is, player is pausing the game, so don't jump.
        else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            playerPawn.TryJump(new Vector2(0, jumpForce));
        }
        else if (Input.GetButton("Cancel"))
        {
            mManagerInstance.PauseGameButton();
        }
    }

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
