using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    public int inventoryLimit = 20;

    // Item‑count map
    private readonly Dictionary<ItemSO, int> inventory = new();

    
    [SerializeField] private InventoryMenu inventoryMenu;
    [SerializeField] private Player player;   // drag the real player here

    [Header("Item Holders")]
    [SerializeField] private Transform meleeHolder;
    [SerializeField] private Transform projectileHolder;
    [SerializeField] private Transform handHolder;

    private GameObject currentEquippedInstance;

    private enum EquippedState { None, Melee, Projectile, Item, ThrowableItem }
    private EquippedState activeState = EquippedState.None;
    private EquippedState lastSyncedState = EquippedState.None;


    // Public getters
    public bool  IsItemEquipped             => activeState == EquippedState.Item;
    public bool  IsProjectileWeaponEquipped => activeState == EquippedState.Projectile;
    public bool  isThrowableObjectEquipped  => activeState == EquippedState.ThrowableItem;
    public GameObject EquippedInstance      => currentEquippedInstance;
    public WeaponSO CurrentProjectileWeaponSO => currentProjectileWeaponSO;
    public WeaponSO CurrentMeleeWeaponSO     => currentMeleeWeaponSO;

    // Current equipped data
    private WeaponSO   currentMeleeWeaponSO;
    private WeaponSO   currentProjectileWeaponSO;
    public  ItemInstSO currentItemSO;

    // Cached defaults (so we can restore when nothing equipped)
    private float       defaultAttackDamage;
    private float       defaultAttackCooldown;
    private float       defaultAttackRange;
    private string      defaultAnimPrefix;
    private AudioClip[] defaultAttackSounds;

    private Camera mainCamera;

    /* ─────────────────────────────────────────────  INITIALISATION ───────────────────────────────────────────── */

    private void Awake()
    {
        if (player == null)
            player = GetComponent<Player>();  // if handler lives on Player
        if (player == null)
            player = GameObject.FindWithTag("Player")?.GetComponent<Player>();

        if (player == null)
        {
            Debug.LogError("[InventoryHandler] No Player reference!");
            enabled = false;
            return;
        }

        defaultAttackDamage   = player.attackDamage;
        defaultAttackCooldown = player.attackCooldown;
        defaultAttackRange    = player.attackRange;
        defaultAnimPrefix     = player.attackAnimPrefix;
        defaultAttackSounds   = player.attackSounds;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null) Debug.LogError("Main camera not found");
    }

    private void Update()
    {
        HandleEquippedSwitching();   // only key‑press handling now
        if (activeState != lastSyncedState)
        {
            ApplyStatsToPlayer();
            lastSyncedState = activeState;
        }
    }

    /* ─────────────────────────────────────────────  PUBLIC INVENTORY API ─────────────────────────────────────── */

    public void AddItem(ItemSO newItem)
    {
        if (newItem == null) return;
        if (inventory.Count >= inventoryLimit && !inventory.ContainsKey(newItem)) return;

        inventory.TryAdd(newItem, 0);
        if (newItem.stackable) inventory[newItem]++;   // stackable counts
        else                   inventory[newItem] = 1; // non‑stackable

        UpdateUI();
        inventoryMenu.UpdateEquippedHUD();
    }

/* ─────────────────────────────────  USE / CONSUME ITEM  ───────────────────────────── */

public void UseItem(ItemSO item)
{
    if (item == null || !inventory.ContainsKey(item) || !item.usable) return;

    /* ❶ run the item’s behaviour (e.g. throw) */
    if (item is ThrowableItemSO) ShootProjectile(1f);

    /* ❷ consume one and decide if it’s gone */
    bool itemDepleted = false;

    if (item.stackable)
    {
        inventory[item]--;
        if (inventory[item] <= 0)
        {
            inventory.Remove(item);
            itemDepleted = true;
        }
    }
    else
    {
        // non‑stackable → remove immediately
        inventory.Remove(item);
        itemDepleted = true;
    }

    /* ❸ if we just consumed the last one, unequip & clear HUD selection */
    if (itemDepleted)
    {
        // clear any UI highlights in the slots
        foreach (var slot in inventoryMenu.itemSlots)
            if (slot.itemSO == item) slot.ClearSelectedIcon();

        if (currentItemSO == item)
        {
            UnequipCurrentItem();
            currentItemSO = null;
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
    }

    /* ❹ refresh UI */
    UpdateUI();
    inventoryMenu.UpdateEquippedHUD();
}


    /* ─────────────────────────────────────────────  EQUIP / UNEQUIP ─────────────────────────────────────────── */

    public void EquipMeleeWeapon(WeaponSO weaponData)
    {
        if (weaponData == null) return;

        currentMeleeWeaponSO    = weaponData;
        currentItemSO           = null;

        ActivateEquippedItem(EquippedState.Melee);
    }

    public void EquipProjectileWeapon(WeaponSO weaponData)
    {
        if (weaponData == null) return;

        currentProjectileWeaponSO = weaponData;
        currentItemSO             = null;

        ActivateEquippedItem(EquippedState.Projectile);
    }

    public void EquipItem(ItemInstSO itemData)
    {
        if (itemData == null) return;

        currentItemSO            = itemData;

        var state = itemData is ThrowableItemSO ? EquippedState.ThrowableItem
                                                : EquippedState.Item;

        ActivateEquippedItem(state);
    }

    /* ─────────────────────────────────────────────  SWITCHING HELPER  ────────────────────────────────────────── */

    private void HandleEquippedSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ActivateEquippedItem(EquippedState.Melee);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ActivateEquippedItem(EquippedState.Projectile);
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (currentItemSO is ThrowableItemSO)
                ActivateEquippedItem(EquippedState.ThrowableItem);
            else
                ActivateEquippedItem(EquippedState.Item);
        }
    }

    private void ActivateEquippedItem(EquippedState state)
    {
        if (state == activeState) return;          // already active
        UnequipCurrentItem();

        switch (state)
        {
            case EquippedState.Melee:
                if (currentMeleeWeaponSO) InstantiateWeapon(currentMeleeWeaponSO, meleeHolder);
                break;

            case EquippedState.Projectile:
                if (currentProjectileWeaponSO) InstantiateWeapon(currentProjectileWeaponSO, projectileHolder);
                break;

            case EquippedState.Item:
            case EquippedState.ThrowableItem:
                if (currentItemSO) InstantiateHoldable(currentItemSO, handHolder);
                break;
        }

        activeState = state;
        ApplyStatsToPlayer();          // ← only happens once per equip
        inventoryMenu.UpdateEquippedHUD();
    }

    private void UnequipCurrentItem()
    {
        if (currentEquippedInstance) Destroy(currentEquippedInstance);
        currentEquippedInstance = null;
        activeState             = EquippedState.None;

        ApplyStatsToPlayer();          // revert to defaults
    }

    /* ─────────────────────────────────────────────  PLAYER STAT SYNC  ───────────────────────────────────────── */

    private void ApplyStatsToPlayer()
    {
        switch (activeState)
        {
            case EquippedState.Melee when currentMeleeWeaponSO:
                player.attackDamage     = currentMeleeWeaponSO.damage;
                player.attackCooldown   = currentMeleeWeaponSO.cooldownTime;
                player.attackRange      = currentMeleeWeaponSO.range;
                player.attackAnimPrefix = currentMeleeWeaponSO.animPrefix;
                player.attackSounds     = currentMeleeWeaponSO.audioClips;
                break;

            case EquippedState.Projectile when currentProjectileWeaponSO:
                player.attackAnimPrefix = currentProjectileWeaponSO.animPrefix;
                player.attackSounds     = currentProjectileWeaponSO.audioClips;
                break;

            case EquippedState.Item when currentItemSO:
                player.attackSounds     = currentItemSO.audioClips;
                break;

            case EquippedState.ThrowableItem when currentItemSO:
                player.attackSounds     = currentItemSO.audioClips;
                break;

            default: // none equipped – restore defaults
                player.attackDamage     = defaultAttackDamage;
                player.attackCooldown   = defaultAttackCooldown;
                player.attackRange      = defaultAttackRange;
                player.attackAnimPrefix = defaultAnimPrefix;
                player.attackSounds     = defaultAttackSounds;
                break;
        }
        Debug.Log($"[InventoryHandler] Now using prefix {player.attackAnimPrefix}  dmg {player.attackDamage}");

    }

    /* ─────────────────────────────────────────────  INSTANTIATION ──────────────────────────────────────────── */

    private void InstantiateWeapon(WeaponSO weaponData, Transform holder)
    {
        currentEquippedInstance = new GameObject(weaponData.itemName);
        currentEquippedInstance.transform.SetParent(holder, false);
        currentEquippedInstance.transform.localScale    = Vector3.one * weaponData.itemScale;

        currentEquippedInstance.AddComponent<SpriteRenderer>();
        currentEquippedInstance.AddComponent<ItemGameObject>().item = weaponData;
    }

    private void InstantiateHoldable(ItemSO itemData, Transform holder)
    {
        currentEquippedInstance = new GameObject(itemData.itemName);
        currentEquippedInstance.transform.SetParent(holder, false);
        currentEquippedInstance.transform.localScale    = Vector3.one * itemData.itemScale;

        currentEquippedInstance.AddComponent<SpriteRenderer>();
        currentEquippedInstance.AddComponent<ItemGameObject>().item = itemData;
    }

    /* ─────────────────────────────────────────────  UI HELPERS ─────────────────────────────────────────────── */

    private void UpdateUI()
    {
        var list = new List<KeyValuePair<ItemSO,int>>(inventory);
        inventoryMenu.UpdateInventoryUI(list);
    }

    public int GetItemCount(ItemSO item) => inventory.TryGetValue(item, out var c) ? c : 0;

    /* ─────────────────────────────────────────────  PROJECTILES (unchanged) ────────────────────────────────── */
    // ── keep your ShootProjectile(...) method here ──
        /// <summary>
    /// Instantiates a holdable item (general item or projectile held as item) using the provided ItemSO data.
    /// Expects itemData to be of type ItemInstSO.
    /// </summary>
    private Quaternion MouseToDegrees(){
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z= 0f;
        // Calculate direction from weapon to mouse
        Vector3 direction = mouseWorldPosition - transform.position;
        direction = direction.normalized;
        // Get the target angle in degrees
        float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
        //angle -=45f;
        return Quaternion.Euler(0f,0f,angle);
    }
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
		Quaternion spawnRotation = MouseToDegrees();

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
				rb.velocity = spawnRotation * Vector2.left * finalSpeed;
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

		// === Projectile Weapon Sound Integration ===
		// Play the shoot sound (clip at index 1) when the projectile is fired.
		if (activeState == EquippedState.Projectile && currentProjectileWeaponSO != null)
		{
			if (currentProjectileWeaponSO.audioClips != null && currentProjectileWeaponSO.audioClips.Length >= 2)
			{
				SoundFXManager.Instance.PlaySoundFXClip(currentProjectileWeaponSO.audioClips[1], transform, 0.5f);
			}
			else
			{
				Debug.LogWarning("Projectile weapon audio clips not set correctly (need at least 2 clips).");
			}
		}
		else if (activeState == EquippedState.ThrowableItem && currentItemSO != null)
		{
			if (currentItemSO.audioClips != null && currentItemSO.audioClips.Length >= 2)
			{
				SoundFXManager.Instance.PlaySoundFXClip(currentItemSO.audioClips[1], transform, 0.5f);
			}
			else
			{
				Debug.LogWarning("Throwable item audio clips not set correctly (need at least 2 clips).");
			}
		}
	}
}
