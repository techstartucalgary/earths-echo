using UnityEngine;

public class MiniGameTrigger : MonoBehaviour
{
    public enum GameType { Memory, PotionWheel }

    [SerializeField] private GameType gameType;
    [SerializeField] private GameObject visualCue;

    private bool playerInRange;

    private MemoryMatchingGame memoryGame;
    private PotionMixingWheel potionGame;

    private void Start()
    {
        memoryGame = FindObjectOfType<MemoryMatchingGame>();
        potionGame = FindObjectOfType<PotionMixingWheel>();

        if (visualCue != null)
            visualCue.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (visualCue != null)
                visualCue.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                LaunchGame();
            }
        }
        else
        {
            if (visualCue != null)
                visualCue.SetActive(false);
        }
    }

    private void LaunchGame()
    {
        if (gameType == GameType.Memory && memoryGame != null)
        {
            memoryGame.StartGame();
        }
        else if (gameType == GameType.PotionWheel && potionGame != null)
        {
            potionGame.StartGame();
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