using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float vertRotation = 0f;

    public float xsensitivity = 30f;
    public float ysensitivity = 30f;

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;


        vertRotation -= mouseY * Time.deltaTime * ysensitivity;
        vertRotation = Mathf.Clamp(vertRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(vertRotation, 0, 0);

        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xsensitivity);
    }
}
