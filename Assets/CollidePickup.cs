using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidePickup : MonoBehaviour
{
    // This method is called when the collider enters a trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has the tag "Player"
        if (other.CompareTag("Player"))
        {
            // Destroy this game object
            Destroy(gameObject);
        }
    }
}
