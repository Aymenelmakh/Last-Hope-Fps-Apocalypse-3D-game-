using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NPCDropoff : MonoBehaviour
{
    public Text mytext;
    public Transform character;
    public Transform dropOff;
    private CarPassenger carPassenger;

    void Start()
    {
        carPassenger = FindFirstObjectByType<CarPassenger>();
    }

    private void OnTriggerStay(Collider other)
    {
        //mytext.text = null;

        if(other.CompareTag("dropoff"))
        {
            mytext.text = "Drop off Hostage: Press G";
            if (Input.GetKeyDown(KeyCode.G) && carPassenger.passengerCount > 0)
            {
                character.gameObject.SetActive(true);
                Vector3 pos = dropOff.position;
                pos.x += 4f;
                pos.y = 0f;
                character.position = pos;

                carPassenger.passengerCount--;
            }
            if(carPassenger.passengerCount <= 0)
            {
                mytext.text = "No passengers abord";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        mytext.text = null;
    }
}
