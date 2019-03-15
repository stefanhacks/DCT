using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private GameData data;
    private PlayerData currentPlayer;

    private void Awake()
    {
        // Loads all players and sets the current based on the last one played.
        data = DataManager.LoadPlayers();
        currentPlayer = this.gameObject.GetComponent<PlayerData>();
        currentPlayer.LoadFromData(data.lastPlayer, data.allPlayers[data.lastPlayer]);
    }

    public PlayerData GetCurrentPlayer()
    {
        return currentPlayer;
    }
}
