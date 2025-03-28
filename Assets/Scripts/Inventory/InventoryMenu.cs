using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    public GameObject inventoryUI;            // The main inventory UI
    public InventorySlot[] itemSlots;         // Array of InventorySlot components
    public InventoryHandler inventoryHandler; // Reference to the InventoryHandler

    [Header("Equipped HUD UI")]
    public Image meleeHUDImage;
    public Image projectileHUDImage;
    public Image itemHUDImage;
    public TMP_Text itemHUDCount;

    /// <summary>
    /// Updates the inventory UI by assigning items and their counts to slots.
    /// </summary>
    public void UpdateInventoryUI(List<KeyValuePair<ItemSO, int>> items)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < items.Count)
            {
                itemSlots[i].SetItem(items[i].Key, items[i].Value);
            }
            else
            {
                itemSlots[i].SetItem(null, 0);
            }
        }
        
    }


    // Called by an InventorySlot when an item is clicked.
    // This method distinguishes between melee weapons, projectile weapons, and holdable items.
    public void OnItemClicked(ItemSO item)
    {
        if (item == null)
        {
            Debug.LogWarning("OnItemClicked: item is null");
            return;
        }
        
        // If the item is a WeaponSO, further check its type.
        if (item is WeaponSO weapon)
        {
            if (weapon is MeleeWeaponSO)
            {
                inventoryHandler.EquipMeleeWeapon(weapon);
                Debug.Log("Equipped melee weapon: " + weapon.itemName);
            }
            else if (weapon is ProjectileWeaponSO)
            {
                inventoryHandler.EquipProjectileWeapon(weapon);
                Debug.Log("Equipped projectile weapon: " + weapon.itemName);
            }
            else
            {
                Debug.LogWarning("OnItemClicked: Unrecognized weapon type for " + item.itemName);
            }
        }
        // Otherwise, if it's a general holdable item (assumed to be of type ItemInstSO)
        else if (item is ItemInstSO holdableItem)
        {
            inventoryHandler.EquipItem(holdableItem);
            Debug.Log("Held item: " + holdableItem.itemName);
        }
        else
        {
            Debug.LogWarning("OnItemClicked: Item type not recognized for " + item.itemName);
        }
    }

    // Deselects all slots in the inventory UI.
    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            slot.Deselect();
        }
    }

    // Toggles the visibility of the inventory UI.
    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    public void UpdateEquippedHUD()
    {
        if (inventoryHandler == null)
            return;
        
        // Update Melee HUD
        if (inventoryHandler.CurrentMeleeWeaponSO != null)
        {
            meleeHUDImage.sprite = inventoryHandler.CurrentMeleeWeaponSO.itemIcon;
            meleeHUDImage.enabled = true;
        }
        else
        {
            meleeHUDImage.enabled = false;
        }

        // Update Projectile HUD
        if (inventoryHandler.CurrentProjectileWeaponSO != null)
        {
            projectileHUDImage.sprite = inventoryHandler.CurrentProjectileWeaponSO.itemIcon;
            projectileHUDImage.enabled = true;

        }
        else
        {
            projectileHUDImage.enabled = false;
        }

        // Update General Item HUD
        if (inventoryHandler.currentItemSO != null)
        {
            itemHUDImage.sprite = inventoryHandler.currentItemSO.itemIcon;
            itemHUDImage.enabled = true;
            int count = inventoryHandler.GetItemCount(inventoryHandler.currentItemSO);
            itemHUDCount.text = count.ToString();
        }
        else
        {
            itemHUDImage.enabled = false;
            itemHUDCount.text = "";
        }
    }

}
