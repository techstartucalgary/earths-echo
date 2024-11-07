using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public GameObject itemPrefab; // Store the weapon prefab instead of an instance
    private InventoryMenu inventoryMenu;
    private Image itemIcon;

    [SerializeField] private GameObject selectedShader;
    [SerializeField] private Image[] selectedIcons; // Array to hold selected icons (0 = melee, 1 = projectile)

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
        itemPrefab = newItemPrefab;

        if (itemIcon != null)
        {
            if (itemPrefab != null)
            {
                Sprite itemSprite = itemPrefab.GetComponent<SpriteRenderer>()?.sprite;
                itemIcon.sprite = itemSprite;
                itemIcon.enabled = true;
            }
            else
            {
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (itemPrefab != null) // Check if itemPrefab is still valid
            {
                inventoryMenu.DeselectAllSlots();
                Select();
                inventoryMenu.OnItemClicked(itemPrefab); // Pass the prefab reference
            }
            else
            {
                Debug.LogWarning("Attempted to select a destroyed or null item.");
            }
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
    }
}
