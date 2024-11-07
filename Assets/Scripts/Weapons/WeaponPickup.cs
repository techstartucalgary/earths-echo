using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab; // Reference to the weapon prefab to be picked up

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is near the weapon. Press 'E' to pick up.");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // && Input.GetKeyDown(KeyCode.E))
        {
            PickupWeapon(other.gameObject);
        }
    }

    private void PickupWeapon(GameObject player)
    {
        Debug.Log("Weapon picked up: " + weaponPrefab.name);

        InventoryHandler inventoryHandler = player.GetComponent<InventoryHandler>();
        if (inventoryHandler != null)
        {
            inventoryHandler.AddItem(weaponPrefab); // Add weapon prefab directly
        }
        else
        {
            Debug.LogWarning("InventoryHandler not found on player.");
        }

        Destroy(gameObject); // Destroy the pickup object after picking up
    }
}
