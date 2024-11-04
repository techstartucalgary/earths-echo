using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    public GameObject inventoryUI;
    public Image[] itemSlots;
    public PlayerWeaponHandler playerWeaponHandler;

    private float doubleClickTime = 0.3f; // Adjust based on preferred double-click speed
    private float lastClickTime = 0;

    // Update the inventory UI and assign onClick listeners to item slots
	public void UpdateInventoryUI(List<GameObject> items)
{
    for (int i = 0; i < itemSlots.Length; i++)
    {
        if (i < items.Count)
        {
            Sprite itemSprite = items[i]?.GetComponent<SpriteRenderer>()?.sprite;

            if (itemSlots[i] == null)
            {
                Debug.LogError("itemSlots[" + i + "] is null.");
                continue;
            }

            itemSlots[i].sprite = itemSprite;
            itemSlots[i].gameObject.SetActive(true);

            // Find the Button component on the child
            var button = itemSlots[i].transform.GetChild(0).GetComponent<Button>();
            if (button == null)
            {
                Debug.LogError("Button component is missing on child of itemSlots[" + i + "].");
            }
            else
            {
                button.onClick.RemoveAllListeners(); // Ensure previous listeners are cleared
                int index = i; // Cache the current index for closure
                button.onClick.AddListener(() => OnItemClicked(items[index]));
            }
        }
        else
        {
            itemSlots[i].sprite = null;
            itemSlots[i].gameObject.SetActive(false);
        }
    }
}



    // Handle click and double-click events on items
	public void OnItemClicked(GameObject item)
	{
		Debug.Log("Button clicked, item: " + (item != null ? item.name : "null"));
		
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


    // Equip the item in the appropriate slot based on its type
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
    }

    // Optional: Highlight the selected item or show more details (single click behavior)
    public void HighlightItem(GameObject item)
    {
        // Logic to highlight the item or show more details (e.g., item stats)
        Debug.Log("Item selected: " + item.name);
    }

    public void ToggleInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
}
