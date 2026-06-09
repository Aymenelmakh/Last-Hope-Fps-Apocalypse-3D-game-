using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NewChestInteract : Interactable
{

    protected override void Interact()
    {
        GetComponent<Animator>().SetTrigger("Play");
    }
}
