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

        this.gameObject.GetComponent<GameMenuManager>().RefreshPlayerModel(new Sprite[] {
            eyesParts[currentPlayer.bodyComposition["eyes"]],
            headParts[currentPlayer.bodyComposition["head"]],
            mouthParts[currentPlayer.bodyComposition["mouth"]],
            torsoParts[currentPlayer.bodyComposition["torso"]],
            legsParts[currentPlayer.bodyComposition["legs"]]
        });
    }

    public PlayerData GetCurrentPlayer()
    {
        return currentPlayer;
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
}
