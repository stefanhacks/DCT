using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : MonoBehaviour {
    private bool onGround, wasHit;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Must collide with Root to stablish ground.
        if (collision.gameObject.tag == "PlayerRoot")
            onGround = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        wasHit = true;
    }

    public bool GetWasHit()
    {
        return wasHit;
    }

    public void SetWasHit(bool value)
    {
        wasHit = value;
    }

    internal void TryJump(Vector2 jumpVector)
    {
        // Only jumps if on ground.
        if (!onGround) return;

        this.gameObject.GetComponent<Rigidbody2D>().AddForce(jumpVector);
        onGround = false;
    }
}
