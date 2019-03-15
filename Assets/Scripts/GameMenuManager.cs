using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour {

    public GameObject SelectPlayerMenu;

    private PlayerData currentPlayer;
    private Text nameField, scoreField;

    public void Start()
    {
        // Runs after Awake, letting GameManager parent load everything first.
        nameField = GameObject.FindGameObjectWithTag("PPNameField").GetComponent<Text>();
        scoreField = GameObject.FindGameObjectWithTag("PPScoreField").GetComponent<Text>();

        // Updates based on the player.
        currentPlayer = this.gameObject.GetComponent<GameManager>().GetCurrentPlayer();
        nameField.text = currentPlayer.playerName;
        scoreField.text = currentPlayer.highScore.ToString();
    }
}
