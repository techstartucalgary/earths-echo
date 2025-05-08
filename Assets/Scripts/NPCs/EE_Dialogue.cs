using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EE_Dialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public bool autoTrigger = false;                  // Triggers dialogue automatically when player enters
    public bool requireInteractKey = true;            // Wait for 'E' press if true
    public KeyCode interactKey = KeyCode.E;           // Key to start dialogue
    public string[] lines;                            // Dialogue lines
    public Dialogue dialogueBox;                      // Reference to Dialogue system

    private bool playerNearby;
    private bool isTalking;

    [SerializeField] private GameObject visualCue;


    private void Start()
    {
        if (dialogueBox == null)
        {
            dialogueBox = GetComponent<Dialogue>();
            if (dialogueBox == null)
                Debug.LogWarning($"EE_Dialogue on {gameObject.name} has no Dialogue component assigned.");
        }
        if (visualCue != null)
            visualCue.SetActive(false);
    }

    private void Update()
    {
        if (!playerNearby || isTalking || dialogueBox == null) return;
        if (playerNearby)
        {
            if (visualCue != null)
                visualCue.SetActive(true);
        }
        if (autoTrigger || (requireInteractKey && Input.GetKeyDown(interactKey)))
        {
            isTalking = true;
            dialogueBox.lines = lines;
            dialogueBox.StartDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;
        if (autoTrigger && !isTalking && dialogueBox != null)
        {
            isTalking = true;
            dialogueBox.lines = lines;
            dialogueBox.StartDialogue();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        isTalking = false;

        if (dialogueBox != null)
        {
            dialogueBox.ForceEndDialogue();
        }
    }
}
