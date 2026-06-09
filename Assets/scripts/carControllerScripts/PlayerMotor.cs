using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    public float speed = 7;
    private Vector3 currentMovement = Vector3.zero;
    public float smoothness = 1f;
    private float gravity = -9.8f;
    private bool isGrounded;
    public float jumpHeight = 10f;
    float airSpeed = 1f;
    Vector2 currentInput;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            currentMovement = Vector3.Lerp(currentMovement, new Vector3(currentInput.x, 0, currentInput.y), Time.deltaTime * 20f * smoothness);
            if (playerVelocity.y < 0)
            playerVelocity.y = -2f;
        }
        else
        {
        currentMovement = Vector3.Lerp(currentMovement, new Vector3(currentInput.x, 0, currentInput.y) * 0.7f, Time.deltaTime * 2f);
        playerVelocity.y += gravity * Time.deltaTime;
        }

        Vector3 finalMove = transform.TransformDirection(currentMovement) * speed + playerVelocity;
        controller.Move(finalMove * Time.deltaTime);

    }

    public void processJump()
    {
        if (isGrounded)
        {
            playerVelocity.y = jumpHeight;
        }
    }



    public void processMove(Vector2 input)
    {
        currentInput = input;
    }
}
