using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Weapon weapon; // Reference to the weapon script

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is near the weapon. Press 'E' to pick up.");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            PickupWeapon(other.gameObject);
        }
    }

    private void PickupWeapon(GameObject player)
    {
        Debug.Log("Weapon picked up: " + weapon.weaponName);

        PlayerWeaponHandler weaponHandler = player.GetComponent<PlayerWeaponHandler>();
        if (weaponHandler != null)
        {
            // Equip the new weapon directly, which handles unequipping and managing the inventory
            weaponHandler.EquipWeapon(weapon);
        }

        // Destroy the weapon object after being picked up
        Destroy(gameObject);
    }
}
