using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// Class used for storing and serializing saved players from the game.

// Decided to save players as a single dictionary instead of in different
// objects, which could have resulted in several different save files.
// In the case for a more complex game, with more data regarding each
// player this would be preferable and safer - for the case of save data corruption, 
// for instance. In this instance , however, it does allow me to ignore things such as 
// finding specific save files and figuring how many there are in the folder.
[System.Serializable]
public class GameData {
    public Dictionary<string, Dictionary<string, int>> allPlayers;
    public string lastPlayer;

    public GameData(PlayerData currentPlayer, Dictionary<string, Dictionary<string, int>> playerBase)
    {
        // Updates playerBase dict, for players that might have been deleted or created.
        allPlayers = playerBase;

        // Updates current player data.
        lastPlayer = currentPlayer.playerName;
        Dictionary<string, int> currentPlayerData = new Dictionary<string, int>() {
            {"highscore", currentPlayer.highScore},
            {"eyes", currentPlayer.bodyComposition["eyes"]},
            {"head", currentPlayer.bodyComposition["head"]},
            {"mouth", currentPlayer.bodyComposition["mouth"]},
            {"torso", currentPlayer.bodyComposition["torso"]},
            {"legs", currentPlayer.bodyComposition["legs"]}
        };

        // Adds or updates already existing entry.
        if (allPlayers.ContainsKey(lastPlayer))
            allPlayers[lastPlayer] = currentPlayerData;
        else
            allPlayers.Add(lastPlayer, currentPlayerData);
    }

    /// <summary>
    /// Checks and returns if a Player exists in the database, using it's key as reference.
    /// </summary>
    internal bool PlayerExists(string name)
    {
        // Returns if provided name string is a key in the dictionary. Also checks if it exists.
        if (allPlayers == null)
            return false;
        else 
            return allPlayers.Keys.Where(n => n.ToLower() == name.ToLower()).Count() != 0;
    }

    /// <summary>
    /// Orders player database by highscore, taking the top three and returning them as an array.
    /// Will also return an empty array if the save file doesn't exist.
    /// </summary>
    public KeyValuePair<string, Dictionary<string, int>>[] GiveHighScores()
    {
        return allPlayers.OrderByDescending(s => s.Value["highscore"]).Take(3).DefaultIfEmpty().ToArray();
    }
}