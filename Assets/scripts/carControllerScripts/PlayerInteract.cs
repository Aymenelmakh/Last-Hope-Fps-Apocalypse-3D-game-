using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f;
    public LayerMask mask;
    private PlayerUI PlayerUI;
    public InputManager inputManager;
    protected Interactable interactable;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        PlayerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerUI.UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitinfo;
        if (Physics.Raycast(ray, out hitinfo, distance, mask))
        {
            if (hitinfo.collider.GetComponentInParent<Interactable>() != null)
            {
                interactable = hitinfo.collider.GetComponentInParent<Interactable>();
                PlayerUI.UpdateText(interactable.promptMessage);
                if (inputManager.onFoot.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
