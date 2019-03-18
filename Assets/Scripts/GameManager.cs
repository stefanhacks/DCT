using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(GameMenuManager), typeof(PlayerData))]
public class GameManager : MonoBehaviour {

    public Sprite[] eyesParts, headParts, mouthParts, torsoParts, legsParts;
    public Dictionary<string, Sprite[]> areaToArray;

    private GameData data;
    private PlayerData currentPlayer;

    private void Awake()
    {
        // Loads all players and sets the current based on the last one played.
        data = DataManager.LoadPlayers();
        if (data != null)
        {
            currentPlayer = this.gameObject.GetComponent<PlayerData>();
            currentPlayer.LoadFromData(data.lastPlayer, data.allPlayers[data.lastPlayer]);
        }
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

    public Sprite[] GetPlayerSprites()
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
        return (data != null && data.PlayerExists(name));
    }

    public void CreateNewPlayer(string newPlayerName)
    {
        // Replaces old PlayerData.
        Destroy(this.GetComponent<PlayerData>());
        PlayerData newPlayerData = this.gameObject.AddComponent<PlayerData>();

        // Randomizes values for new Player.
        newPlayerData.playerName = newPlayerName;
        newPlayerData.bodyComposition["eyes"] = UnityEngine.Random.Range(0, eyesParts.Length);
        newPlayerData.bodyComposition["head"] = UnityEngine.Random.Range(0, headParts.Length);
        newPlayerData.bodyComposition["mouth"] = UnityEngine.Random.Range(0, mouthParts.Length);
        newPlayerData.bodyComposition["torso"] = UnityEngine.Random.Range(0, torsoParts.Length);
        newPlayerData.bodyComposition["legs"] = UnityEngine.Random.Range(0, legsParts.Length);

        // Saves data, replaces current player and deletes mock object.
        Dictionary<string, Dictionary<string, int>> playerBase = (data == null)
                ? new Dictionary<string, Dictionary<string, int>>()
                : data.allPlayers;

        DataManager.SavePlayerData(newPlayerData, playerBase);
        currentPlayer = newPlayerData;
        data = DataManager.LoadPlayers();
    }

    public List<string> GetPlayerNames()
    {
        return data.allPlayers.Keys.ToList();
    }

    public void LoadPlayerWithKey(string playerKey)
    {
        // Replace old PlayerData.
        Destroy(this.GetComponent<PlayerData>());
        PlayerData newPlayerData = this.gameObject.AddComponent<PlayerData>();

        // Loads values.
        newPlayerData.LoadFromData(playerKey, data.allPlayers[playerKey]);
        currentPlayer = newPlayerData;
    }

    public void DeleteCurrentPlayer()
    {
        // On the event a player is deleted, another one must be loaded. Although
        // First() method doesn't reliably gets the first item on dict, this proves 
        // to be irrelevant on this case - all we need is another player anyway.

        // Removes player key from player dict.
        data.allPlayers.Remove(currentPlayer.playerName);
         
        // However, an exception is thrown if we try to access this value in the 
        // case for an empty dictionary, so this can only run on a Count > 0;
        if (data.allPlayers.Count > 0)
        {
            LoadPlayerWithKey(data.allPlayers.First().Key);
            DataManager.SavePlayerData(currentPlayer, data.allPlayers);
        }
        else
        {   
            // If Count == 0, it was the last player, so Save File must go as well.
            DataManager.DeleteSaveFile();
            data = null;
            currentPlayer = null;
        }
    }
}
