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
    private enum EquippedState { None, Melee, Projectile, Item }

	public bool IsItemEquipped {
    get { return activeState == EquippedState.Item; }
	}

	public bool IsProjectileWeaponEquipped {
		get { return activeState == EquippedState.Projectile; }
	}
	public GameObject EquippedInstance 
	{ 
		get { return currentEquippedInstance; } 
	}

	public WeaponSO CurrentProjectileWeaponSO 
	{ 
		get { return currentProjectileWeaponSO; } 
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
        UpdateUI();
    }

    /// <summary>
    /// Uses one instance of the specified item.
    /// When count reaches zero, the item is removed from the inventory,
    /// any equipped instance is unequipped, and the UI slot is cleared.
    /// </summary>
   public void UseItem(ItemSO item)
	{
		if (item == null || !inventory.ContainsKey(item) || !item.usable)
			return;
        if(item.stackable){
            // Decrement the count.
            inventory[item]--;
            Debug.Log($"Used one {item.itemName}. New count: {inventory[item]}");

            // If count is zero or less, remove the item.
            if (inventory[item] <= 0)
            {
                inventory.Remove(item);
                Debug.Log($"{item.itemName} removed from inventory.");

                // Clear selection only on the slot(s) that display this item.
                foreach (var slot in inventoryMenu.itemSlots)
                {
                    if (slot.itemSO == currentItemSO)
                    {
                        slot.ClearSelectedIcon();
                    }
                }
                // If the removed item is currently equipped, unequip it.
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
        else{
            Debug.Log($"Used {item.itemName} (non-stackable)");
        }
		// Only clear selection for the removed item; other slots remain as-is.
		UpdateUI();
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
        currentItemSO = itemData;
        ActivateEquippedItem(EquippedState.Item);
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
            // Toggle general item state.
            if (activeState == EquippedState.Item)
                UnequipCurrentItem();
            else
                ActivateEquippedItem(EquippedState.Item);
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
            default:
                break;
        }

        if (state == EquippedState.Item)
        {
            InstantiateHoldable(currentItemSO, targetHolder);
            activeState = state;
            Debug.Log($"Activated Item: {currentItemSO.itemName}");
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
		// Ensure a projectile weapon is active and valid.
		if (activeState != EquippedState.Projectile || currentProjectileWeaponSO == null)
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

		// Use the current equipped instance's transform as the spawn point.
		if (currentEquippedInstance == null)
		{
			Debug.LogError("No equipped instance found for the projectile weapon.");
			return;
		}
		
		Vector3 spawnPosition = currentEquippedInstance.transform.position;
		Quaternion spawnRotation = currentEquippedInstance.transform.rotation;

		// Instantiate the projectile prefab.
		GameObject projectile = Instantiate(projWeapon.projectilePrefab, spawnPosition, spawnRotation);

		// Configure the projectile's initial velocity using the pullback percentage.
		ProjectileBehaviour pb = projectile.GetComponent<ProjectileBehaviour>();
		if (pb != null)
		{
			Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
			if (rb != null)
			{
				// Scale projectile speed by the pullback percentage.
				float finalSpeed = projWeapon.projectileSpeed * pullbackPercentage;
				rb.velocity = spawnRotation * Vector2.right * finalSpeed;
			}
			
			// If it's a physics projectile, adjust gravity and damage.
			if (pb.projectileType == ProjectileBehaviour.ProjectileType.Physics)
			{
				// Example: reduce gravity scale based on pullback (max reduction of 50%).
				float newGravityScale = pb.gravityScale * (1 - 0.5f * pullbackPercentage);
				pb.AdjustGravityScale(newGravityScale);
				
				// Adjust damage using the pullback percentage (e.g. full pullback doubles the damage).
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
}
