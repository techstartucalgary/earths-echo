// In order to change the music in certain areas, use an empty gameobjet with a collider2d and musictrigger script attached
// Else, make an appropriate call to AudioManager.instance.SetGameplayMusic(stateForMusic); wherever necessary



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum GameplayContext
{
    Tutorial,
    TaintedGrove,
    VerdantHollow,
    CorruptedThicket1,
    CorruptedThicket2,
    CelestialGardens,
    Timberlands,
    Factory,
    EcoStation,
    HunterBossFight,
    TigerBossFight,
    RobotBossFight
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // Audio Source for background music
    private AudioSource musicSource;

    // Core Audio Clips
    public AudioClip mainMenuMusic;
    public AudioClip pauseMusic;
    public AudioClip gameOverMusic;
    
    // Default fallback clip for non-boss gameplay contexts.
    public AudioClip gameplayMusic;
    // Fallback for boss fights if a specific boss clip isn't assigned.
    public AudioClip fightMusic;
    
    // Dedicated Audio Clips for each gameplay context.
    public AudioClip TutorialMusic;
    public AudioClip TaintedGroveMusic;
    public AudioClip VerdantHollowMusic;
    public AudioClip CorruptedThicket1Music;
    public AudioClip CorruptedThicket2Music;
    public AudioClip CelestialGardensMusic;
    public AudioClip TimberlandsMusic;
    public AudioClip FactoryMusic;
    public AudioClip EcoStationMusic;
    public AudioClip HunterBossFightMusic;
    public AudioClip TigerBossFightMusic;
    public AudioClip RobotBossFightMusic;

    // Audio Mixer Group for music
    public AudioMixerGroup musicMixerGroup;

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

        // Initialize the music source
        musicSource = gameObject.AddComponent<AudioSource>();
        if (musicMixerGroup != null)
        {
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        }
        musicSource.loop = true;
    }

    private void Update()
    {
        // If dialogue is playing, stop the music.
        if (DialogueManager.GetInstance().dialogueIsPlaying)
        {
            StopMusic();
        }
        // Resume playback if there is a clip assigned but not playing.
        else if (!musicSource.isPlaying && musicSource.clip != null)
        {
            musicSource.Play();
        }
    }

    // Plays background music while preserving its relative playback position.
    public void PlayMusic(AudioClip clip)
    {
        // If the requested clip is already playing, do nothing.
        if (musicSource.clip == clip)
            return;

        // Save playback position of the current clip.
        if (musicSource.clip != null)
        {
            clipPlaybackPositions[musicSource.clip] = musicSource.time;
        }

        // Switch to the new clip.
        musicSource.clip = clip;

        // Resume from the saved playback position if it exists.
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

    // Stops music while saving its playback position.
    public void StopMusic()
    {
        if (musicSource.clip != null)
        {
            clipPlaybackPositions[musicSource.clip] = musicSource.time;
        }
        musicSource.Stop();
    }

    // Adjusts volume for music.
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    // Switches gameplay music based on context (e.g., level, boss fight) with an optional fade.
    public void SetGameplayMusic(GameplayContext context, float fadeDuration = 1f)
    {
        AudioClip newClip = null;
        switch (context)
        {
            case GameplayContext.Tutorial:
                newClip = TutorialMusic != null ? TutorialMusic : gameplayMusic;
                break;
            case GameplayContext.TaintedGrove:
                newClip = TaintedGroveMusic != null ? TaintedGroveMusic : gameplayMusic;
                break;
            case GameplayContext.VerdantHollow:
                newClip = VerdantHollowMusic != null ? VerdantHollowMusic : gameplayMusic;
                break;
            case GameplayContext.CorruptedThicket1:
                newClip = CorruptedThicket1Music != null ? CorruptedThicket1Music : gameplayMusic;
                break;
            case GameplayContext.CorruptedThicket2:
                newClip = CorruptedThicket2Music != null ? CorruptedThicket2Music : gameplayMusic;
                break;
            case GameplayContext.CelestialGardens:
                newClip = CelestialGardensMusic != null ? CelestialGardensMusic : gameplayMusic;
                break;
            case GameplayContext.Timberlands:
                newClip = TimberlandsMusic != null ? TimberlandsMusic : gameplayMusic;
                break;
            case GameplayContext.Factory:
                newClip = FactoryMusic != null ? FactoryMusic : gameplayMusic;
                break;
            case GameplayContext.EcoStation:
                newClip = EcoStationMusic != null ? EcoStationMusic : gameplayMusic;
                break;
            case GameplayContext.HunterBossFight:
                newClip = HunterBossFightMusic != null ? HunterBossFightMusic : fightMusic;
                break;
            case GameplayContext.TigerBossFight:
                newClip = TigerBossFightMusic != null ? TigerBossFightMusic : fightMusic;
                break;
            case GameplayContext.RobotBossFight:
                newClip = RobotBossFightMusic != null ? RobotBossFightMusic : fightMusic;
                break;
            default:
                newClip = gameplayMusic;
                break;
        }

        // Update the global gameplayMusic to the new clip so that future calls (like resume) use this clip.
        gameplayMusic = newClip;

        // Transition to the selected music with a fade.
        StartCoroutine(TransitionToMusic(newClip, fadeDuration));
    }


    // Coroutine to smoothly fade out the current music and fade in the new music.
    private IEnumerator TransitionToMusic(AudioClip newClip, float fadeDuration)
    {
        // Fade out current music if playing.
        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
            musicSource.volume = 0f;
            musicSource.Stop();
        }

        // Play the new clip.
        PlayMusic(newClip);

        // Fade in the new music.
        float targetVolume = 1f; // Use your desired default music volume.
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }
        musicSource.volume = targetVolume;
    }

}
