using UnityEngine;

public class EE_Villager : EE_NPC
{
    [Header("Villager Settings")]
    public string villagerName;

    [Header("Dialogue Settings")]
    public bool doesSpeak = true;

    [SerializeField]
    Dialogue dialogueBox;
    public string[] lines;

    bool isTalking;
    BoxCollider2D speechRange;
    private bool playerNearby;

    [Header("Wander Settings")]
    public bool doesWander = true;
    public float wanderMaxDistance;
    private Transform spawnLocation;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        isTalking = false;
        speechRange = GetComponent<BoxCollider2D>();

        spawnLocation = new GameObject("SpawnLocation").transform;
        spawnLocation.position = gameObject.transform.position;

        target = new GameObject("TargetLocation").transform;
        target.position = spawnLocation.position;

        if (doesWander)
        {
            InvokeRepeating(nameof(WanderOnTimer), 5f, 5f);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        // Dialogue updates
        if (doesSpeak)
        {
            if (playerNearby && !isTalking && Input.GetKeyDown(KeyCode.E))
            {
                isTalking = true;
                PlaySpeech();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            playerNearby = true;
            Debug.Log($"Player enters dialogue range of '{villagerName}'. Press 'E' to talk.");

			// stop moving if player in range to talk
            target.position = new Vector3(
                gameObject.transform.position.x,
                gameObject.transform.position.y,
                gameObject.transform.position.z
            );
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            playerNearby = false;
            isTalking = false;
            dialogueBox.ForceEndDialogue();
            Debug.Log($"Player exits dialogue range of '{villagerName}'.");
        }
    }

    void PlaySpeech()
    {
        dialogueBox.lines = lines;
        dialogueBox.StartDialogue();
    }

    bool InRangeOf(Transform pos, float range) =>
        Vector3.Distance(gameObject.transform.position, pos.position) < range;

    void WanderOnTimer()
    {
		// do nothing if player in range to talk
		if (playerNearby) {
			return;
		}
        float newInterval = Random.Range(3f, 7f);
        CancelInvoke(nameof(WanderOnTimer));
        InvokeRepeating(nameof(WanderOnTimer), newInterval, newInterval);

        if (InRangeOf(spawnLocation, wanderMaxDistance))
        {
            float newTargetX = Random.Range(-Mathf.Abs(wanderMaxDistance), wanderMaxDistance);
            target.position = new Vector3(
                target.position.x + newTargetX,
                target.position.y,
                target.position.z
            );

            Debug.Log($"'{villagerName}' is wandering to {target.position}.");
        }
        else
        {
            target.position = spawnLocation.position;
        }
    }
}
