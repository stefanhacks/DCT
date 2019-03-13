using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GameData {
    public Dictionary<string, Dictionary<string, int>> allPlayers;

    public GameData(Player currentPlayer, Dictionary<string, Dictionary<string, int>> playerBase) {
        // Updates playerBase dict, for players that might have been deleted.
        allPlayers = playerBase;

        // Updates current player data.
        Dictionary<string, int> currentPlayerData = new Dictionary<string, int>() {
            {"highscore", currentPlayer.highScore},
            {"eyes", currentPlayer.bodyComposition["eyes"]},
            {"mouth", currentPlayer.bodyComposition["mouth"]},
            {"head", currentPlayer.bodyComposition["head"]},
            {"torso", currentPlayer.bodyComposition["torso"]},
            {"legs", currentPlayer.bodyComposition["legs"]}
        };

        if (allPlayers.ContainsKey(currentPlayer.name))
            allPlayers[currentPlayer.playerName] = currentPlayerData;
        else
            allPlayers.Add(currentPlayer.playerName, currentPlayerData);
    }
}