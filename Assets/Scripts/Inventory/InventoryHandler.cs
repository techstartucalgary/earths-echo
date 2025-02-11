using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    public int inventoryLimit = 20;
    private List<GameObject> items = new List<GameObject>();
    [SerializeField] InventoryMenu inventoryMenu;

    void Start()
    {
        // inventoryMenu = FindObjectOfType<InventoryMenu>();
    }

    public void AddItem(GameObject item)
	{
		if (item == null)
		{
			Debug.LogError("Attempted to add a null item to the inventory.");
			return;
		}

		if (inventoryMenu == null)
		{
			Debug.LogError("InventoryMenu is not assigned in InventoryHandler.");
			return;
		}

		if (items.Count >= inventoryLimit)
		{
			Debug.LogWarning("Inventory is full. Cannot add more items.");
			return;
		}

		if (items.Contains(item))
		{
			Debug.LogWarning("Item is already in inventory.");
			return;
		}

		items.Add(item);
		Debug.Log($"Added item: {item.name}");

		inventoryMenu.UpdateInventoryUI(items);
	}



    public List<GameObject> GetItems()
    {
        return items;
    }

}

