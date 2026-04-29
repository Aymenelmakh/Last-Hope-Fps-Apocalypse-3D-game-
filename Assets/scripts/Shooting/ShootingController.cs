using UnityEngine;

public class ShootingController : MonoBehaviour
{
    // Use RayCast
    [Header("Shooting System")]
    public Transform firepoint; // where the fire start
    public float fireRate = 10f; // the time between fires
    private float nextShootTime = 0f;
    public float DamageAmount = 10f; // damage per Shot
    public float maxDistance = 100f; // the distance of shooting
    public LayerMask Enemy; // hit only zombie
    public bool isAuto = false;

    [Header("Ammo and reload system")]
    public int magazinSize = 30; // Max bullets per clip
    private int currentAmmo; // Bullets left in current mag
    public float reloadTime = 1.5f;
    public bool isReloading = false;
    public int reserveAmmo = 90; // Bullets left in reserve (2 mag)

    [Header("Animator controller")]
    public Animator animator;

    [Header("Particle system")]
    public ParticleSystem muzzleFlash;

    [Header("Gun Audio System")]
    public AudioSource AmmoOutSource;
    public AudioClip[] AmmoOutSounds;
    public float AmmoOutInterval = 0.3f;
    public float nextAmmoOutSoundTime = 0f;
    public AudioSource FiringSource;
    public AudioClip FiringClip;
    public AudioClip ReloadSoundClip;
    public AudioClip RiflePickClip;
    void Start()
    {
        currentAmmo = magazinSize;
    }
    void Update()
    {
        animator.SetBool("Idle", true);
        animator.SetBool("Shoot", false);
        if (isReloading)
        {
            return ;
        }
        if (isAuto == true)
        {
            if (Input.GetButton("Fire 1") && Time.time >= nextShootTime)
            {
                nextShootTime = Time.time + 1f / fireRate;
                FireHandle();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire 1") && Time.time >= nextShootTime)
            {
                nextShootTime = Time.time + 1f / fireRate;
                FireHandle();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazinSize)
        {
            if (reserveAmmo > 0)
            {
                animator.SetTrigger("Reload");
                Reload();
            }
            else
            {
                // Play Mag out sound and display 0 mag in UI
                AmmoOutSound();
                Debug.Log("No more mags!");
            }
        }
    }

    void FireHandle()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            RaycastHit hit;

            if (Physics.Raycast(firepoint.transform.position, firepoint.transform.forward, out hit, maxDistance)) // "out" means:"Unity, take this empty variable and fill it with whatever you hit"
            {
                GameObject objectHit = hit.transform.gameObject;
                ZombieHealth enemyHealth = hit.transform.GetComponent<ZombieHealth>();
                Debug.Log("Hits : "+ hit.transform.name);

                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(DamageAmount);
                }
            }
            muzzleFlash.Play();
            animator.SetBool("Idle", false);
            animator.SetBool("Shoot", true);
            FiringSource.PlayOneShot(FiringClip);
        }
        else
        {
            // Auto Reload
            {
                // if (reserveAmmo > 0)
                // {
                //     Reload();
                // }
                // else
                // {
                //     Debug.Log("you don't have more mag!");
                // }
            }
            //Play empty rifle sound
            if (Time.time >= nextAmmoOutSoundTime)
            {
                nextAmmoOutSoundTime = Time.time + AmmoOutInterval;
                AmmoOutSound();
            }
        }
    }
    private void Reload()
    {
        if (!isReloading && currentAmmo < magazinSize && reserveAmmo > 0)
        {
            isReloading = true;
            //play reload sound
            Invoke("PlayReloadSound", 0.5f);
            Invoke("FinishReloading", reloadTime); // lets you call a function after a time delay — like a built-in timer.
        }
    }
    private void FinishReloading()
    {
        int bulletsNeeded = magazinSize - currentAmmo;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);
        currentAmmo += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;
        isReloading = false;
        // reset reload anim
        animator.ResetTrigger("Reload");
    }

    void AmmoOutSound()
    {
        AudioClip AmmoOutClip = AmmoOutSounds[Random.Range(0, AmmoOutSounds.Length)];
        AmmoOutSource.PlayOneShot(AmmoOutClip);
    }

    void PlayPickupSound()
    {
        FiringSource.PlayOneShot(RiflePickClip);
    }
    void PlayReloadSound()
    {
        FiringSource.PlayOneShot(ReloadSoundClip);
    }
}
