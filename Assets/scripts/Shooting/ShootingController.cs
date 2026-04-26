using UnityEngine;

public class ShootingController : MonoBehaviour
{
    // Use RayCast
    public Transform firepoint; // where the fire start
    public float fireRate = 10f; // the time between fires
    private float nextShootTime = 0f;
    public float DamageAmount = 10f; // damage per Shot
    public float maxDistance = 100f; // the distance of shooting
    public LayerMask Enemy; // hit only zombie
    public bool isAuto = false;
    void Update()
    {
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
    }

    void FireHandle()
    {
        RaycastHit hit;

        if (Physics.Raycast(firepoint.transform.position, firepoint.transform.forward, out hit, maxDistance, Enemy)) // "out" means:"Unity, take this empty variable and fill it with whatever you hit"
        {
            GameObject objectHit = hit.transform.gameObject;
            ZombieHealth enemyHealth = hit.transform.GetComponent<ZombieHealth>();
            Debug.Log("Hits : "+ hit.transform.name);

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(DamageAmount);
            }
        }
    }
}
