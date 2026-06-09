using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputActions playerInput;
    public InputActions.OnFootActions onFoot;
    private PlayerMotor motor;
    private PlayerLook look;
    // Start is called before the first frame update
    void Awake()
    {
        look = GetComponent<PlayerLook>();
        motor = GetComponent<PlayerMotor>();
        playerInput = new InputActions();
        onFoot = playerInput.OnFoot;
        onFoot.Jump.performed += ctx => motor.processJump();
    }

    // Update is called once per frame
    void Update()
    {
        motor.processMove(onFoot.Movement.ReadValue<Vector2>());
    }
    void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>()); 
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}
