using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Represents a single slot in the inventory UI.
/// Responsible for displaying an item's icon, count (if stackable), and handling selection/interaction.
/// </summary>
public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    // The item stored in this slot.
    public ItemSO itemSO;
    
    // The count of this item in the inventory.
    public int itemCount;
    
    // The name and description of the item.
    public string itemName;
    public string itemDescription;
    
    // Reference to the InventoryMenu that manages all slots.
    private InventoryMenu inventoryMenu;
    
    // UI references.
    private Image itemIcon;
    public TMP_Text slotIconName;
    [SerializeField] private TMP_Text countText; // Displays the stack count.
    
    // Variables to track selection state.
    private bool isSelected;
    [SerializeField] private GameObject selectedShader;
    // Selected icons: index 0 = melee, 1 = projectile, 2 = general item.
    [SerializeField] private Image[] selectedIcons;

    // UI elements for displaying item descriptions.
    public Image itemDescriptionImage;
    public TMP_Text itemDescriptionNameText;
    public TMP_Text itemDescriptionText;

    // UI elements for ammo info (used by projectile weapons).
    [Header("Ammo Info UI")]
    public Image ammo1Image;
    public TMP_Text ammo1NameText;
    public Image ammo2Image;
    public TMP_Text ammo2NameText;

private void Start()
{
    // Cache the InventoryMenu reference (unchanged)
    inventoryMenu = FindObjectOfType<InventoryMenu>();

    /* ── Find the child called "ItemImage" safely ───────────────────────── */
    Transform imgT = transform.Find("ItemImage");

    if (imgT != null)
    {
        itemIcon = imgT.GetComponent<Image>();
        if (itemIcon == null)   // child exists but has no Image component
        {
            Debug.LogError($"[InventorySlot] \"ItemImage\" on {name} has no <Image> component.");
            itemIcon = imgT.gameObject.AddComponent<Image>();
            itemIcon.enabled = false;
        }
    }
    else
    {
        Debug.LogError($"[InventorySlot] Child \"ItemImage\" not found on {name}. " +
                       "Creating a placeholder so the slot can still run.");

        // Create an invisible placeholder so later code won’t NRE
        GameObject placeholder = new GameObject("ItemImage");
        placeholder.transform.SetParent(transform, false);
        itemIcon = placeholder.AddComponent<Image>();
        itemIcon.enabled = false;
    }

    /* ── Hide icon initially (same as before) ───────────────────────────── */
    itemIcon.enabled = false;
}


    /// <summary>
    /// Sets the slot with the given item and count.
    /// If the slot already contains the same item (identified by itemName), updates only the count,
    /// preserving the selection state and selected icons.
    /// </summary>
    /// <param name="newItem">The item to display.</param>
    /// <param name="newCount">The count of that item.</param>
    public void SetItem(ItemSO newItem, int newCount)
    {
        // Check if the slot already holds an item and it is of the same type.
        // We assume that each item type has a unique itemName.
        if (itemSO != null && newItem != null && itemSO.itemName == newItem.itemName)
        {
            // Update only the count.
            itemCount = newCount;
            
            // Update the count display if the item is stackable and count > 1.
            if (itemSO.stackable && itemCount > 1 && countText != null)
            {
                countText.text = itemCount.ToString();
                countText.enabled = true;
            }
            else if (countText != null)
            {
                countText.text = "";
                countText.enabled = false;
            }
            
            // Do not change selection state or reassign itemSO.
            return;
        }
        
        // If the new item is different (or newItem is null), perform a full update.
        if (newItem == null)
        {
            ClearItem();
            return;
        }
        
        // For a completely new item, reset the selection state.
        isSelected = false;
        
        // Assign the new item and count.
        itemSO = newItem;
        itemCount = newCount;
        itemName = itemSO.itemName;
        itemDescription = itemSO.description;
        slotIconName.text = itemName;
        
        // Update the item icon.
        if (itemIcon != null)
        {
            if (itemSO.itemIcon != null)
            {
                itemIcon.sprite = itemSO.itemIcon;
                itemIcon.enabled = true;
            }
            else
            {
                Debug.LogWarning("ItemSO does not have a valid sprite.");
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }
        
        // Update the count display.
        if (itemSO.stackable && itemCount > 1 && countText != null)
        {
            countText.text = itemCount.ToString();
            countText.enabled = true;
        }
        else if (countText != null)
        {
            countText.text = "";
            countText.enabled = false;
        }
    }


    /// <summary>
    /// Clears the slot completely. Resets item data, UI elements, and selection state.
    /// </summary>
    public void ClearItem()
    {
        itemSO = null;
        itemName = string.Empty;
        slotIconName.text = "";
        itemDescription = string.Empty;
        itemDescriptionNameText.text = "";
        itemDescriptionText.text = "";
        // Clear the description image (set to null so it can be reset by external code if needed).
        itemDescriptionImage.sprite = null;

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }
        if (countText != null)
        {
            countText.text = "";
            countText.enabled = false;
        }

        // Clear selection visuals.
        Deselect();
    }

    /// <summary>
    /// Clears only the selection visuals (shader and selected icons) without removing item data.
    /// </summary>
public void ClearSelectedIcon()
{
    // Only clear the selected icon corresponding to the item type.
    if (itemSO != null)
    {
        WeaponSO weapon = itemSO as WeaponSO;
        if (weapon != null)
        {
            // If it's a melee weapon, clear only the first icon.
            if (weapon is MeleeWeaponSO && selectedIcons.Length > 0 && selectedIcons[0] != null)
            {
                selectedIcons[0].enabled = false;
            }
            // If it's a projectile weapon, clear only the second icon.
            else if (weapon is ProjectileWeaponSO && selectedIcons.Length > 1 && selectedIcons[1] != null)
            {
                selectedIcons[1].enabled = false;
            }
        }
        else
        {
            // For non-weapon items, clear only the third icon.
            if (selectedIcons.Length > 2 && selectedIcons[2] != null)
            {
                selectedIcons[2].enabled = false;
            }
        }
    }
    isSelected = false;
}


    /// <summary>
    /// Handles pointer click events on the slot.
    /// On first click, shows item details. On second click, sends the item to the InventoryMenu for equipping/holding.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Only respond to left-clicks.
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (itemSO != null)
            {
                // If the item is a weapon.
                WeaponSO weapon = itemSO as WeaponSO;
                if (weapon != null)
                {
                    // For melee weapons, clear any projectile ammo stats.
                    if (weapon is MeleeWeaponSO)
                        DeselectAmmoStats();

                    if (!isSelected)
                    {
                        ShowItemDescription();
                        ShowAmmoInfo();
                        isSelected = true;
                    }
                    else
                    {
                        inventoryMenu.DeselectAllSlots();
                        Select();
                        // Call the InventoryMenu method to process equipping/holding.
                        inventoryMenu.OnItemClicked(itemSO);
                        isSelected = false;
                    }
                }
                else
                {
                    // For non-weapon items.
                    if (!isSelected)
                    {
                        ShowItemDescription();
                        isSelected = true;
                    }
                    else
                    {
                        inventoryMenu.DeselectAllSlots();
                        Select();
                        inventoryMenu.OnItemClicked(itemSO);
                        isSelected = false;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Attempted to select a null item.");
            }
        }
    }

    /// <summary>
    /// Displays the item's description in the UI.
    /// </summary>
    private void ShowItemDescription()
    {
        if (itemDescriptionImage != null)
        {
            itemDescriptionImage.sprite = itemIcon.sprite;
            itemDescriptionImage.enabled = true;
        }
        if (itemDescriptionNameText != null)
            itemDescriptionNameText.text = itemName;
        if (itemDescriptionText != null)
            itemDescriptionText.text = itemDescription;
    }

    /// <summary>
    /// Shows ammo info if the item is a projectile weapon.
    /// </summary>
    private void ShowAmmoInfo()
    {
        ProjectileWeaponSO projectileWeapon = itemSO as ProjectileWeaponSO;
        if (projectileWeapon == null)
        {
            Debug.LogWarning("Item is not a projectile weapon; no ammo info to display.");
            return;
        }

        // Update Ammo1 info.
        if (projectileWeapon.projectilePrefab != null)
        {
            Sprite ammo1Sprite = projectileWeapon.projectilePrefab.GetComponent<SpriteRenderer>()?.sprite;
            if (ammo1Image != null && ammo1Sprite != null)
            {
                ammo1Image.sprite = ammo1Sprite;
                ammo1Image.enabled = true;
                ammo1NameText.text = projectileWeapon.projectilePrefab.name;
            }
        }

        // Update Ammo2 info, if available.
        if (projectileWeapon.secondaryProjectilePrefab != null)
        {
            Sprite ammo2Sprite = projectileWeapon.secondaryProjectilePrefab.GetComponent<SpriteRenderer>()?.sprite;
            if (ammo2Image != null && ammo2Sprite != null)
            {
                ammo2Image.sprite = ammo2Sprite;
                ammo2Image.enabled = true;
                ammo2NameText.text = projectileWeapon.secondaryProjectilePrefab.name;
            }
        }
        else
        {
            DeselectAmmoStats();
        }
    }

    /// <summary>
    /// Activates the selected visual elements.
    /// </summary>
    public void Select()
    {
        if (selectedShader != null)
            selectedShader.SetActive(true);

        // Optionally update specific selected icons based on the type.
        WeaponSO weapon = itemSO as WeaponSO;
        if (weapon != null)
        {
            if (weapon is MeleeWeaponSO && selectedIcons.Length > 0 && selectedIcons[0] != null)
            {
                DeselectAmmoStats();
                selectedIcons[0].sprite = itemIcon.sprite;
                selectedIcons[0].enabled = true;
            }
            else if (weapon is ProjectileWeaponSO && selectedIcons.Length > 1 && selectedIcons[1] != null)
            {
                selectedIcons[1].sprite = itemIcon.sprite;
                selectedIcons[1].enabled = true;
            }
        }
        else
        {
            if (selectedIcons.Length > 2 && selectedIcons[2] != null)
            {
                selectedIcons[2].sprite = itemIcon.sprite;
                selectedIcons[2].enabled = true;
            }
        }
    }

    /// <summary>
    /// Clears the selection.
    /// </summary>
    public void Deselect()
    {
        if (selectedShader != null)
            selectedShader.SetActive(false);
        isSelected = false;
    }

    /// <summary>
    /// Clears ammo-related UI elements.
    /// </summary>
    public void DeselectAmmoStats()
    {
        if (ammo1Image != null)
        {
            ammo1Image.sprite = null;
            ammo1Image.enabled = false;
        }
        if (ammo1NameText != null)
            ammo1NameText.text = string.Empty;
        if (ammo2Image != null)
        {
            ammo2Image.sprite = null;
            ammo2Image.enabled = false;
        }
        if (ammo2NameText != null)
            ammo2NameText.text = string.Empty;
    }
}
