using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    // Audio Mixer Groups
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    // Dictionary to store playback positions for each clip
    private Dictionary<AudioClip, float> clipPlaybackPositions = new Dictionary<AudioClip, float>();

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

        // Assign the mixer groups dynamically
        if (musicMixerGroup != null)
        {
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        }
        if (sfxMixerGroup != null)
        {
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        }

        musicSource.loop = true;  // Loop background music
    }

    private void Update()
    {
        // If dialogue is playing, stop music and sound effects.
        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            StopMusic();
            sfxSource.Stop();
        }
        // If the music source has a clip but isn’t playing, resume playback.
        else if (!musicSource.isPlaying && musicSource.clip != null)
        {
            musicSource.Play();
        }
    }

    // Method to play background music while preserving its relative position.
    public void PlayMusic(AudioClip clip)
    {
        // If the requested clip is already playing, do nothing.
        if (musicSource.clip == clip)
            return;

        // If there's a clip currently playing, store its playback position.
        if (musicSource.clip != null)
        {
            clipPlaybackPositions[musicSource.clip] = musicSource.time;
        }

        // Switch to the new clip.
        musicSource.clip = clip;

        // Check if we have a saved playback position for the new clip.
        if (clipPlaybackPositions.TryGetValue(clip, out float savedTime))
        {
            musicSource.time = savedTime;
        }
        else
        {
            musicSource.time = 0f;
        }

        musicSource.Play();
    }

    // Method to stop music while saving its playback position.
    public void StopMusic()
    {
        if (musicSource.clip != null)
        {
            clipPlaybackPositions[musicSource.clip] = musicSource.time;
        }
        musicSource.Stop();
    }

    // Method to play sound effects (SFX)
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Optionally adjust volume for music
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // Optionally adjust volume for sound effects
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
