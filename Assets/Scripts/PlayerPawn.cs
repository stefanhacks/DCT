using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for animating and controlling the player pawn,
/// a Game Object used in the game loop as the Player's Avatar.
/// </summary>
public class PlayerPawn : MonoBehaviour {
    private bool onGround, wasHit;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Must collide with Root to stablish ground.
        if (collision.gameObject.tag == "PlayerRoot")
        {
            onGround = true;
            this.GetComponent<Animator>().SetBool("OnGround", true);
        }   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        wasHit = true;
    }

    /// <summary>
    /// Returns if the Player was hit. Callable function used instead of
    /// straight OnCollision call for better understanding of gameloop flow.
    /// </summary>
    public bool GetWasHit()
    {
        return wasHit;
    }

    public void SetWasHit(bool value)
    {
        wasHit = value;
    }

    /// <summary>
    /// Tries to make the Game Pawn jump. Will fail, with no feedback nor
    /// exception, if the player is not in contact with the player base.
    /// </summary>
    internal void TryJump(Vector2 jumpVector)
    {
        // Only jumps if on ground.
        if (!onGround) return;

        this.GetComponent<Animator>().SetBool("OnGround", false);
        this.gameObject.GetComponent<Rigidbody2D>().AddForce(jumpVector);
        onGround = false;
    }
}
