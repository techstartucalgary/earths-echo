using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    public GameObject inventoryUI; // The main inventory UI
    public InventorySlot[] itemSlots; // Array of InventorySlot components
    public PlayerWeaponHandler playerWeaponHandler; // Reference to player's weapon handler

    private float doubleClickTime = 0.3f; // Time for double-click detection
    private float lastClickTime = 0;

    // Update the inventory UI by assigning items to slots
    public void UpdateInventoryUI(List<GameObject> items)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < items.Count)
            {
                // Set the item in the slot
                itemSlots[i].SetItem(items[i]);
            }
            else
            {
                // Clear the slot if no item
                itemSlots[i].SetItem(null);
            }
        }
    }

    // Called by InventorySlot when an item is clicked
    public void OnItemClicked(GameObject item)
    {
        Debug.Log("Slot clicked, item: " + (item != null ? item.name : "null"));

        // Double-click detection
        if (Time.time - lastClickTime < doubleClickTime)
        {
            if (item != null)
            {
                EquipSelectedItem(item);
            }
            else
            {
                Debug.LogWarning("Attempted to equip a null item.");
            }
        }
        lastClickTime = Time.time;
    }

    // Equip the item in the appropriate slot based on type
    public void EquipSelectedItem(GameObject item)
    {
        Weapon weapon = item.GetComponent<Weapon>();
        if (weapon != null)
        {
            if (weapon is MeleeWeapon)
            {
                playerWeaponHandler.EquipMeleeWeapon(item);
            }
            else if (weapon is ProjectileWeapon)
            {
                playerWeaponHandler.EquipProjectileWeapon(item);
            }
            Debug.Log("Equipped: " + weapon.weaponName);
        }
        else
        {
            Debug.LogWarning("Selected item does not contain a Weapon component.");
        }
    }

    // Toggle the visibility of the inventory UI
    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
}

