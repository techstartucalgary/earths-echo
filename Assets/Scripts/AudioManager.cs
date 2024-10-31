using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // Audio Sources
    private AudioSource musicSource;  // For background music
    private AudioSource sfxSource;    // For sound effects

    // Audio Clips
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    public AudioClip pauseMusic;
    public AudioClip gameOverMusic;
    public AudioClip buttonClickSound;

    private void Awake()
    {
        // Ensure there's only one instance of the AudioManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Keep AudioManager persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;  // Loop background music
    }

    // if dialogue is playing stop music and sfx
    public void Update()
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            StopMusic();
            sfxSource.Stop();
        }
        else if (!musicSource.isPlaying)
        {
            PlayMusic(gameplayMusic);
        }
    }

    // Method to play background music
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;  // Don't restart the same music
        musicSource.clip = clip;
        musicSource.Play();
    }

    // Method to stop music
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Method to play sound effects (SFX)
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Optionally adjust volume
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
