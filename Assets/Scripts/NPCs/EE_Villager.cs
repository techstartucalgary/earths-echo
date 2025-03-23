using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EE_Villager : EE_NPC
{
    [Header("Villager Settings")]
    public string villagerName;

    [Header("Dialogue")]
	[SerializeField]
	Dialogue dialogueBox;
    public string[] lines;
	bool isTalking;

    BoxCollider2D speechRange;
    private bool playerNearby;

    // Start is called before the first frame update
    void Start()
    {
		isTalking = false;
        speechRange = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerNearby && !isTalking && Input.GetKeyDown(KeyCode.E))
        {
			isTalking = true;
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
			isTalking = false;
			dialogueBox.ForceEndDialogue();
            Debug.Log($"Player exits dialogue range of '{villagerName}'.");
        }
    }

    private void PlaySpeech()
    {
		dialogueBox.lines = lines;
		dialogueBox.StartDialogue();
    }
}
