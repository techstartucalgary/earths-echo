using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab; // Reference to the weapon prefab to be picked up
    private bool isPlayerNearby = false; // Flag to track if the player is within range
    private GameObject nearbyPlayer; // Reference to the player object

	private void Awake()
	{
		if (weaponPrefab == null)
		{
			Debug.LogError($"WeaponPickup on '{gameObject.name}' has a null weaponPrefab. Assign it in the Inspector.");
		}
	}


	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other != null && other.CompareTag("Player"))
		{
			isPlayerNearby = true;
			nearbyPlayer = other.gameObject;
			Debug.Log($"Player entered pickup range of '{gameObject.name}'. Press 'E' to pick up.");
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other != null && other.CompareTag("Player"))
		{
			isPlayerNearby = false;
			nearbyPlayer = null;
			Debug.Log($"Player left pickup range of '{gameObject.name}'.");
		}
	}


    private void Update()
    {
        // Check if the player is nearby and presses the E key
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PickupWeapon(nearbyPlayer);
        }
    }

	private void PickupWeapon(GameObject player)
	{
		if (weaponPrefab == null)
		{
			Debug.LogError($"WeaponPrefab is null on '{gameObject.name}'. Cannot pick up weapon.");
			return;
		}

		InventoryHandler inventoryHandler = player.GetComponent<InventoryHandler>();
		if (inventoryHandler == null)
		{
			Debug.LogWarning($"Player '{player.name}' does not have an InventoryHandler. Cannot add weapon to inventory.");
			return;
		}

		if (!inventoryHandler.GetItems().Contains(weaponPrefab))
		{
			Debug.Log($"Weapon picked up: {weaponPrefab.name}");
			inventoryHandler.AddItem(weaponPrefab);
			Destroy(gameObject); // Destroy the pickup object
		}
		else
		{
			Debug.LogWarning($"Player already has '{weaponPrefab.name}' in inventory.");
		}
	}


}
