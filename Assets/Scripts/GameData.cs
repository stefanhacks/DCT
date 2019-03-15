using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Class used for storing and serializing saved players from the game.
[System.Serializable]
public class GameData {
    public Dictionary<string, Dictionary<string, int>> allPlayers;
    public string lastPlayer;

    public GameData(PlayerData currentPlayer, Dictionary<string, Dictionary<string, int>> playerBase)
    {
        // Updates playerBase dict, for players that might have been deleted.
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
}