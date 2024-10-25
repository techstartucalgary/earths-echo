using UnityEngine;
using System.Collections.Generic;

public class PlayerWeaponHandler : MonoBehaviour
{
    private Weapon equippedWeapon;
    private List<Weapon> inventory = new List<Weapon>();
    public int inventoryLimit = 3; // Maximum number of weapons

    public void EquipWeapon(Weapon newWeapon)
    {
		
        // Check if the weapon is already in the inventory or if there's space in the inventory
        if (!inventory.Contains(newWeapon))
        {
            if (inventory.Count >= inventoryLimit)
            {
                Debug.Log("Cannot pick up " + newWeapon.weaponName + ". Inventory is full.");
                return; // Prevent adding or equipping the new weapon if inventory is full
            }

            // Add the new weapon to the inventory if there's space
            inventory.Add(newWeapon);
        }

        // Unequip the currently equipped weapon
        if (equippedWeapon != null)
        {
            UnequipWeapon();
        }

        // Equip the new weapon
        equippedWeapon = newWeapon;
        equippedWeapon.Equip(transform); // Pass the player's transform to the Equip method
        Debug.Log("Player equipped: " + equippedWeapon.weaponName);
        UpdateWeaponVisibility();
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.Unequip();
            equippedWeapon = null;
            UpdateWeaponVisibility();
        }
    }

    private void UpdateWeaponVisibility()
    {
        // Hide or show weapons based on equipped weapon
        foreach (var weapon in inventory)
        {
            if (weapon != null)
            {
                weapon.gameObject.SetActive(weapon == equippedWeapon); // Only show the equipped weapon
            }
        }
    }

    private void DropCurrentWeapon()
    {
        if (equippedWeapon != null)
        {
            UnequipWeapon();
            Debug.Log("Dropped weapon: " + equippedWeapon.weaponName);
        }
    }

    private void Update()
    {
        HandleWeaponSwitch();
        HandleWeaponAttack();
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && inventory.Count > 0)
        {
            EquipWeapon(inventory[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && inventory.Count > 1)
        {
            EquipWeapon(inventory[1]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && inventory.Count > 2)
        {
            EquipWeapon(inventory[2]);
        }
    }

    private void HandleWeaponAttack()
    {
        if (equippedWeapon != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                equippedWeapon.PrimaryAttack();
            }
            else if (Input.GetKey(KeyCode.LeftControl)) // Check if LeftControl is held down
            {
                if (Input.GetKeyDown(KeyCode.D))
                    equippedWeapon.SideAttack();
                else if (Input.GetKeyDown(KeyCode.Space))
                    equippedWeapon.UpAttack();
                else if (Input.GetKeyDown(KeyCode.S))
                    equippedWeapon.DownAttack();
            }
        }
    }
}



