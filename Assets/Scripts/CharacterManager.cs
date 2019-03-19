using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CharacterManager : MonoBehaviour {

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

    public void SetCurrentPlayer(PlayerData newPlayer)
    {
        currentPlayer = newPlayer;
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
    
    private void ChangePart(string part, bool next)
    {
        // Functions for customizing player appearance.
        // First finds current selected part.
        int currentPart = currentPlayer.bodyComposition[part.ToString()];
        int nextPart;

        // Logic that adds or reduces it's index based on request, but
        // also clamps the value to a minimum of 0 and maximum of array length.
        if (currentPart + 1 >= areaToArray[part].Length && next)
            nextPart = 0;
        else if (currentPart == 0 && !next)
            nextPart = areaToArray[part].Length - 1;
        else
            nextPart = (next) ? ++currentPart : --currentPart;

        // Refreshes and saves everything.
        currentPlayer.bodyComposition[part.ToString()] = nextPart;
        Sprite nextSprite = areaToArray[part][nextPart];

        this.GetComponent<GameMenuManager>().RefreshPlayerModel(part, nextSprite);
        DataManager.SavePlayerData(currentPlayer, data.allPlayers);
    }

    public void ChangePartNext(string part)
    {
        this.ChangePart(part, true);
    }

    public void ChangePartPrevious(string part)
    {
        this.ChangePart(part, false);
    }

    public bool CheckPlayerKey(string name)
    {
        // Returns if key exists.
        return (data != null && data.PlayerExists(name));
    }

    public List<string> GetPlayerNames()
    {
        // Returns player all keys.
        return data.allPlayers.Keys.ToList();
    }

    public void CreateNewPlayer(string newPlayerName)
    {
        // Function for Player creation. 
        // First replaces old PlayerData.
        Destroy(this.GetComponent<PlayerData>());
        PlayerData newPlayerData = this.gameObject.AddComponent<PlayerData>();

        // Randomizes values for the new Player.
        newPlayerData.playerName = newPlayerName;
        newPlayerData.bodyComposition["eyes"] = UnityEngine.Random.Range(0, eyesParts.Length);
        newPlayerData.bodyComposition["head"] = UnityEngine.Random.Range(0, headParts.Length);
        newPlayerData.bodyComposition["mouth"] = UnityEngine.Random.Range(0, mouthParts.Length);
        newPlayerData.bodyComposition["torso"] = UnityEngine.Random.Range(0, torsoParts.Length);
        newPlayerData.bodyComposition["legs"] = UnityEngine.Random.Range(0, legsParts.Length);

        // Saves data and replaces current player.
        // Following verification is only for the case there is no player base yet. 
        Dictionary<string, Dictionary<string, int>> playerBase = (data == null)
                ? new Dictionary<string, Dictionary<string, int>>()
                : data.allPlayers;

        DataManager.SavePlayerData(newPlayerData, playerBase);
        currentPlayer = newPlayerData;
        data = DataManager.LoadPlayers();
    }

    public void LoadPlayerWithKey(string playerKey)
    {
        // Replace old PlayerData.
        Destroy(this.GetComponent<PlayerData>());
        PlayerData newPlayerData = this.gameObject.AddComponent<PlayerData>();

        // Loads values, based on player key sent.
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

    internal KeyValuePair<string, Dictionary<string, int>>[] GetHighScores()
    {
        return (data == null) 
            ? new KeyValuePair<string, Dictionary<string, int>>[0]
            : data.GiveHighScores();
    }
}
