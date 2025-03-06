using UnityEngine;
using System.Collections;
using Unity.Services.Analytics;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	Player player;
    [SerializeField] private InventoryHandler inventoryHandler;
    private ItemGameObject currentItemInRange;
	void Start () {
		player = GetComponent<Player> ();
	}

	void Update () {

        if (DialogueManager.GetInstance() != null && DialogueManager.GetInstance().dialogueIsPlaying)
        {
            return;
        }

        Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		player.SetDirectionalInput (directionalInput);

		if (Input.GetKeyDown (KeyCode.Space)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}

        // Check for sprint input (Left Shift key)
        player.SetSprinting(Input.GetKey(KeyCode.LeftShift));

        // Sliding logic
        if (Input.GetKeyDown(KeyCode.C) && player.isSprinting)
        {
            player.StartSlide();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            player.StopSlide();
        }

        if (currentItemInRange != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Picked up item: " + currentItemInRange.item.itemName);
            // Add the item to the inventory.
            inventoryHandler.AddItem(currentItemInRange.item);
            // Remove the item from the world.
            Destroy(currentItemInRange.gameObject);
            currentItemInRange = null;
        }
        // Attacks
        if (Input.GetMouseButtonDown(0)) {
            if (Input.GetKey(KeyCode.W)) {
                Debug.Log("Up Attack Triggered");
                player.PerformUpAttack(player.attackDamage,player.attackRange);
            }
            else if (Input.GetKey(KeyCode.S)) {
                Debug.Log("Down Attack Triggered");
                player.PerformDownAttack(player.attackDamage,player.attackRange);
            }
            else {
                Debug.Log("Side Attack Triggered");
                player.PerformSideAttack(player.attackDamage,player.attackRange);
            }
        }
        // Item usage (to be changed later this is for testing)
        if(inventoryHandler != null && inventoryHandler.currentItemSO != null && inventoryHandler.IsItemEquipped && Input.GetKeyDown(KeyCode.T))
        {
            inventoryHandler.UseItem(inventoryHandler.currentItemSO);
        }

		player.playerSpeed = player.velocity.x; // So we can see the current speed
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object has an ItemGameObject component.
        ItemGameObject itemGO = collision.GetComponent<ItemGameObject>();
        if (itemGO != null && itemGO.item != null)
        {
            // Store the reference so that the player can pick it up by pressing E.
            currentItemInRange = itemGO;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // If the item leaves the trigger area, clear the reference.
        ItemGameObject itemGO = collision.GetComponent<ItemGameObject>();
        if (itemGO != null && currentItemInRange == itemGO)
        {
            currentItemInRange = null;
        }
    }
}
