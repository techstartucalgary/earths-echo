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
    Respawn,
    LevelComplete,
    Cutscene,
    Settings,
    AudioSetting,
    GraphicSetting,
    ControlSetting,
    Inventory,
    Minimap
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameState currentState;
    // New field to store the previous state before entering Settings
    private GameState previousState;

    [Header("UI References")]
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject mainMenu;
    public GameObject levelCompleteMenu;
    public GameObject inventoryMenu;
    public GameObject miniMap;

    [Header("Settings UI References")]
    public GameObject settingsMenu;
    public GameObject audioSettingMenu;
    public GameObject graphicSettingMenu;
    public GameObject controlSettingMenu;




    [Header("Spawn Points")]
    public Transform defaultSpawnPoint;
    private Transform currentRespawnPoint;

    // If using a prefab, uncomment:
    // public GameObject playerPrefab;
    private GameObject playerInstance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);  // Start at Main Menu
    }

    private void Update()
    {
        // Basic inputs for pausing and opening menus
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else if (currentState == GameState.Settings ||
                    currentState == GameState.GraphicSetting ||
                     currentState == GameState.ControlSetting ||
                     currentState == GameState.AudioSetting ||
                    currentState == GameState.Inventory)
            {
                if(currentState == GameState.Settings)
                {
                    CloseSettings();
                }
                if (currentState == GameState.AudioSetting)
                {
                    CloseAudio();
                }
                if (currentState == GameState.ControlSetting)
                {
                    CloseControls();
                }
                if (currentState == GameState.GraphicSetting)
                {
                    CloseGraphics();
                } 
                if(currentState == GameState.Inventory)
                {
                    CloseInventory();
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (currentState == GameState.Playing)
                OpenInventory();
            else if (currentState == GameState.Inventory)
                CloseInventory();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (currentState == GameState.Playing)
                OpenMap();
            else if (currentState == GameState.Minimap)
                CloseMap();
        }
    }

    // Gameplay actions only valid in Playing
    public bool CanProcessGameplayActions()
    {
        return currentState == GameState.Playing;
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        // Reset UI
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        mainMenu.SetActive(false);
        inventoryMenu.SetActive(false);
        miniMap.SetActive(false);
        settingsMenu.SetActive(false); // Also disable settings UI by default
        audioSettingMenu.SetActive(false);
        controlSettingMenu.SetActive(false);
        graphicSettingMenu.SetActive(false);    

        switch (currentState)
        {
            case GameState.MainMenu:
                mainMenu.SetActive(true);
                Time.timeScale = 0f;
                AudioManager.instance.PlayMusic(AudioManager.instance.mainMenuMusic);
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                AudioManager.instance.PlayMusic(AudioManager.instance.gameplayMusic);
                //UnityEngine.Cursor.visible=false;   //cursor needs to be disabled during gameplay, but currently if G is pressing while playing, the cursor does not appear but it appears when accessed through pause menu
                break;

            case GameState.Paused:
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
                AudioManager.instance.PlayMusic(AudioManager.instance.pauseMusic);
                break;

            case GameState.GameOver:
                gameOverMenu.SetActive(true);
                Time.timeScale = 0f;
                AudioManager.instance.PlayMusic(AudioManager.instance.gameOverMusic);
                break;

            case GameState.Respawn:
                Time.timeScale = 1f;
                // Actually re-spawn the player here
                SetupSpawnPoints();
                InstantiatePlayer();
                AudioManager.instance.PlayMusic(AudioManager.instance.gameplayMusic);

                // Option 1: stay in Respawn state (player is now alive)
                // Option 2: immediately go back to Playing
                ChangeState(GameState.Playing);
                break;

            case GameState.LevelComplete:
                levelCompleteMenu.SetActive(true);
                Time.timeScale = 0f;
                AudioManager.instance.PlayMusic(AudioManager.instance.gameplayMusic);
                break;

            case GameState.Cutscene:
                Time.timeScale = 1f;
                // ...
                break;

            case GameState.Settings:
                settingsMenu.SetActive(true);
                Time.timeScale = 0f;
                break;

            case GameState.Inventory:
                inventoryMenu.SetActive(true);
                Time.timeScale = 0f;
                break;

            case GameState.Minimap:
                miniMap.SetActive(true);
                Time.timeScale = 0f;
                break;
            case GameState.AudioSetting:
                audioSettingMenu.SetActive(true);
                Time.timeScale = 0f;
                break;
            case GameState.ControlSetting:
                controlSettingMenu.SetActive(true);
                Time.timeScale = 0f;
                break;
            case GameState.GraphicSetting:
                graphicSettingMenu.SetActive(true);
                Time.timeScale = 0f;
                break;
        }
    }

    // Called (e.g.) from a button press or from the main menu
    public void StartGame()
    {
        // Could load a level if needed
        // SceneManager.LoadScene("Level1");

        // Do an initial spawn, then switch to Playing
        SetupSpawnPoints();
        InstantiatePlayer();

        ChangeState(GameState.Playing);
    }

    private void SetupSpawnPoints()
    {
        if (currentRespawnPoint == null && defaultSpawnPoint != null)
        {
            currentRespawnPoint = defaultSpawnPoint;
        }
    }

    public void SetRespawnPoint(Transform newRespawn)
    {
        currentRespawnPoint = newRespawn;
    }

    private void InstantiatePlayer()
    {
        // 1) Try to find an existing player if one isn't stored
        if (playerInstance == null)
        {
            playerInstance = GameObject.FindGameObjectWithTag("Player");
        }

        // 2) Optionally instantiate from a prefab if no Player is found
        // if (playerInstance == null && playerPrefab != null)
        // {
        //     playerInstance = Instantiate(playerPrefab, currentRespawnPoint.position, Quaternion.identity);
        // }

        if (playerInstance != null && currentRespawnPoint != null)
        {
            // Move the player to the respawn point
            playerInstance.transform.position = currentRespawnPoint.position;

            // Reset health bar
            HealthBar healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
            {
                healthBar.ResetHealthBar();
            }
            // Example of clearing velocity:
            // Rigidbody2D rb = playerInstance.GetComponent<Rigidbody2D>();
            // if (rb != null) rb.velocity = Vector2.zero;
        }
        else
        {
            Debug.LogWarning("No valid player or respawn point assigned!");
        }
    }

    // -- Public method so the Player (or another script) can call for a respawn
    public void RespawnPlayer()
    {
        ChangeState(GameState.Respawn);
    }

    // --- Basic Menu Management ---

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
        SceneManager.LoadScene(nextLevelName);
        ChangeState(GameState.Playing);
    }

    // Modified Settings functions:
    public void OpenSettings()
    {
        // Only store the previous state if we are not already in Settings.
        if (currentState != GameState.Settings)
        {
            previousState = currentState;
        }
        ChangeState(GameState.Settings);
    }


    public void CloseSettings()
    {
        // If the previous state was MainMenu, return to the MainMenu state.
        // Otherwise, restore the previous state.
        if (previousState == GameState.MainMenu)
        {
            ChangeState(GameState.MainMenu);
        }
        else
        {
            ChangeState(previousState);
        }
    }


    public void OpenInventory()
    {
        if (currentState != GameState.Inventory)
        {
            previousState = currentState;
        }
        ChangeState(GameState.Inventory);
    }

    public void CloseInventory()
    {
        if (previousState == GameState.Paused)
        {
            ChangeState(GameState.Paused);
        }
        else
        {
            ChangeState(previousState);
        }
    }

    public void OpenMap()
    {
        ChangeState(GameState.Minimap);
    }
    public void CloseMap()
    {
        ChangeState(GameState.Playing);
    }
    public void OpenAudio()
    {
        ChangeState(GameState.AudioSetting);
    }
    public void CloseAudio()
    {
        ChangeState(GameState.Settings);
    }
    public void OpenGraphics()
    {
        ChangeState(GameState.GraphicSetting);
    }
    public void CloseGraphics()
    {
        ChangeState(GameState.Settings);
    }
    public void OpenControls()
    {
        ChangeState(GameState.ControlSetting);
    }
    public void CloseControls()
    {
        ChangeState(GameState.Settings);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Re-spawn the Player once the scene reloads
        ChangeState(GameState.Respawn);
    }

    public void QuitToMainMenu()
    {
        gameOverMenu.SetActive(false);
        ChangeState(GameState.MainMenu);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
