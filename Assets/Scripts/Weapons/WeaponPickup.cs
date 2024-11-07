using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Weapon weapon; // Reference to the weapon script
    private bool hasBeenPickedUp = false; // Flag to prevent duplicate pickups

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            Debug.Log("Player is near the weapon. Press 'E' to pick up.");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            hasBeenPickedUp = true;
            PickupWeapon(other.gameObject);
        }
    }

    private void PickupWeapon(GameObject player)
    {
        Debug.Log("Weapon picked up: " + weapon.weaponName);

        InventoryHandler inventoryHandler = player.GetComponent<InventoryHandler>();
        if (inventoryHandler != null)
        {
            inventoryHandler.AddItem(weapon.weaponPrefab);
        }
        else
        {
            Debug.LogWarning("InventoryHandler not found on player.");
        }

        Destroy(gameObject); // Prevent further pickups
    }
}
