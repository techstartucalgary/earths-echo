using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    public int inventoryLimit = 20;
    private List<GameObject> items = new List<GameObject>();
    public InventoryMenu inventoryMenu;

    void Start()
    {
        inventoryMenu = FindObjectOfType<InventoryMenu>();
    }

    public void AddItem(GameObject item)
    {
        if (inventoryMenu == null)
        {
            Debug.LogError("InventoryMenu is not assigned in InventoryHandler.");
            return;
        }

        if (items.Count >= inventoryLimit)
        {
            Debug.Log("Inventory is full. Cannot add more items.");
            return;
        }

        if (items.Contains(item))
        {
            Debug.LogWarning("Item is already in inventory.");
            return;
        }

        items.Add(item);
        inventoryMenu.UpdateInventoryUI(items);
        Debug.Log("Item added to inventory: " + item.name);
    }

    public List<GameObject> GetItems()
    {
        return items;
    }

}

