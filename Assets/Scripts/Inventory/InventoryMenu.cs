using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    public GameObject inventoryUI; // The main inventory UI
    public InventorySlot[] itemSlots; // Array of InventorySlot components
    public PlayerWeaponHandler playerWeaponHandler; // Reference to player's weapon handler
    public InventoryHandler inventoryHandler; // Reference to the InventoryHandler

    // Updates the inventory UI by assigning items to slots
	public void UpdateInventoryUI(List<GameObject> items)
	{
		for (int i = 0; i < itemSlots.Length; i++)
		{
			if (i < items.Count && items[i] != null)
			{
				itemSlots[i].SetItem(items[i]);
			}
			else
			{
				itemSlots[i].SetItem(null); // Clear the slot
			}
		}
	}


    // Called by InventorySlot to equip the item
    public void OnItemClicked(GameObject itemPrefab)
    {
        if (itemPrefab != null)
        {
            EquipSelectedItem(itemPrefab);
        }
    }

    // Equip the item in the appropriate slot based on type
    public void EquipSelectedItem(GameObject item)
    {
        if (playerWeaponHandler == null)
        {
            Debug.LogError("PlayerWeaponHandler is not assigned in InventoryMenu.");
            return;
        }

        Weapon weapon = item.GetComponent<Weapon>();
        if (weapon == null)
        {
            Debug.LogError("No Weapon component found on the item: " + item.name);
            return;
        }

        // Check the type of weapon and call the appropriate equip method
        if (weapon is MeleeWeapon)
        {
            playerWeaponHandler.EquipMeleeWeapon(item);
        }
        else if (weapon is ProjectileWeapon)
        {
            playerWeaponHandler.EquipProjectileWeapon(item);
        }
        else
        {
            Debug.LogWarning("Unknown weapon type for item: " + item.name);
        }

        Debug.Log("Equipped: " + weapon.weaponName);
    }

    // Deselects all slots in the inventory UI
    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            slot.Deselect();
        }
    }

    // Toggles the visibility of the inventory UI
    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);

       

    }

}

