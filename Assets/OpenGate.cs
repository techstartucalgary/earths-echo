using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGate : MonoBehaviour
{
    [SerializeField] private int requiredGarbageAmount; // Serialized variable for required garbage amount

    // Update is called once per frame
    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Find the player by tag
        if (player != null)
        {
            Player playerScript = player.GetComponent<Player>(); // Access the Player script
            if (playerScript != null && playerScript.collectedGarbage >= requiredGarbageAmount)
            {
                OpenTheGate(); // Call a method to open the gate
            }
        }
    }

    private void OpenTheGate()
    {
        Destroy(gameObject);
    }
}
