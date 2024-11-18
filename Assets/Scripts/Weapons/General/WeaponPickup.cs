using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab; // Reference to the weapon prefab to be picked up
    private bool isPlayerNearby = false; // Flag to track if the player is within range
    private GameObject nearbyPlayer; // Reference to the player object

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            nearbyPlayer = other.gameObject;
            Debug.Log("Player is near the weapon. Press 'E' to pick up.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            nearbyPlayer = null;
        }
    }

    private void Update()
    {
        // Check if the player is nearby and presses the E key
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PickupWeapon(nearbyPlayer);
        }
    }

    private void PickupWeapon(GameObject player)
    {
        if (weaponPrefab == null)
        {
            Debug.LogError("WeaponPrefab is null. Cannot pick up weapon.");
            return;
        }

        Debug.Log("Weapon picked up: " + weaponPrefab.name);

        InventoryHandler inventoryHandler = player.GetComponent<InventoryHandler>();
        if (inventoryHandler != null)
        {
            inventoryHandler.AddItem(weaponPrefab);
        }
        else
        {
            Debug.LogWarning("InventoryHandler not found on player.");
        }

        Destroy(gameObject); // Destroy the pickup object after picking up
    }

}
