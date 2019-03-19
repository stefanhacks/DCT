using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour {

    [Tooltip("Points awarded for dodging the obstacle.")]
    [Range(1,3)]
    public int pointsAwarded = 0;

    [Header("Object Movement Variables")]
    [Tooltip("Defines it's speed and height of movement.")]
    [Range(1f, 70f)]
    public int horizontalSpeed = 10;
    [Range(1f, 30f)]
    public int verticalAmplitude = 5;

    [Tooltip("Defines a value that acts as a limiter on a Random Range, " +
        "used in a percentage which alters object speed to provide some game variance.")]
    [Range(0f, 10f)]
    public float variation = 1;

    [Tooltip("Defines how this obstacle moves.\n" +
        "'Straight' moves it along a horizontal line.\n" +
        "'Wave' simulates a sinusoidal wave.\n" +
        "'Weird' tries to do something funky.")]
    public TypeOfMovement obstacleType = TypeOfMovement.Straight;
    public enum TypeOfMovement { Straight, Wave, Jumping, Weird }

    private float actingSpeed, actingAmplitude;
    private Transform detailObject;
    
    private void Start()
    {
        // Initializing variables.
        detailObject = this.transform.GetChild(0);

        // Variation logic.
        // Mixes speed up and rotates obstacle appearance somewhat.
        float actualVariation = Random.Range(-variation, variation);
        Vector3 detailRotation = new Vector3(Random.Range(0, 45), Random.Range(0, 45), Random.Range(0, 45));

        actingSpeed = horizontalSpeed * (100 + actualVariation ) / 100;
        detailObject.Find("Details").rotation *= Quaternion.Euler(detailRotation);
    }

    private void Update()
    {
        // Move;
        switch (obstacleType)
        {
            case TypeOfMovement.Straight:
                this.transform.Translate(-actingSpeed * Time.deltaTime, 0, 0);
                break;
            case TypeOfMovement.Wave:
                break;
            case TypeOfMovement.Jumping:
                break;
            case TypeOfMovement.Weird:
                break;
        }
    }

    private void FixedUpdate()
    {
        // Runs on Fixed Update, as recommended for rigidbody interaction.
        detailObject.Rotate(0, 0, actingSpeed / 1.5f);
    }
}