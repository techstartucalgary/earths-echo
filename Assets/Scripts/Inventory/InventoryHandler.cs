using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    public int inventoryLimit = 20;
    // Dictionary to track each ItemSO and its count.
    private Dictionary<ItemSO, int> inventory = new Dictionary<ItemSO, int>();

    [SerializeField] private InventoryMenu inventoryMenu;
	private Player player;
    // Holder transforms for each equipped type.
    [SerializeField] private Transform meleeHolder;
    [SerializeField] private Transform projectileHolder;
    [SerializeField] private Transform handHolder;  // For general holdable items

    // Currently instantiated GameObject for the equipped item.
    private GameObject currentEquippedInstance;

    // Equipped state enum.
    private enum EquippedState { None, Melee, Projectile, Item, ThrowableItem }

	public bool IsItemEquipped {
    get { return activeState == EquippedState.Item; }
	}

	public bool IsProjectileWeaponEquipped {
		get { return activeState == EquippedState.Projectile; }
	}

    public bool isThrowableObjectEquipped {
        get {return activeState == EquippedState.ThrowableItem;}
    }
	public GameObject EquippedInstance 
	{ 
		get { return currentEquippedInstance; } 
	}

	public WeaponSO CurrentProjectileWeaponSO 
	{ 
		get { return currentProjectileWeaponSO; } 
	}

    public WeaponSO CurrentMeleeWeaponSO 
	{ 
		get { return currentMeleeWeaponSO; } 
	}


	private float defaultAttackDamage;
	private float defaultAttackCooldown;
	private float defaultAttackRange;
	private string defaultAnimPrefix;

    private EquippedState activeState = EquippedState.None;

    // Dynamic references set when items are equipped from the inventory.
    private WeaponSO currentMeleeWeaponSO;
    private WeaponSO currentProjectileWeaponSO;
    public ItemInstSO currentItemSO;  // General holdable item (non-weapon)

    private void Awake()
    {
        // Optionally assign inventoryMenu if not set in the Inspector.
        // inventoryMenu = FindObjectOfType<InventoryMenu>();
        
        player = FindObjectOfType<Player>();
        defaultAttackDamage = player.attackDamage;
		defaultAttackCooldown = player.attackCooldown;
		defaultAttackRange = player.attackRange;
		defaultAnimPrefix = player.attackAnimPrefix;
        
    }

    private void Update()
    {
        HandleEquippedSwitching();
        HandlePlayerAttackStats();
    }

    #region Inventory Management

    /// <summary>
    /// Adds an item (as an ItemSO) to the inventory.
    /// If the item is stackable and already exists, increases its count.
    /// </summary>
    public void AddItem(ItemSO newItem)
    {
        if (newItem == null)
        {
            Debug.LogError("Attempted to add a null item to the inventory.");
            return;
        }
        if (inventoryMenu == null)
        {
            Debug.LogError("InventoryMenu is not assigned in InventoryHandler.");
            return;
        }
        // If already in the inventory:
        if (inventory.ContainsKey(newItem))
        {
            if (newItem.stackable)
            {
                inventory[newItem]++;
                Debug.Log($"Increased stack of {newItem.itemName} to {inventory[newItem]}");
            }
            else
            {
                Debug.LogWarning("Item is already in inventory and is not stackable.");
                return;
            }
        }
        else
        {
            if (inventory.Count >= inventoryLimit)
            {
                Debug.LogWarning("Inventory is full. Cannot add more items.");
                return;
            }
            inventory[newItem] = 1;
            Debug.Log($"Added item: {newItem.itemName}");
        }
        inventoryMenu.UpdateEquippedHUD();
        UpdateUI();
    }

    /// <summary>
    /// Uses one instance of the specified item.
    /// When count reaches zero, the item is removed from the inventory,
    /// any equipped instance is unequipped, and the UI slot is cleared.
    /// </summary>
    public void UseItem(ItemSO item)
    {
        // Early exit if item is invalid.
        if (item == null || !inventory.ContainsKey(item) || !item.usable)
            return;

        // If it's a throwable item, call ShootProjectile.
        if (item is ThrowableItemSO)
        {
            Debug.Log($"Using throwable item: {item.itemName}");
            // Use a full pullback percentage (1f). Adjust if needed.
            ShootProjectile(1f);
        }

        // Common decrement logic for both basic and throwable items.
        if (item.stackable)
        {
            inventory[item]--;
            Debug.Log($"Used one {item.itemName}. New count: {inventory[item]}");

            if (inventory[item] <= 0)
            {
                inventory.Remove(item);
                Debug.Log($"{item.itemName} removed from inventory.");

                // Clear any UI selection and unequip if this item was currently equipped.
                foreach (var slot in inventoryMenu.itemSlots)
                {
                    if (slot.itemSO == currentItemSO)
                    {
                        slot.ClearSelectedIcon();
                    }
                }
                if (currentMeleeWeaponSO == item)
                {
                    UnequipCurrentItem();
                    currentMeleeWeaponSO = null;
                }
                if (currentProjectileWeaponSO == item)
                {
                    UnequipCurrentItem();
                    currentProjectileWeaponSO = null;
                }
                if (currentItemSO == item)
                {
                    UnequipCurrentItem();
                    currentItemSO = null;
                }
            }
        }
        else
        {
            Debug.Log($"Used {item.itemName} (non-stackable)");
            // Add any additional behavior for non-stackable non-throwable items here if needed.
        }
        UpdateUI();
        inventoryMenu.UpdateEquippedHUD();

    }





    /// <summary>
    /// Updates the inventory UI by passing a list of key–value pairs (ItemSO, count) to the InventoryMenu.
    /// </summary>
    private void UpdateUI()
    {
        List<KeyValuePair<ItemSO, int>> itemList = new List<KeyValuePair<ItemSO, int>>(inventory);
        inventoryMenu.UpdateInventoryUI(itemList);
    }

    /// <summary>
    /// Iterates through all inventory slots and clears any slot displaying the given item.
    /// </summary>

    public List<ItemSO> GetItems()
    {
        return new List<ItemSO>(inventory.Keys);
    }

    #endregion

    #region External Equip Methods

    /// <summary>
    /// Called externally (from InventoryMenu) to equip a melee weapon.
    /// </summary>
    public void EquipMeleeWeapon(WeaponSO weaponData)
    {
        if (weaponData == null)
        {
            Debug.LogError("EquipMeleeWeapon called with null weaponData.");
            return;
        }
        currentMeleeWeaponSO = weaponData;
        ActivateEquippedItem(EquippedState.Melee);
        inventoryMenu.UpdateEquippedHUD();

    }

    /// <summary>
    /// Called externally (from InventoryMenu) to equip a projectile weapon.
    /// </summary>
    public void EquipProjectileWeapon(WeaponSO weaponData)
    {
        if (weaponData == null)
        {
            Debug.LogError("EquipProjectileWeapon called with null weaponData.");
            return;
        }
        currentProjectileWeaponSO = weaponData;
        ActivateEquippedItem(EquippedState.Projectile);
        inventoryMenu.UpdateEquippedHUD();

    }

    /// <summary>
    /// Called externally (from InventoryMenu) to equip a general holdable item.
    /// </summary>
    public void EquipItem(ItemInstSO itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("EquipItem called with null itemData.");
            return;
        }
        // Check if the item is a throwable item.
        if (itemData is ThrowableItemSO)
        {
            currentItemSO = itemData;
            ActivateEquippedItem(EquippedState.ThrowableItem);
        }
        else
        {
            currentItemSO = itemData;
            ActivateEquippedItem(EquippedState.Item);
        }
        inventoryMenu.UpdateEquippedHUD();
    }

    #endregion

    #region Activation / Switching

    /// <summary>
    /// Handles switching between equipped states via key input.
    /// Alpha1: Melee, Alpha2: Projectile, Alpha3: General Item (toggle).
    /// </summary>
    private void HandleEquippedSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (activeState != EquippedState.Melee)
                ActivateEquippedItem(EquippedState.Melee);
            else
                Debug.Log("Melee weapon is already active.");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (activeState != EquippedState.Projectile)
                ActivateEquippedItem(EquippedState.Projectile);
            else
                Debug.Log("Projectile weapon is already active.");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // If there's an equipped item, check if it's throwable.
            if (currentItemSO != null && currentItemSO is ThrowableItemSO)
            {
                // Toggle throwable state.
                if (activeState != EquippedState.ThrowableItem)
                    ActivateEquippedItem(EquippedState.ThrowableItem);
                else
                    UnequipCurrentItem();
            }
            else
            {
                // Otherwise, treat it as a regular item.
                if (activeState == EquippedState.Item)
                    UnequipCurrentItem();
                else
                    ActivateEquippedItem(EquippedState.Item);
            }
        }
    }

    /// <summary>
    /// Activates an equipped item based on the desired state.
    /// Activating one type automatically unequips any previously active item.
    /// </summary>
    private void ActivateEquippedItem(EquippedState state)
    {
        if (activeState != EquippedState.None)
            UnequipCurrentItem();

        WeaponSO weaponData = null;
        Transform targetHolder = null;

        switch (state)
        {
            case EquippedState.Melee:
                weaponData = currentMeleeWeaponSO;
                targetHolder = meleeHolder;
                break;
            case EquippedState.Projectile:
                weaponData = currentProjectileWeaponSO;
                targetHolder = projectileHolder;
                break;
            case EquippedState.Item:
                if (currentItemSO == null)
                {
                    Debug.LogWarning("No general item equipped.");
                    return;
                }
                targetHolder = handHolder;
                break;
            case EquippedState.ThrowableItem:
                if (currentItemSO == null)
                {
                    Debug.LogWarning("No throwable item equipped.");
                    return;
                }
                targetHolder = handHolder;
                break;
            default:
                break;
        }

        // Instantiate holdable item for both Item and ThrowableItem states.
        if (state == EquippedState.Item || state == EquippedState.ThrowableItem)
        {
            InstantiateHoldable(currentItemSO, targetHolder);
            activeState = state;
            Debug.Log($"Activated {(state == EquippedState.ThrowableItem ? "Throwable Item" : "Item")}: {currentItemSO.itemName}");
        }
        else if (weaponData != null)
        {
            InstantiateWeapon(weaponData, targetHolder);
            activeState = state;
            Debug.Log($"Activated {state}: {weaponData.itemName}");
        }
        else
        {
            Debug.LogWarning($"No data set for state {state}.");
            activeState = EquippedState.None;
        }
    }

    /// <summary>
    /// Unequips the currently active item.
    /// Dynamic references remain intact for seamless reactivation.
    /// </summary>
    private void UnequipCurrentItem()
    {
        if (currentEquippedInstance != null)
        {
            Debug.Log($"Unequipping: {currentEquippedInstance.name}");
            Destroy(currentEquippedInstance);
            currentEquippedInstance = null;
        }
        activeState = EquippedState.None;
    }

    #endregion

    #region Instantiation Methods

    /// <summary>
    /// Instantiates a weapon (melee or projectile) using the provided WeaponSO data.
    /// </summary>
    private void InstantiateWeapon(WeaponSO weaponData, Transform parentHolder)
    {
        if (weaponData == null)
        {
            Debug.LogError("InstantiateWeapon called with null weaponData.");
            return;
        }
        if (currentEquippedInstance != null)
            Destroy(currentEquippedInstance);

        currentEquippedInstance = new GameObject(weaponData.itemName);
        currentEquippedInstance.transform.SetParent(parentHolder, false);
        currentEquippedInstance.transform.localScale = new Vector3(weaponData.itemScale, weaponData.itemScale, 0);
        currentEquippedInstance.transform.localPosition = Vector3.zero;
        currentEquippedInstance.transform.localRotation = Quaternion.identity;

        SpriteRenderer sr = currentEquippedInstance.AddComponent<SpriteRenderer>();
        ItemGameObject itemGO = currentEquippedInstance.AddComponent<ItemGameObject>();
        itemGO.item = weaponData;
    }

    /// <summary>
    /// Instantiates a holdable item (general item or projectile held as item) using the provided ItemSO data.
    /// Expects itemData to be of type ItemInstSO.
    /// </summary>
    private void InstantiateHoldable(ItemSO itemData, Transform parentHolder)
    {
        if (itemData == null)
        {
            Debug.LogError("InstantiateHoldable called with null itemData.");
            return;
        }
        ItemInstSO holdableItem = itemData as ItemInstSO;
        if (holdableItem == null)
        {
            Debug.LogWarning("Item is not holdable: " + itemData.itemName);
            return;
        }
        if (currentEquippedInstance != null)
            Destroy(currentEquippedInstance);

        currentEquippedInstance = new GameObject(holdableItem.itemName);
        currentEquippedInstance.transform.SetParent(parentHolder, false);
        currentEquippedInstance.transform.localScale = new Vector3(holdableItem.itemScale, holdableItem.itemScale, 0);
        currentEquippedInstance.transform.localPosition = Vector3.zero;
        currentEquippedInstance.transform.localRotation = Quaternion.identity;

        SpriteRenderer sr = currentEquippedInstance.AddComponent<SpriteRenderer>();
        ItemGameObject itemGO = currentEquippedInstance.AddComponent<ItemGameObject>();
        itemGO.item = holdableItem;
    }
    
    // Add this method to your InventoryHandler class
    public void ShootProjectile(float pullbackPercentage)
    {
        // Ensure that the equipped state is either Projectile or ThrowableItem.
        if (activeState != EquippedState.Projectile && activeState != EquippedState.ThrowableItem)
        {
            Debug.LogWarning("No projectile weapon or throwable item equipped.");
            return;
        }

        GameObject projectilePrefab = null;
        float projectileSpeed = 0f;

        if (activeState == EquippedState.Projectile)
        {
            if (currentProjectileWeaponSO == null)
            {
                Debug.LogWarning("No projectile weapon equipped.");
                return;
            }

            // Attempt to cast the equipped weapon to a ProjectileWeaponSO.
            ProjectileWeaponSO projWeapon = currentProjectileWeaponSO as ProjectileWeaponSO;
            if (projWeapon == null)
            {
                Debug.LogWarning("Equipped weapon is not a projectile weapon.");
                return;
            }
            projectilePrefab = projWeapon.projectilePrefab;
            projectileSpeed = projWeapon.projectileSpeed;
        }
        else if (activeState == EquippedState.ThrowableItem)
        {
            // For throwable items, we expect currentItemSO to be a ThrowableItemSO.
            ThrowableItemSO throwableItem = currentItemSO as ThrowableItemSO;
            if (throwableItem == null)
            {
                Debug.LogWarning("Equipped item is not a throwable item.");
                return;
            }
            int randomIndex = UnityEngine.Random.Range(0, throwableItem.projectilePrefab.Length);

            projectilePrefab = throwableItem.projectilePrefab[randomIndex];
            projectileSpeed = throwableItem.projectileSpeed;
        }

        if (currentEquippedInstance == null)
        {
            Debug.LogError("No equipped instance found for the projectile/throwable item.");
            return;
        }

        // Use the current equipped instance's transform as the spawn point.
        Vector3 spawnPosition = currentEquippedInstance.transform.position;
        Quaternion spawnRotation = currentEquippedInstance.transform.rotation;

        // Instantiate the projectile prefab.
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, spawnRotation);

        // Configure the projectile's initial velocity using the pullback percentage.
        ProjectileBehaviour pb = projectile.GetComponent<ProjectileBehaviour>();
        if (pb != null)
        {
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Scale projectile speed by the pullback percentage.
                float finalSpeed = projectileSpeed * pullbackPercentage;
                rb.velocity = spawnRotation * Vector2.right * finalSpeed;
            }
            
            // For physics projectiles, adjust gravity and damage based on pullback.
            if (pb.projectileType == ProjectileBehaviour.ProjectileType.Physics)
            {
                // Example: reduce gravity scale based on pullback (max reduction of 50%).
                float newGravityScale = pb.gravityScale * (1 - 0.5f * pullbackPercentage);
                pb.AdjustGravityScale(newGravityScale);
                
                // Adjust damage using the pullback percentage.
                pb.AdjustDamageScale(pullbackPercentage);
            }
        }
    }



    
	private void HandlePlayerAttackStats()
	{
		// Only update player's melee attack stats if a melee weapon is currently active.
		if (activeState == EquippedState.Melee && currentMeleeWeaponSO != null)
		{
			// Assuming the WeaponSO has these properties.
			player.attackDamage = currentMeleeWeaponSO.damage;
			player.attackCooldown = currentMeleeWeaponSO.cooldownTime;
			player.attackRange = currentMeleeWeaponSO.range;
			player.attackAnimPrefix = currentMeleeWeaponSO.animPrefix;
 		}
		else
		{
			// Revert to the player's default stats when no melee weapon is equipped.
			player.attackDamage = defaultAttackDamage;
			player.attackCooldown = defaultAttackCooldown;
			player.attackRange = defaultAttackRange;
			player.attackAnimPrefix = defaultAnimPrefix;
		}
	}


    #endregion
    public int GetItemCount(ItemSO item)
    {
        if (item != null && inventory.ContainsKey(item))
            return inventory[item];
        return 0;
    }
}
