using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 6f;
    public float height = 3f;
    public float positionSmoothSpeed = 5f;
    public float rotationSmoothSpeed = 2f; // lower = more lag on turns

    private float currentYRotation;

    void LateUpdate()
    {
        // lazily follow car's Y rotation only (ignore pitch/roll)
        currentYRotation = Mathf.LerpAngle(
            currentYRotation, 
            target.eulerAngles.y, 
            rotationSmoothSpeed * Time.deltaTime
        );

        // calculate position based on lagged rotation
        Quaternion rotation = Quaternion.Euler(0, currentYRotation, 0);
        Vector3 desiredPosition = target.position 
            - (rotation * Vector3.forward) * distance 
            + Vector3.up * height;

        // smooth position
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            positionSmoothSpeed * Time.deltaTime
        );

        // always look at car
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}