using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : MonoBehaviour
{
    public GameObject inventoryUI;
    public InventorySlot[] itemSlots;
    public PlayerWeaponHandler playerWeaponHandler;

    public void UpdateInventoryUI(List<GameObject> items)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < items.Count)
            {
                itemSlots[i].SetItem(items[i]);
            }
            else
            {
                itemSlots[i].SetItem(null);
            }
        }
    }

    public void OnItemClicked(GameObject itemPrefab)
    {
        if (itemPrefab != null)
        {
            EquipSelectedItem(itemPrefab);
        }
    }

    public void EquipSelectedItem(GameObject itemPrefab)
    {
        if (playerWeaponHandler == null)
        {
            Debug.LogError("PlayerWeaponHandler is not assigned in InventoryMenu.");
            return;
        }

        playerWeaponHandler.EquipWeapon(itemPrefab); // Pass the prefab to be instantiated
    }

    public void DeselectAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            slot.Deselect();
        }
    }
}
