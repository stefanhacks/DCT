using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {
    private bool onGround;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlayerRoot")
            onGround = true;
    }

    internal void TryJump(Vector2 jumpVector)
    {
        if (!onGround) return;

        this.gameObject.GetComponent<Rigidbody2D>().AddForce(jumpVector);
        onGround = false;
    }
}
