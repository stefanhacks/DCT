using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclePawn : MonoBehaviour {

    [Header("Generic Obstacle Variables")]
    [Tooltip("Points awarded for dodging the obstacle.")]
    [Range(1,3)]
    public int pointsAwarded = 0;
    [Tooltip("Defines a value that acts as a limiter on a Random Range, " +
        "used in a percentage which alters object speed to provide some game variance.")]
    [Range(0f, 10f)]
    public float variation = 1;

    [Header("Obstacle Movement Variables")]
    [Tooltip("Defines object's horizontal speed and height.")]
    [Range(1f, 20)]
    public int horizontalSpeed = 10;
    [Range(0f, 8f)]
    public float verticalOffset = 2;

    [Header("Wave Pattern Variables")]
    [Tooltip("Defines settings for the Wave style of movement.")]
    [Range(0f, 20f)]
    public float waveAmplitude = 0;
    [Range(0f, 20f)]
    public float waveFrequency = 0;

    [Tooltip("Defines how this obstacle moves.\n" +
        "'Straight' moves it along a horizontal line.\n" +
        "'Wave' simulates a sinusoidal wave.\n" +
        "'Weird' tries to do something funky.")]
    public TypeOfMovement obstacleType = TypeOfMovement.Straight;
    public enum TypeOfMovement { Straight, Wave, Weird, Fading }

    private bool canMove = true, hasStopped = false, fades = false;
    private float weirdTimer = 0, fadingTimer = 0;
    private float actingSpeed, actingOffset, actingAmplitude, actingFrequency;
    private Transform detailObject;
    
    private void Start()
    {
        // Initializing references.
        detailObject = this.transform.GetChild(0);
        
        // Writes correct values.
        // Mixes speed up and rotates obstacle appearance somewhat.
        actingSpeed = horizontalSpeed * (100 + Random.Range(-variation, variation)) / 100;
        actingOffset = verticalOffset * (100 + Random.Range(-variation, variation)) / 100;
        actingAmplitude = waveAmplitude * (100 + Random.Range(-variation, variation)) / 100;
        actingFrequency = waveFrequency * (100 + Random.Range(-variation, variation)) / 100;

        Vector3 detailRotation = new Vector3(Random.Range(0, 45), Random.Range(0, 45), Random.Range(0, 45));
        detailObject.Find("Details").rotation *= Quaternion.Euler(detailRotation);

        // Offsets it, if needs be;
        if (verticalOffset != 0)
            this.transform.position = new Vector3(
                this.transform.position.x,
                this.transform.position.y + actingOffset,
                this.transform.position.z);
    }

    private void Update()
    {
        switch (obstacleType)
        {
            case TypeOfMovement.Straight:
                this.transform.Translate(-actingSpeed * Time.deltaTime, 0, 0);
                break;
            case TypeOfMovement.Wave:
                this.transform.Translate(
                    -actingSpeed * Time.deltaTime,
                    actingAmplitude * Mathf.Sin(Time.time * actingFrequency) * Time.deltaTime,
                    0);
                break;
            case TypeOfMovement.Weird:
                if (canMove)
                {
                    weirdTimer += Time.deltaTime;

                    if (!hasStopped && weirdTimer > 1)
                    {
                        // Act Weird
                        weirdTimer = 0;
                        canMove = false;
                        hasStopped = true;
                        StartCoroutine(WaitDuringWeird(Random.Range(0f,1.5f)));
                    }
                    else
                    {
                        this.transform.Translate(-actingSpeed * Time.deltaTime, 0, 0);
                    }
                }
                break;
            case TypeOfMovement.Fading:
                this.transform.Translate(-actingSpeed * Time.deltaTime, 0, 0);
                if (fades)
                {
                    fadingTimer += Time.deltaTime;
                    if (fadingTimer > 0.7f)
                    {
                        StartCoroutine(Disappear(1));
                    }
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        // Runs on Fixed Update, as recommended for rigidbody interaction.
        detailObject.Rotate(0, 0, actingSpeed / 1.5f);
    }

    private IEnumerator WaitDuringWeird(float duration)
    {
        yield return new WaitForSeconds(duration);
        canMove = true;        
    }

    private IEnumerator Disappear(float duration)
    {
        float alpha = transform.GetComponent<Renderer>().material.color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            Color newColor = transform.GetComponent<Renderer>().material.color;
            newColor.a = Mathf.Lerp(alpha, 0, t);
            transform.GetComponent<Renderer>().material.color = newColor;
            yield return null;
        }

        Destroy(this.gameObject);
    }

    public void SetFade()
    {
        fades = true;
        Destroy(this.transform.Find("Shadow").gameObject);
        Destroy(this.GetComponent<CircleCollider2D>());
    }
}