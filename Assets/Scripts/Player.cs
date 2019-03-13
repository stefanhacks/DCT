using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public string playerName = "Player";
    public int highScore = 0;
    public Dictionary <string, int> bodyComposition = new Dictionary<string, int>() {
            {"eyes", 0},
            {"mouth", 0},
            {"head", 0},
            {"torso", 0},
            {"legs", 0}
        };
}
