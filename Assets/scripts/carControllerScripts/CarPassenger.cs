using UnityEngine;
using UnityEngine.UI;

public class CarPassenger : MonoBehaviour
{
    public int passengerCount = 0;
    public int maxPassengers = 4;
    private Interactable interactable;
    public Text mytext;

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponentInParent<Interactable>() != null)
        {
            interactable = other.GetComponentInParent<Interactable>();
            Debug.Log(interactable.promptMessage);
            mytext.text = interactable.promptMessage;
        }

        if (other.CompareTag("NPC") && passengerCount < maxPassengers && Input.GetKeyDown(KeyCode.F))
        {
            passengerCount++;
            other.gameObject.SetActive(false); // disappear the NPC
            Debug.Log("Passengers: " + passengerCount);
            Invoke("displayPickUpText", 2f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Interactable>() != null)
        {
            interactable = null;
            mytext.text = "";
        }
    }
    private void displayPickUpText()
    {
        mytext.text = "";
    }
}

