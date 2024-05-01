using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointOnTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Drone"))
        {
            gameObject.SetActive(false);
        }
    }
}
