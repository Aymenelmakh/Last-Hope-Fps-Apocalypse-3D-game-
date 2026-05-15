using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovements : MonoBehaviour
{
    [Header("Player Health and Damage")]
    private int maxHealth = 100;
    public int currentHealth;
    // UI and deathScreen
    [SerializeField] private Image filler;
    [SerializeField] private Text HealthState;
    public float minValue = 0f;
    public float maxValue = 100f;
    [Header("Player Movement and gravity")]
    public float PlayerWalkSpeed = 2.5f;
    public float PlayerSprintSpeed = 4f;
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
    public DeathScreen death;

    [Header("Player hurt sound")]
    public AudioClip PlayerHurtClip;
    public AudioSource HurtSource;
    public float hurtSoundCooldown = 0.4f;
    private float lastHurtSoundTime = -999f;
    public DeathScreen ds;

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
        currentHealth = maxHealth;
        filler.fillAmount = 1;
        HealthState.text = "100 hp";
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
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !ds.isShowed)
        {
            PlayerJumpSound();
            animator.SetBool("Walk", false);
            animator.SetBool("Idle", true);
            velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
        }
        CC.Move(velocity * Time.deltaTime);

        // Switch Guns
        if (!ds.isShowed)
        {
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
            // else if(Input.GetKeyDown(KeyCode.Alpha8)) SwitchWeapon(8);
        }
    }

    public void PlayerMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        move.y = 0f;
        if (move.magnitude > 1f)
            move = move.normalized;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        if (isSprinting)
        {
            footstepInterval = 0.3f;
        }
        else
        {
            footstepInterval = 0.5f;
        }
        float speed = isSprinting ? PlayerSprintSpeed : PlayerWalkSpeed;

        CC.Move(move * speed * Time.deltaTime);

        if (animator == null || animator.runtimeAnimatorController == null) return;

        if (x != 0 || z != 0)
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Walk", !isSprinting); //  walk when moving without shift
            animator.SetBool("Run", isSprinting);   //  run only when shift held
        }
        else
        {
            //  standing still — clear everything
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
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
                     // Enable only this weapon's camera
                    Camera weaponCam = gun.GetComponentInChildren<Camera>(true);
                    if (weaponCam != null)
                        weaponCam.enabled = true;
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
                    if (nb == 5)
                    {
                        sc = gun.GetComponent<ShootingController>();
                        sc.DamageAmount = 50f;
                        sc.maxDistance = 300f;
                    }
                    lastEquippedWeapon = nb;
                }
                else
                {
                    // Disable camera of inactive weapons
                    Camera weaponCam = gun.GetComponentInChildren<Camera>(true);
                    if (weaponCam != null)
                        weaponCam.enabled = false;
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

    public void PlayerDamage(int Damage)
    {
        filler.fillAmount = currentHealth > 0 ? (currentHealth - Damage) / (maxValue - minValue) : 0;
        currentHealth -= Damage;
        currentHealth = Mathf.Max(currentHealth, 0); 
        int SpaceIndex = HealthState.text.IndexOf(' ');
        HealthState.text = currentHealth + " hp";
        if (Time.time - lastHurtSoundTime >= hurtSoundCooldown)
        {
            HurtSource.PlayOneShot(PlayerHurtClip);
            lastHurtSoundTime = Time.time;
        }
        if (currentHealth <= 0 && filler.fillAmount <= 0)
        {
            currentHealth = 0;
            // player Dead
            PlayerDeath();
        }
    }
    private void PlayerDeath()
    {
        // change Screen
        death.isShowed = true;
        Debug.Log("Player Dead");
    }
}
