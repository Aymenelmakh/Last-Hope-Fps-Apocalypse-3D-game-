using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BloodEffect : MonoBehaviour
{
    public Image bloodImage;
    public float fadeSpeed = 0.5f;
    public float displayTime = 0.3f;

    private float fadeDelay = 0f; // counts down before fading starts

    void Start()
    {
        Color c = bloodImage.color;
        c.a = 0f;
        bloodImage.color = c;
    }

    void Update()
    {
        if (bloodImage.color.a <= 0f) return;

        if (fadeDelay > 0f)
        {
            fadeDelay -= Time.deltaTime; // still in display window, don't fade
        }
        else
        {
            Color c = bloodImage.color;
            c.a -= Time.deltaTime * fadeSpeed;
            c.a = Mathf.Max(c.a, 0f);
            bloodImage.color = c;
        }
    }

    public void ShowBlood()
    {
        Color c = bloodImage.color;
        c.a = 1f;
        bloodImage.color = c;
        fadeDelay = displayTime; // reset the hold timer on every hit
    }
}