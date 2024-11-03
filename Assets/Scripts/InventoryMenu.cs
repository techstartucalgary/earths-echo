using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenu : MonoBehaviour
{
    public GameObject inventoryMenu;
    public List<GameObject> items = new List<GameObject>();
    private List<Image> itemImages = new List<Image>(); // List to hold the item images

	void Start()
	{
		Cursor.visible = false;

		// Search all Image components in the children, including nested ones
		foreach (Transform child in transform.GetComponentsInChildren<Transform>())
		{
			Image image = child.GetComponent<Image>();
			if (image != null && child.name.Contains("Image")) // Ensure it’s an item image
			{
				itemImages.Add(image);
				image.gameObject.SetActive(false); // Hide initially
			}
		}
	}


    void Update()
    {
        if (inventoryMenu.activeSelf)
        {
            Time.timeScale = 0;
            inventoryMenu.SetActive(true);
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            inventoryMenu.SetActive(false);
            Cursor.visible = false;
        }
    }

    // Method to add an item and its image
    public void AddItem(GameObject item, Sprite itemImage)
    {
        items.Add(item);
        UpdateItemImages(itemImage);
    }

    // Method to update the item images
    private void UpdateItemImages(Sprite itemImage)
    {
        for (int i = 0; i < itemImages.Count; i++)
        {
            if (itemImages[i].sprite == null) // Find the first empty slot
            {
                itemImages[i].sprite = itemImage; // Set the image
                itemImages[i].gameObject.SetActive(true); // Make sure the image is visible
                break;
            }
        }
    }
}
