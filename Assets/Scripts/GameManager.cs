using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameMenuManager), typeof(PlayerData))]
public class GameManager : MonoBehaviour {

    public Sprite[] eyesParts, headParts, mouthParts, torsoParts, legsParts;
    public Dictionary<string, Sprite[]> areaToArray;

    private GameData data;
    private PlayerData currentPlayer;

    private void Awake()
    {
        // If save file doesn't exist yet, must create it with a default player.
        if (DataManager.LoadPlayers() == null)
        {
            GameObject emptyPlayer = new GameObject("emptyPlayer");
            emptyPlayer.AddComponent<PlayerData>();

            DataManager.SavePlayerData(
                emptyPlayer.GetComponent<PlayerData>(),
                new Dictionary<string, Dictionary<string, int>>()
                );

            Destroy(emptyPlayer);
        }

        // Loads all players and sets the current based on the last one played.
        data = DataManager.LoadPlayers();
        currentPlayer = this.gameObject.GetComponent<PlayerData>();
        currentPlayer.LoadFromData(data.lastPlayer, data.allPlayers[data.lastPlayer]);
    }
    
    public void Start()
    {
        areaToArray = new Dictionary<string, Sprite[]>()
        {
            {"eyes", eyesParts},
            {"head", headParts},
            {"mouth", mouthParts},
            {"torso", torsoParts},
            {"legs", legsParts},
        };
    }

    public PlayerData GetCurrentPlayer()
    {
        return currentPlayer;
    }

    internal Sprite[] GetPlayerSprites()
    {
        return new Sprite[] {
            eyesParts[currentPlayer.bodyComposition["eyes"]],
            headParts[currentPlayer.bodyComposition["head"]],
            mouthParts[currentPlayer.bodyComposition["mouth"]],
            torsoParts[currentPlayer.bodyComposition["torso"]],
            legsParts[currentPlayer.bodyComposition["legs"]]
        };
    }
    
    public void SetCurrentPlayer(PlayerData newPlayer)
    {
        currentPlayer = newPlayer;
    }

    public void ChangePartNext(string part)
    {
        this.ChangePart(part, true);
    }

    public void ChangePartPrevious(string part)
    {
        this.ChangePart(part, false);
    }

    private void ChangePart(string part, bool next)
    {
        int currentPart = currentPlayer.bodyComposition[part.ToString()];
        int nextPart;

        if (currentPart + 1 >= areaToArray[part].Length && next)
            nextPart = 0;
        else if (currentPart == 0 && !next)
            nextPart = areaToArray[part].Length - 1;
        else
            nextPart = (next) ? ++currentPart : --currentPart;

        currentPlayer.bodyComposition[part.ToString()] = nextPart;
        Sprite nextSprite = areaToArray[part][nextPart];

        this.GetComponent<GameMenuManager>().RefreshPlayerModel(part, nextSprite);
        DataManager.SavePlayerData(currentPlayer, data.allPlayers);
    }

    public bool CheckPlayerKey(string name)
    {
        return data.PlayerExists(name);
    }

    internal void CreateNewPlayer(string newPlayerName)
    {
        // Creates mock object to house new PlayerData.
        GameObject newPlayer = new GameObject(newPlayerName);
        PlayerData newPlayerData = newPlayer.AddComponent<PlayerData>();

        // Randomizes values for new Player.
        newPlayerData.playerName = newPlayerName;
        newPlayerData.bodyComposition["eyes"] = Random.Range(0, eyesParts.Length);
        newPlayerData.bodyComposition["head"] = Random.Range(0, headParts.Length);
        newPlayerData.bodyComposition["mouth"] = Random.Range(0, mouthParts.Length);
        newPlayerData.bodyComposition["torso"] = Random.Range(0, torsoParts.Length);
        newPlayerData.bodyComposition["legs"] = Random.Range(0, legsParts.Length);

        // Saves data, replaces current player and deletes mock object.
        DataManager.SavePlayerData(newPlayerData, data.allPlayers);
        currentPlayer = newPlayerData;
        Destroy(newPlayer);
    }
}
