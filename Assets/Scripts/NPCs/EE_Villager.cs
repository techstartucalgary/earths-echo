using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EE_Villager : EE_NPC
{
    [Header("Villager Settings")]
    public string villagerName;

    [Header("Dialogue")]
    public string speech;
    BoxCollider2D speechRange;
    private bool playerNearby;

    // Start is called before the first frame update
    void Start()
    {
        speechRange = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            PlaySpeech();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log($"Player enters dialogue range of '{villagerName}'. Press 'E' to talk.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            playerNearby = false;
            Debug.Log($"Player exits dialogue range of '{villagerName}'.");
        }
    }

    private void PlaySpeech()
    {
        Debug.Log($"'{villagerName}' is talking.");
    }
}
