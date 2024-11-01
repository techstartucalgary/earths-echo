using UnityEngine;
<<<<<<< HEAD
using System.Collections.Generic;
=======
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15

public class PlayerWeaponHandler : MonoBehaviour
{
    private Weapon equippedWeapon;
<<<<<<< HEAD
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
=======

    public void EquipWeapon(Weapon newWeapon)
    {
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
        if (equippedWeapon != null)
        {
            UnequipWeapon();
        }

<<<<<<< HEAD
        // Equip the new weapon
        equippedWeapon = newWeapon;
        equippedWeapon.Equip(transform); // Pass the player's transform to the Equip method
        Debug.Log("Player equipped: " + equippedWeapon.weaponName);
        UpdateWeaponVisibility();
=======
        equippedWeapon = newWeapon;
        equippedWeapon.Equip(transform); // Pass the player's transform to the Equip method
        Debug.Log("Player equipped: " + equippedWeapon.weaponName);
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
    }

    public void UnequipWeapon()
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.Unequip();
            equippedWeapon = null;
<<<<<<< HEAD
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
=======
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
        }
    }

    private void Update()
    {
<<<<<<< HEAD
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
=======
        // Handle weapon attack inputs
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
        if (equippedWeapon != null)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                equippedWeapon.PrimaryAttack();
            }
<<<<<<< HEAD
            else if (Input.GetKey(KeyCode.LeftControl)) // Check if LeftControl is held down
            {
                if (Input.GetKeyDown(KeyCode.D))
                    equippedWeapon.SideAttack();
                else if (Input.GetKeyDown(KeyCode.Space))
                    equippedWeapon.UpAttack();
                else if (Input.GetKeyDown(KeyCode.S))
=======
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.D))
                    equippedWeapon.SideAttack();
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.W))
                    equippedWeapon.UpAttack();
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.S))
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
                    equippedWeapon.DownAttack();
            }
        }
    }
}
<<<<<<< HEAD



=======
>>>>>>> 4d13b36c3729ac9bdde2730f117255ae03d0bd15
