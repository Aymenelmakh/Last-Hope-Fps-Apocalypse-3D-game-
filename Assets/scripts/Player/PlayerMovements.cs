using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    [Header("Player Movement and gravity")]
    public float PlayerWalkSpeed = 2.5f;
    public float jumpForce = 1f;
    private CharacterController CC;
    public float gravity = -9.81f;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.5f; // the distance between the player feets and ground
    private bool isGrounded;
    private Vector3 velocity;
    public Animator animator;

    [Header("foot steps")]
    public AudioSource leftFootAudioSource;
    public AudioSource rightFootAudioSource;
    public AudioClip[] footstepsSounds;
    public float footstepInterval = 0.5f;
    private float nextFootStepTime;
    private bool isLeftFootStep = true;

    [Header("Jump Sound Effect")]
    public AudioSource JumpSource;
    public AudioClip[] JumpSounds;

    [Header("Land Sound Effect")]
    public AudioSource LandSource;
    public AudioClip[] LandSounds;
    private bool wasGrounded;
    private int lastEquippedWeapon = -1;
    public ShootingController sc;

    Transform GetActifWeapon()
    {
        foreach(Transform child in transform)
        {
            if (child.gameObject.activeSelf && child.transform.name != "GroundCheck")
            {
                return child;
            }
        }
        return null;
    }

    void Start()
    {
        CC = GetComponent<CharacterController>();
        Transform Weapon = GetActifWeapon();
        if (Weapon != null)
        {
            animator = Weapon.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("No active weapon found on Start!");
        }
    }
    void Update()
    {
        bool groundedThisFrame = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // Player just landed this frame
        if (groundedThisFrame && !wasGrounded)
        {
            PlayerLandSound(); //  only plays when touching ground
        }
        wasGrounded = groundedThisFrame;
        isGrounded = groundedThisFrame;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        PlayerMove();
        HandleGravity();
        // handle footsteps
        if (isGrounded && CC.velocity.magnitude > 0.1f && Time.time >= nextFootStepTime)
        {
            PlayerFootstepSound();
            nextFootStepTime = Time.time + footstepInterval;
        }
        // Handle jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            PlayerJumpSound();
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
            velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
        }
        CC.Move(velocity * Time.deltaTime);

        // Switch Guns
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(1);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(2);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        { 
            SwitchWeapon(3);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(4);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha5)) SwitchWeapon(5);
        else if(Input.GetKeyDown(KeyCode.Alpha6)) SwitchWeapon(6);
        else if(Input.GetKeyDown(KeyCode.Alpha7)) SwitchWeapon(7);
    }

    public void PlayerMove()
    {   
        // Inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        //  move direction
        Vector3 move = transform.right * x + transform.forward * z;
        move.y = 0f;
        CC.Move(move * PlayerWalkSpeed * Time.deltaTime);
        if (animator == null || animator.runtimeAnimatorController == null) return; 
        // set animation based on movement
        if (x != 0 || z != 0)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
        }
    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    void PlayerFootstepSound()
    {
        AudioClip footstepClip = footstepsSounds[Random.Range(0, footstepsSounds.Length)];

        if (isLeftFootStep)
        {
            leftFootAudioSource.PlayOneShot(footstepClip);
        }
        else{
            rightFootAudioSource.PlayOneShot(footstepClip);
        }
        isLeftFootStep = !isLeftFootStep;
    }
    void PlayerJumpSound()
    {
        AudioClip JumpClip = JumpSounds[Random.Range(0, JumpSounds.Length)];
        JumpSource.PlayOneShot(JumpClip);
    }

    void PlayerLandSound()
    {
        AudioClip LandClip = LandSounds[Random.Range(0, LandSounds.Length)];
        LandSource.PlayOneShot(LandClip);
    }

    void SwitchWeapon(int nb)
    {
        if (nb == lastEquippedWeapon) return; 
        foreach(Transform gun in transform)
        {
            string gun_name = gun.transform.name;
            if (gun_name != "GroundCheck")
            {
                int Weapon_Number = int.Parse(gun_name.Substring(3));
                if (nb == Weapon_Number)
                {
                    gun.gameObject.SetActive(true);
                    if (nb != 1)
                    {
                        sc = gun.GetComponent<ShootingController>();
                        AudioClip pickupSound = sc.RiflePickClip;
                        if (pickupSound != null)
                        {
                            JumpSource.PlayOneShot(pickupSound);
                        }
                    }
                    animator = gun.GetComponent<Animator>();
                    if (nb == 3 || nb == 4)
                    {
                        sc.enabled = false;
                        animator.SetBool("Idle", false);
                        animator.SetTrigger("Reload");
                        StartCoroutine(EnableShootingAfterReload());
                    }
                    lastEquippedWeapon = nb;
                }
                else
                {
                    gun.gameObject.SetActive(false);
                }
            }
        }
    }
    IEnumerator EnableShootingAfterReload()
    {
        yield return null; // wait one frame so animator transitions to reload state
        float reloadLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(reloadLength);
        if (sc != null)
            sc.enabled = true;
    }
}
