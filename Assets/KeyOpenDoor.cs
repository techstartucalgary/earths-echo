using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyOpenDoor : MonoBehaviour
{
    [SerializeField] private ScriptableObject acceptedKey;
    private Player player;
    private InventoryHandler inventoryHandler;
    private Animator anim;

    private bool playerInRange = false;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
            return;
        }

        inventoryHandler = player.GetComponent<InventoryHandler>();
        if (inventoryHandler == null)
        {
            Debug.LogError("InventoryHandler not found on Player.");
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator not found on Door.");
        }
    }


    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.T))
        {
            openDoor();
        }
    }

    void openDoor()
    {
        if (inventoryHandler == null || acceptedKey == null || anim == null){
            Debug.Log("Error with setup");
        }

        if (inventoryHandler.currentItemSO == acceptedKey)
        {
            inventoryHandler.UseItem(inventoryHandler.currentItemSO);
            anim.Play("door movement");
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
