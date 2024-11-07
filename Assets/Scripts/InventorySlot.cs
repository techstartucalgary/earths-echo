using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public GameObject item; // Reference to the actual item GameObject in this slot
    private InventoryMenu inventoryMenu; // Reference to the main inventory menu
    private Image itemIcon; // Reference to the child Image for displaying the item icon
    
    [SerializeField]
    public GameObject selectedShader;
    public bool thisItemSelected;

    void Start()
    {
        inventoryMenu = FindObjectOfType<InventoryMenu>(); // Or assign in Inspector

        // Find the child Image component for displaying the item icon
        itemIcon = transform.Find("ItemImage").GetComponent<Image>();

        // Ensure the icon image is initially hidden
        if (itemIcon != null)
        {
            itemIcon.enabled = false;
        }
    }

    // Set the item in this slot and update the icon
    public void SetItem(GameObject newItem)
    {
        item = newItem;

        if (itemIcon != null)
        {
            // Set the sprite for the icon if the item has a SpriteRenderer
            if (item != null)
            {
                Sprite itemSprite = item.GetComponent<SpriteRenderer>()?.sprite;
                itemIcon.sprite = itemSprite;
                itemIcon.enabled = true;
            }
            else
            {
                // Clear the icon if there’s no item
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }
        else
        {
            Debug.LogWarning("Item icon Image component is missing.");
        }
    }

    // Handle click events
    public void OnPointerClick(PointerEventData eventData)
    {
		if(eventData.button==PointerEventData.InputButton.Left){
			Debug.Log("Slot clicked: " + (item != null ? item.name : "null"));
			OnLeftClick();
		}
		
    }
    public void OnLeftClick()
    {
		selectedShader.SetActive(true);
		thisItemSelected = true;
	}
}
