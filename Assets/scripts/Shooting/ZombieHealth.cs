using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Zombie Health")]
    public float Health = 100f;

    public void TakeDamage(float Damage)
    {
        Health -= Damage;

        if (Health <= 0)
        {
            Destroy(transform.gameObject);
        }
    }
}