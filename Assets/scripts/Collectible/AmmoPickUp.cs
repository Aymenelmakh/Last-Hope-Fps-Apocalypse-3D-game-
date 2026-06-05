using UnityEngine;
using UnityEngine.UI;

public class AmmoPickUp : MonoBehaviour
{
    public Slider slider;
    public int ammoToAdd = 120;
    public int maxReserveAmmo = 240;
    public Text Mag_nb;
    public AudioSource ammoAudioSource;
    public AudioClip pickUpClip;

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= 2f && ShootingController.reserveAmmo  < 240)
        {
            ShootingController.reserveAmmo = Mathf.Min(
                ShootingController.reserveAmmo + ammoToAdd,
                maxReserveAmmo
            );
            // Mag_nb.text = Mathf.Ceil((float)ShootingController.reserveAmmo / 30f) + "";
            Mag_nb.text = Mathf.CeilToInt((float)ShootingController.reserveAmmo / 30f).ToString();
            slider.value = (float)ShootingController.reserveAmmo / maxReserveAmmo;
            AudioSource.PlayClipAtPoint(pickUpClip, transform.position);
            Destroy(gameObject);
        }
    }
}