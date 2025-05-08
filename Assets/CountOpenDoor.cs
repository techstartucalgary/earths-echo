using UnityEngine;

public class CountItemDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private ItemSO requiredItem;
    [SerializeField] private int requiredCount = 3;
    [SerializeField] private Animator doorAnimator;

    private InventoryHandler inventoryHandler;
    [SerializeField] private Player player;
    private bool playerInRange = false;
    private bool doorOpened = false;

    void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }

        inventoryHandler = player.GetComponent<InventoryHandler>();
        if (inventoryHandler == null)
        {
            Debug.LogError("InventoryHandler not found on Player.");
            return;
        }

        if (doorAnimator == null)
        {
            doorAnimator = GetComponent<Animator>();
            if (doorAnimator == null)
            {
                Debug.LogError("Animator not assigned or found on GameObject.");
            }
        }
    }

    void Update()
    {
        if (!doorOpened && playerInRange)
        {
            TryOpenDoor();
        }
    }

    void TryOpenDoor()
    {
        int itemCount = inventoryHandler.GetItemCount(requiredItem);

        if (itemCount >= requiredCount)
        {
            Debug.Log("Door opened with item count: " + itemCount);
            doorAnimator.Play("door movement"); // Make sure this matches your Animator state name
            doorOpened = true;
        }
        else
        {
            Debug.Log($"Need {requiredCount} {requiredItem.itemName}, but only have {itemCount}.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
