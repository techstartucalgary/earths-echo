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

<<<<<<< HEAD
        PlayerWeaponHandler weaponHandler = player.GetComponent<PlayerWeaponHandler>();
        if (weaponHandler != null)
        {
            // Equip the new weapon directly, which handles unequipping and managing the inventory
            weaponHandler.EquipWeapon(weapon);
        }

        // Destroy the weapon object after being picked up
        Destroy(gameObject);
    }
=======
        // Equip the weapon to the player
        PlayerWeaponHandler weaponHandler = player.GetComponent<PlayerWeaponHandler>();
        if (weaponHandler != null)
        {
            weaponHandler.EquipWeapon(weapon);
        }

        Destroy(gameObject); // Destroy the weapon object after being picked up
    }

>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
}
