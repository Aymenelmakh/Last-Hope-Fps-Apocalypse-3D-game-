using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSound : MonoBehaviour
{
    public GameObject Car;
    private CarController carController;
    public AudioSource engineSound;
    public float minPitch = 0.8f;
    public float maxPitch = 2f;
    public float maxVolume = 1f;

    void Start()
    {
        carController = Car.GetComponent<CarController>();
        engineSound.loop = true;
        engineSound.Stop();
    }

    void Update()
    {
        if (carController.playerInCar)
        {
            if (!engineSound.isPlaying)
                engineSound.Play();

            float ratio = carController.speedRatio();
            engineSound.volume = Mathf.Lerp(0.3f, maxVolume, ratio);
            engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, ratio);
        }
        else
        {
            if (engineSound.isPlaying)
                engineSound.Stop();
        }
    }
}
