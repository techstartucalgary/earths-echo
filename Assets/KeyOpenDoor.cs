using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyOpenDoor : MonoBehaviour
{
    [SerializeField] private ItemInstSO acceptedKey;
    [SerializeField] private Player player;
    private InventoryHandler inventoryHandler;
    [SerializeField] private Animator doorAnimator;

    private bool playerInRange = false;

    void Start()
    {
        // if (player == null)
        // {
        //     player = FindAnyObjectByType<Player>();

        // }
        // else{
        //     Debug.LogError("Player not found.");
        //     return;
        // }

        inventoryHandler = player.GetComponent<InventoryHandler>();


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
        if (inventoryHandler == null)
        {
            Debug.LogError("InventoryHandler not found on Player.");
            return;
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.T))
        {
            openDoor();
        }
    }

    void openDoor()
    {
        if (inventoryHandler == null || acceptedKey == null || doorAnimator == null)
        {
            Debug.LogError("Error with setup");
            return;
        }

        if (inventoryHandler.currentItemSO != null &&
            (inventoryHandler.currentItemSO == acceptedKey ||
            inventoryHandler.currentItemSO.itemName == acceptedKey.itemName))
        {
            inventoryHandler.UseItem(inventoryHandler.currentItemSO);
            doorAnimator.Play("door movement");
        }
        else
        {
            Debug.Log("Player does not have the correct item equipped.");
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
