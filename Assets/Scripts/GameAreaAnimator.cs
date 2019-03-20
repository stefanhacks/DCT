using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAreaAnimator : MonoBehaviour {

    private int groundToAngle;
    private float lerpTimer = 0, fullCycle;
    private Quaternion originalGroundRotation, groundFromRotation, groundTargetRotation;
    private GameObject groundObject;

    public void SetReferences(GameObject ground, float cycle)
    {
        this.groundObject = ground;
        this.fullCycle = cycle;
        this.originalGroundRotation = groundObject.transform.rotation;
    }

    public void Reset()
    {
        lerpTimer = 0;
        groundToAngle = 20;
        groundFromRotation = originalGroundRotation;
        groundTargetRotation = Quaternion.AngleAxis(groundToAngle, Vector3.forward);
    }

    public void UpdatePlayArea()
    {
        lerpTimer += Time.deltaTime;

        if (groundObject.transform.rotation == groundTargetRotation)
        {
            lerpTimer = 0;
            groundFromRotation = groundTargetRotation;
            groundToAngle = (groundToAngle == 20) ? 0 : 20;
        }

        groundTargetRotation = Quaternion.AngleAxis(groundToAngle, Vector3.forward);

        float rotationTotal = lerpTimer / fullCycle;
        groundObject.transform.rotation =
            Quaternion.Lerp(groundFromRotation, groundTargetRotation, rotationTotal);
    }
}
