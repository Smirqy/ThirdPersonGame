using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            Debug.Log("Enter");
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            Debug.Log("Stay");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Pickup")
        {
            Debug.Log("Exit");
        }

    }
}
