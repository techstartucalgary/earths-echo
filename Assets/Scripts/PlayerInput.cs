using UnityEngine;
using System.Collections;
using Unity.Services.Analytics;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {

    Player player;
    [SerializeField] private InventoryHandler inventoryHandler;
    [SerializeField] private TrajectoryLine trajectoryLine;  // Reference to our TrajectoryLine component
    private ItemGameObject currentItemInRange;

    // Variables for projectile charge (only used for projectile weapons)
    private float projectileChargeTime = 0f;
    [SerializeField] private float maxChargeTime = 1.0f; // Maximum charge time equals 100%
    
    void Start () {
        player = GetComponent<Player>();
    }

    void Update () {
        if (GameManager.instance != null && !GameManager.instance.CanProcessGameplayActions())
            return;
        
        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().dialogueIsPlaying)
            return;

        // Process movement and jump inputs.
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if (Input.GetKeyDown(KeyCode.Space))
            player.OnJumpInputDown();
        if (Input.GetKeyUp(KeyCode.Space))
            player.OnJumpInputUp();

        // Sprinting
        player.SetSprinting(Input.GetKey(KeyCode.LeftShift));

        // Sliding
        if (Input.GetKeyDown(KeyCode.C) && player.isSprinting)
            player.StartSlide();
        if (Input.GetKeyUp(KeyCode.C))
            player.StopSlide();

        // Item pickup
        if (currentItemInRange != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Picked up item: " + currentItemInRange.item.itemName);
            inventoryHandler.AddItem(currentItemInRange.item);
            Destroy(currentItemInRange.gameObject);
            currentItemInRange = null;
        }

        // Process Attack Inputs for projectile/throwable weapons.
        if (inventoryHandler != null && (inventoryHandler.IsProjectileWeaponEquipped || inventoryHandler.isThrowableObjectEquipped))
        {
            // --- PROJECTILE WEAPONS (e.g., bows with pullback mechanic) ---
            if (inventoryHandler.IsProjectileWeaponEquipped && inventoryHandler.CurrentProjectileWeaponSO != null)
            {
                // On fire button press: reset charge, play pullback sound, and start pullback animation.
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    projectileChargeTime = 0f;
                    
                    // Play pullback sound (using audio clip at index 0).
                    if (inventoryHandler.CurrentProjectileWeaponSO.audioClips != null &&
                        inventoryHandler.CurrentProjectileWeaponSO.audioClips.Length > 0)
                    {
                        SoundFXManager.Instance.PlaySoundFXClip(
                            inventoryHandler.CurrentProjectileWeaponSO.audioClips[0],
                            transform, 1f);
                    }
                    
                    // Start the pullback animation using the weapon's animation prefix.
                    player.PlayProjectilePullbackAnimation(inventoryHandler.CurrentProjectileWeaponSO.animPrefix);
                }
                
                // While the fire button is held, update the charge timer and blend tree parameter.
                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                {
                    player.isCharging = true;
                    projectileChargeTime += Time.deltaTime;
                    projectileChargeTime = Mathf.Min(projectileChargeTime, maxChargeTime);
                    
                    float pullbackPercentage = projectileChargeTime / maxChargeTime;
                    player.UpdateProjectilePullback(pullbackPercentage);
                    
                    // Update the trajectory line if needed.
                    if (trajectoryLine != null)
                    {
                        // (Insert your trajectory line update logic here)
                    }
                }
                
                // On fire button release, fire the projectile and reset the animation.
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    float pullbackPercentage = projectileChargeTime / maxChargeTime;
                    inventoryHandler.ShootProjectile(pullbackPercentage);
                    
                    projectileChargeTime = 0f;
                    player.ResetProjectileAnimation(inventoryHandler.CurrentProjectileWeaponSO.animPrefix);
                    
                    player.isCharging = false;


                    if (trajectoryLine != null)
                        trajectoryLine.HideTrajectory();
                }
            }
            // --- THROWABLE ITEMS (no pullback mechanic) ---
            else if (inventoryHandler.isThrowableObjectEquipped && inventoryHandler.currentItemSO != null)
            {
                // On fire button press: simply play the throw animation.
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    // Play the throw sound (using audio clip at index 0).
                    if (inventoryHandler.currentItemSO.audioClips != null &&
                        inventoryHandler.currentItemSO.audioClips.Length > 0)
                    {
                        SoundFXManager.Instance.PlaySoundFXClip(
                            inventoryHandler.currentItemSO.audioClips[0],
                            transform, 1f);
                    }
                    
                    // Play the throwable animation using its animation prefix.
                    player.PlayThrowableAnimation(inventoryHandler.currentItemSO.animPrefix);
                }
                
                // On fire button release: throw the item.
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    inventoryHandler.UseItem(inventoryHandler.currentItemSO);
                    
                    projectileChargeTime = 0f;
                    // Optionally, reset animation to idle if desired.
                    // player.ResetProjectileAnimation(inventoryHandler.currentItemSO.animPrefix);
                    if (trajectoryLine != null)
                        trajectoryLine.HideTrajectory();
                }
            }
        }
        else
        {
            // Process melee attacks if no projectile or throwable weapon is equipped.
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && 
                !inventoryHandler.IsProjectileWeaponEquipped && 
                !inventoryHandler.isThrowableObjectEquipped && 
                !inventoryHandler.IsItemEquipped)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    Debug.Log("Up Attack Triggered");
                    player.PerformUpAttack(player.attackDamage, player.attackRange);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    Debug.Log("Down Attack Triggered");
                    player.PerformDownAttack(player.attackDamage, player.attackRange);
                }
                else
                {
                    Debug.Log("Side Attack Triggered");
                    player.PerformSideAttack(player.attackDamage, player.attackRange);
                }
            }
            
            if (trajectoryLine != null)
                trajectoryLine.HideTrajectory();
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (inventoryHandler != null && inventoryHandler.IsItemEquipped)
            {
                Debug.Log("T key pressed.");
                inventoryHandler.UseItem(inventoryHandler.currentItemSO);
            }
        }
        
        // For debugging: display player's current speed.
        player.playerSpeed = player.velocity.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemGameObject itemGO = collision.GetComponent<ItemGameObject>();
        if (itemGO != null && itemGO.item != null)
            currentItemInRange = itemGO;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemGameObject itemGO = collision.GetComponent<ItemGameObject>();
        if (itemGO != null && currentItemInRange == itemGO)
            currentItemInRange = null;
    }
}
