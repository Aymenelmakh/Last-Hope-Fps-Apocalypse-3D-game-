using UnityEngine;
using UnityEngine.UI;

public class ArmsCurrentMags : MonoBehaviour
{
    public Text currentMag;
    public Slider slider;

    void Start()
    {
        currentMag.text = ShootingController.reserveAmmo / 30 + "";
        slider.value = 1f;
    }

    void Update()
    {
        if (ShootingController.reserveAmmo >= 100)
        {
            RectTransform rect = currentMag.GetComponent<RectTransform>();
            Vector2 pos = rect.anchoredPosition;
            pos.x = 136; // your desired value
            rect.anchoredPosition = pos;
        }
    }
}
