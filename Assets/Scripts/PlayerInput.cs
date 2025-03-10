using UnityEngine;
using System.Collections;
using Unity.Services.Analytics;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {

    Player player;
    [SerializeField] private InventoryHandler inventoryHandler;
    [SerializeField] private TrajectoryLine trajectoryLine;  // Reference to our TrajectoryLine component
    private ItemGameObject currentItemInRange;

    // Variables for projectile charge
    private float projectileChargeTime = 0f;
    [SerializeField] private float maxChargeTime = 1.0f; // Maximum charge time equals 100%

    void Start () {
        player = GetComponent<Player>();
    }

    void Update () {
        if (GameManager.instance != null && !GameManager.instance.CanProcessGameplayActions())
        return;
        
        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }

        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if (Input.GetKeyDown(KeyCode.Space)) {
            player.OnJumpInputDown();
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            player.OnJumpInputUp();
        }

        // Check for sprint input (Left Shift key)
        player.SetSprinting(Input.GetKey(KeyCode.LeftShift));

        // Sliding logic
        if (Input.GetKeyDown(KeyCode.C) && player.isSprinting) {
            player.StartSlide();
        }
        if (Input.GetKeyUp(KeyCode.C)) {
            player.StopSlide();
        }

        if (currentItemInRange != null && Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("Picked up item: " + currentItemInRange.item.itemName);
            inventoryHandler.AddItem(currentItemInRange.item);
            Destroy(currentItemInRange.gameObject);
            currentItemInRange = null;
        }

        // Process Attack Inputs
        // When a projectile weapon is equipped, use a charge mechanic and update trajectory line.
        if (inventoryHandler != null && (inventoryHandler.IsProjectileWeaponEquipped || inventoryHandler.isThrowableObjectEquipped))
        {
            // On fire button press, reset the charge timer.
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                projectileChargeTime = 0f;
            }
            // While the fire button is held, accumulate charge time (clamped to maxChargeTime).
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                projectileChargeTime += Time.deltaTime;
                projectileChargeTime = Mathf.Min(projectileChargeTime, maxChargeTime);
                
                // Calculate pullback percentage (0 to 1).
                float pullbackPercentage = projectileChargeTime / maxChargeTime;
                
                // Retrieve the currently equipped projectile weapon data.
                // (Assumes InventoryHandler exposes currentProjectileWeaponSO and currentEquippedInstance.)
                if (inventoryHandler.EquippedInstance != null && inventoryHandler.CurrentProjectileWeaponSO != null)
                {
                    ProjectileWeaponSO projWeapon = inventoryHandler.CurrentProjectileWeaponSO as ProjectileWeaponSO;
                    if (projWeapon != null)
                    {
                        // Calculate final speed based on pullback.
                        float finalSpeed = projWeapon.projectileSpeed * pullbackPercentage;
                        
                        // Use the equipped instance's transform as spawn data.
                        Vector3 spawnPosition = inventoryHandler.EquippedInstance.transform.position;
                        Quaternion spawnRotation = inventoryHandler.EquippedInstance.transform.rotation;
                        
                        // Calculate initial velocity.
                        Vector3 initialVelocity = spawnRotation * Vector2.right * finalSpeed;
                        
                        // Determine the effective gravity scale.
                        // For physics projectiles, we reduce gravity based on pullback.
                        ProjectileBehaviour prefabPb = projWeapon.projectilePrefab.GetComponent<ProjectileBehaviour>();
                        float effectiveGravityScale = 0f;
                        if (prefabPb != null && prefabPb.projectileType == ProjectileBehaviour.ProjectileType.Physics)
                        {
                            effectiveGravityScale = prefabPb.gravityScale * (1 - 0.5f * pullbackPercentage);
                        }
                        
                        // Update the trajectory line so it follows the cursor direction.
                        if (trajectoryLine != null)
                        {
                            trajectoryLine.RenderTrajectory(spawnPosition, initialVelocity, effectiveGravityScale);
                        }
                    }
                }
            }
            // On fire button release, fire the projectile and hide the trajectory line.
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                
                if (inventoryHandler.IsProjectileWeaponEquipped){
                    float pullbackPercentage = projectileChargeTime / maxChargeTime;
                    inventoryHandler.ShootProjectile(pullbackPercentage);
                }
                // If a throwable item is equipped, call UseItem to decrement its count.
                else if (inventoryHandler.isThrowableObjectEquipped)
                {
                    inventoryHandler.UseItem(inventoryHandler.currentItemSO);
                }
                
                projectileChargeTime = 0f;
                if (trajectoryLine != null)
                {
                    trajectoryLine.HideTrajectory();
                }
                else{
                    Debug.Log("No Trajectory Line Active");
                }
            }
            

        }
        else
        {
            // Process melee attacks if a projectile weapon is not equipped.
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !inventoryHandler.IsProjectileWeaponEquipped && !inventoryHandler.isThrowableObjectEquipped && !inventoryHandler.IsItemEquipped)
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
            // Ensure the trajectory line is hidden when not charging a shot.
            if (trajectoryLine != null)
            {
                trajectoryLine.HideTrajectory();
            }

        }
        if(Input.GetKeyDown(KeyCode.T)){
            if (inventoryHandler != null && inventoryHandler.IsItemEquipped)
            {
                Debug.Log("T key pressed.");
                inventoryHandler.UseItem(inventoryHandler.currentItemSO);
            }
        }

        
        player.playerSpeed = player.velocity.x; // For debugging: displays current speed
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemGameObject itemGO = collision.GetComponent<ItemGameObject>();
        if (itemGO != null && itemGO.item != null)
        {
            currentItemInRange = itemGO;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemGameObject itemGO = collision.GetComponent<ItemGameObject>();
        if (itemGO != null && currentItemInRange == itemGO)
        {
            currentItemInRange = null;
        }
    }
}



