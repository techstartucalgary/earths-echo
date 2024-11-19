using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public GameObject itemPrefab; // Store the weapon prefab instead of an instance
    private InventoryMenu inventoryMenu;
    private Image itemIcon;
    private bool isSelected; // Track if the item has been selected once
    public string itemName;
    public string itemDescription;

    [SerializeField] private GameObject selectedShader;
    [SerializeField] private Image[] selectedIcons; // Array to hold selected icons (0 = melee, 1 = projectile)

    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    [Header("Ammo Info UI")]
    public Image ammo1Image; // Image for ammo 1
    public TMP_Text ammo1NameText; // Name for ammo 1
    public Image ammo2Image; // Image for ammo 2
    public TMP_Text ammo2NameText; // Name for ammo 2

    void Start()
    {
        inventoryMenu = FindObjectOfType<InventoryMenu>();
        itemIcon = transform.Find("ItemImage").GetComponent<Image>();

        if (itemIcon != null)
        {
            itemIcon.enabled = false;
        }
    }

    public void SetItem(GameObject newItemPrefab)
    {
        if (newItemPrefab == null)
        {
            Debug.LogError("Attempted to set a null itemPrefab in InventorySlot.");
            ClearItem(); // Clear the slot UI if the item is null
            return;
        }

        itemPrefab = newItemPrefab;
        Weapon weapon = itemPrefab.GetComponent<Weapon>();

        if (weapon != null)
        {
            itemName = weapon.weaponName;
            itemDescription = weapon.description;
        }
        else
        {
            Debug.LogWarning("No Weapon component found on the assigned itemPrefab.");
            itemName = "Unknown Item";
            itemDescription = "No description available.";
        }

        if (itemIcon != null)
        {
            Sprite itemSprite = itemPrefab.GetComponent<SpriteRenderer>()?.sprite;
            if (itemSprite != null)
            {
                itemIcon.sprite = itemSprite;
                itemIcon.enabled = true;
            }
            else
            {
                Debug.LogWarning("ItemPrefab does not have a SpriteRenderer or a valid sprite.");
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }
    }

    private void ClearItem()
    {
        itemPrefab = null;
        itemName = string.Empty;
        itemDescription = string.Empty;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (itemPrefab != null)
            {
				Weapon weapon = itemPrefab.GetComponent<Weapon>();
				if(weapon is MeleeWeapon)
					DeselectAmmoStats();
                if (!isSelected)
                {
                    // Show item description and ammo info on the first click
                    ShowItemDescription();
                    ShowAmmoInfo();
                    isSelected = true;
                }
                else
                {
                    // Equip item on subsequent click
                    inventoryMenu.DeselectAllSlots();
                    Select();
                    inventoryMenu.OnItemClicked(itemPrefab);
                    isSelected = false; // Reset selection state
                }
            }
            else
            {
                Debug.LogWarning("Attempted to select a destroyed or null item.");
            }
        }
    }

    private void ShowItemDescription()
    {
        // Display item details in the UI
        if (itemDescriptionImage != null)
        {
            itemDescriptionImage.sprite = itemIcon.sprite;
            itemDescriptionImage.enabled = true;
        }
        if (itemDescriptionNameText != null)
        {
            itemDescriptionNameText.text = itemName;
        }
        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = itemDescription;
        }
    }

    private void ShowAmmoInfo()
    {
        // Check if the itemPrefab is a projectile weapon
        ProjectileWeapon projectileWeapon = itemPrefab.GetComponent<ProjectileWeapon>();
        if (projectileWeapon == null)
        {
            Debug.LogWarning("Item is not a projectile weapon, cannot display ammo info.");
            return;
        }

        // Update Ammo1 Info
        if (projectileWeapon.projectilePrefab != null)
        {
            ProjectileBehaviour ammo1 = projectileWeapon.projectilePrefab.GetComponent<ProjectileBehaviour>();
            if (ammo1Image != null && ammo1 != null)
            {
                Sprite ammo1Sprite = projectileWeapon.projectilePrefab.GetComponent<SpriteRenderer>()?.sprite;
                ammo1Image.sprite = ammo1Sprite;
                ammo1Image.enabled = true;
                ammo1NameText.text = ammo1.name;
            }
        }

        // Update Ammo2 Info
        if (projectileWeapon.secondaryProjectilePrefab != null)
        {
            ProjectileBehaviour ammo2 = projectileWeapon.secondaryProjectilePrefab.GetComponent<ProjectileBehaviour>();
            if (ammo2Image != null && ammo2 != null)
            {
                Sprite ammo2Sprite = projectileWeapon.secondaryProjectilePrefab.GetComponent<SpriteRenderer>()?.sprite;
                ammo2Image.sprite = ammo2Sprite;
                ammo2Image.enabled = true;
                ammo2NameText.text = ammo2.name;
            }
        }
    }

    public void Select()
    {
        if (selectedShader != null)
        {
            selectedShader.SetActive(true);
        }

        if (itemPrefab == null)
        {
            Debug.LogWarning("Select method called, but itemPrefab is null or destroyed.");
            return;
        }

        Weapon weapon = itemPrefab.GetComponent<Weapon>();
        if (weapon != null)
        {
            if (weapon is MeleeWeapon && selectedIcons.Length > 0 && selectedIcons[0] != null)
            {
				DeselectAmmoStats();
                selectedIcons[0].sprite = itemIcon.sprite;
                selectedIcons[0].enabled = true;
            }
            else if (weapon is ProjectileWeapon && selectedIcons.Length > 1 && selectedIcons[1] != null)
            {
                selectedIcons[1].sprite = itemIcon.sprite;
                selectedIcons[1].enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("No Weapon component found on the selected itemPrefab.");
        }
    }

    public void Deselect()
    {
        if (selectedShader != null)
        {
            selectedShader.SetActive(false);
        }
        isSelected = false; // Reset selection state when deselected
    }
	public void DeselectAmmoStats()
	{
		// Clear Ammo1 Info
		if (ammo1Image != null)
		{
			ammo1Image.sprite = null;
			ammo1Image.enabled = false;
		}
		if (ammo1NameText != null)
		{
			ammo1NameText.text = string.Empty;
		}

		// Clear Ammo2 Info
		if (ammo2Image != null)
		{
			ammo2Image.sprite = null;
			ammo2Image.enabled = false;
		}
		if (ammo2NameText != null)
		{
			ammo2NameText.text = string.Empty;
		}
	}

}

