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
                if (!isSelected)
                {
                    // Show item description on first click
                    ShowItemDescription();
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

    public void Select()
    {
        if (selectedShader != null)
        {
            selectedShader.SetActive(true);
        }

        // Check if itemPrefab is still valid before proceeding
        if (itemPrefab == null)
        {
            Debug.LogWarning("Select method called, but itemPrefab is null or destroyed.");
            return;
        }

        // Attempt to get the Weapon component only if itemPrefab is valid
        Weapon weapon = itemPrefab.GetComponent<Weapon>();
        if (weapon != null)
        {
            if (weapon is MeleeWeapon && selectedIcons.Length > 0 && selectedIcons[0] != null)
            {
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
}

