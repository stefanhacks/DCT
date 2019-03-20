using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple class for eliminating obstacles that leave the screen. Relies on Trigger Collision to do so.
/// </summary>
public class ObstacleLimiter : MonoBehaviour {

    private GameManager gmInstance;

    private void Start()
    {
        gmInstance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            ObstaclePawn obstaclePawn = collision.gameObject.GetComponent<ObstaclePawn>();
            Destroy(collision.gameObject, 1);
            gmInstance.ObstacleDodged(obstaclePawn.pointsAwarded);
        }   
    }
}
