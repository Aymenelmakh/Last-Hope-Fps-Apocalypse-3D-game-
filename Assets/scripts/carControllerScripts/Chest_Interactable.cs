using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;

public class Chest_Interactable : Interactable
{
    public GameObject lid;
    private bool ChestOpen;

    protected override void Interact()
    {
        ChestOpen = !ChestOpen;
        lid.GetComponent<Animator>().SetBool("isOpen", ChestOpen);
    }
}
