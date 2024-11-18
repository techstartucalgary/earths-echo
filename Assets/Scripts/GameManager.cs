using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UIElements;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    LevelComplete,
    Cutscene,
    Settings,
    Inventory
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameState currentState;

    private GameObject playerInstance;  // This will hold the instantiated player
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject mainMenu;
    public GameObject levelCompleteMenu;
    public GameObject settingsMenu;
    public GameObject inventoryMenu;

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Keeps the GameManager persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);  // Initial state
    }

    private void Update()
    {
        // Handle basic inputs for pausing and opening menus
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                Debug.Log("Game Paused");  // Add this line for debugging
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (currentState == GameState.Playing)
            {
                OpenInventory();
            }
            else if (currentState == GameState.Inventory)
            {
                CloseInventory();
            }
        }
    }
    public bool CanProcessGameplayActions()
    {
        // Gameplay actions are allowed only when in the Playing state
        return currentState == GameState.Playing;
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        // Reset UI elements visibility
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        mainMenu.SetActive(false);
        //levelCompleteMenu.SetActive(false);
        //settingsMenu.SetActive(false);
        inventoryMenu.SetActive(false);
        //make sure player is not in control of input

        switch (currentState)
        {
            case GameState.MainMenu:
                mainMenu.SetActive(true);
                Time.timeScale = 0f;
                AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuMusic);  // Play main menu music
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                InstantiatePlayer();
                AudioManager.instance.PlayMusic(AudioManager.instance.gameplayMusic);  // Play gameplay music
                break;

            case GameState.Paused:
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
                AudioManager.instance.PlayMusic(AudioManager.instance.pauseMusic);  // Play pause music
                break;

            case GameState.GameOver:
                gameOverMenu.SetActive(true);
                Time.timeScale = 0f;
                AudioManager.instance.PlayMusic(AudioManager.instance.gameOverMusic);  // Play game over music
                break;

            case GameState.LevelComplete:
                levelCompleteMenu.SetActive(true);
                Time.timeScale = 0f;
                AudioManager.instance.PlayMusic(AudioManager.instance.gameplayMusic);  // Keep gameplay music or add victory music
                break;

            case GameState.Cutscene:
                Time.timeScale = 1f;  // Cutscenes may require time movement
                // Play cutscene here
                break;

            case GameState.Settings:
                settingsMenu.SetActive(true);
                Time.timeScale = 0f;
                break;

            case GameState.Inventory:
                inventoryMenu.SetActive(true);
                Time.timeScale = 0f;  // Freeze time while in the inventory
                break;
        }
    }

    public void StartGame()
    {
        // Load the first level and set the state to Playing
        //SceneManager.LoadScene("Level1");  // Replace with the actual scene name
        ChangeState(GameState.Playing);
    }

    private void InstantiatePlayer()
    {
        // Find the player and instantiate him instead of creating it from the prefab
        playerInstance = GameObject.FindGameObjectWithTag("Player");
    }

    public void PauseGame()
    {
        ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        ChangeState(GameState.Playing);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    public void CompleteLevel()
    {
        ChangeState(GameState.LevelComplete);
    }

    public void StartNextLevel(string nextLevelName)
    {
        // Load next level and transition back to Playing state
        SceneManager.LoadScene(nextLevelName);
        ChangeState(GameState.Playing);
    }

    public void OpenSettings()
    {
        ChangeState(GameState.Settings);
    }

    public void CloseSettings()
    {
        ChangeState(GameState.MainMenu);
    }

    public void OpenInventory()
    {
        ChangeState(GameState.Inventory);
    }

    public void CloseInventory()
    {
        ChangeState(GameState.Playing);
    }

    public void RestartGame()
    {
        // Reload the current scene and reset the game state
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ChangeState(GameState.Playing);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");  // Replace with actual Main Menu scene name
        ChangeState(GameState.MainMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
