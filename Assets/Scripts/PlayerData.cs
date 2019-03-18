using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class used for using data from a specific player.
public class PlayerData : MonoBehaviour {

    [HideInInspector]
    public string playerName = "Player";
    [HideInInspector]
    public int highScore = 0;

    public Dictionary <string, int> bodyComposition = new Dictionary<string, int>() {
            {"eyes", 0}, {"head", 0}, {"mouth", 0}, {"torso", 0}, {"legs", 0}
        };
    
    public void LoadFromData(string nameArg, Dictionary<string, int> features)
    {
        this.playerName = nameArg;
        this.highScore = features["highscore"];
        this.bodyComposition["eyes"] = features["eyes"];
        this.bodyComposition["head"] = features["head"];
        this.bodyComposition["mouth"] = features["mouth"];
        this.bodyComposition["torso"] = features["torso"];
        this.bodyComposition["legs"] = features["legs"];
    }
}
