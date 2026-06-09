using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarInteractable : Interactable
{
    public CarController carController;
    private bool inCar = false;
    public Camera carCamera;
    public Transform player;
    public GameObject playerCamera;
    public PlayerMotor playerMotor;
    public Canvas playerUi;
    public Transform exitPoint;
    public PlayerLook playerLook;


    void Start()
    {
        playerMotor = FindObjectOfType<PlayerMotor>();
        playerLook = FindObjectOfType<PlayerLook>();
    }

   
    void Awake()
    {
        carController = GetComponent<CarController>();
        playerMotor = FindObjectOfType<PlayerMotor>();
    }
    
    void Update()
    {
        if(inCar && Input.GetKeyDown(KeyCode.E))
        {
            Exit();
        }
    }

    protected override void Interact()
    {
        base.Interact();
        if (!inCar)
        {
            Enter();
        }
    }


    private void Enter()
    {
        inCar = true;
        carCamera.gameObject.SetActive(true);
        carCamera.enabled = true;

        player.gameObject.SetActive(false);
        playerLook.enabled = false;
        Cursor.lockState = CursorLockMode.None;

        playerCamera.SetActive(false);

        carController.playerInCar = true;
        playerMotor.enabled = false;
        playerUi.gameObject.SetActive(false);
    }

    private void Exit()
    {
        inCar = false;
        carCamera.gameObject.SetActive(false);
        carCamera.enabled = false;

        player.gameObject.SetActive(true);
        CharacterController cc = player.GetComponent<CharacterController>();
        cc.enabled = false;
        player.position = exitPoint.position;
        cc.enabled = true;
        
        playerLook.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;

        playerCamera.SetActive(true);

        carController.playerInCar = false;
        playerMotor.enabled = true;
        playerUi.gameObject.SetActive(true);
    }
}
