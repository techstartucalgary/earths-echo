using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum GameplayContext
{
    Tutorial, TaintedGrove, VerdantHollow,
    CorruptedThicket1, CorruptedThicket2, CelestialGardens,
    Timberlands, Factory, EcoStation,
    HunterBossFight, TigerBossFight, RobotBossFight
}

public sealed class AudioManager : MonoBehaviour
{
    /* ─────────────────────────  SINGLETON  ───────────────────────── */
    public static AudioManager instance { get; private set; }

    /* ────────────────────────  INSPECTOR DATA  ───────────────────── */
    [Header("Core clips")]
    public AudioClip mainMenuMusic;
    public AudioClip pauseMusic;
    public AudioClip gameOverMusic;

    [Header("Fallbacks")]
    public AudioClip gameplayMusic;        // used if a level‑specific clip is missing
    public AudioClip fightMusic;           // used if a boss‑specific clip is missing

    [Header("Per‑level / per‑boss clips")]
    public AudioClip TutorialMusic, TaintedGroveMusic, VerdantHollowMusic,
                     CorruptedThicket1Music, CorruptedThicket2Music,
                     CelestialGardensMusic, TimberlandsMusic, FactoryMusic,
                     EcoStationMusic, HunterBossFightMusic, TigerBossFightMusic,
                     RobotBossFightMusic;

    [Header("Audio mixer")]
    public AudioMixerGroup musicMixerGroup;

    /* ───────────────────────────  RUNTIME  ────────────────────────── */
    readonly Dictionary<AudioClip,float> _clipPositions = new();
    AudioSource _music;
    const float _defaultVolume = 1f;       // change if you have a different master vol

    /* ───────────────────────────  UNITY LIFE  ────────────────────── */

    void Awake()
    {
        if (instance == null)    { instance = this;  DontDestroyOnLoad(gameObject); }
        else                     { Destroy(gameObject); return; }

        _music = gameObject.AddComponent<AudioSource>();
        if (musicMixerGroup) _music.outputAudioMixerGroup = musicMixerGroup;
        _music.loop = true;
    }

    void Update()
    {
        /* 1️⃣  SAFETY‑NET – clamp AudioListener position so WebAudio never
               receives NaN / ±Infinity (this is what was killing WebGL).   */
        SanitizeListenerTransform();

        /* 2️⃣  Dialogue pause behaviour (unchanged)                         */
        if (DialogueManager.GetInstance().dialogueIsPlaying) StopMusic();
        else if (!_music.isPlaying && _music.clip) _music.Play();
    }

    /* ─────────────────────────  PUBLIC API  ───────────────────────── */

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;           // silent fail (no crash)

        if (_music.clip == clip) return;    // already playing

        // remember position of the outgoing clip
        if (_music.clip) _clipPositions[_music.clip] = _music.time;

        // switch
        _music.clip = clip;
        _music.time = _clipPositions.TryGetValue(clip, out var t) ? t : 0f;
        _music.Play();
    }

    public void StopMusic()
    {
        if (_music.clip) _clipPositions[_music.clip] = _music.time;
        _music.Stop();
    }

    public void SetMusicVolume(float vol)  // guarantees finite 0‑1 range
    {
        _music.volume = Mathf.Clamp01(float.IsFinite(vol) ? vol : _defaultVolume);
    }

    public void SetGameplayMusic(GameplayContext ctx, float fade = 1f)
    {
        PlayWithFade(ChooseClip(ctx), fade);
    }

    /* ─────────────────────────  INTERNALS  ───────────────────────── */

    AudioClip ChooseClip(GameplayContext ctx) => ctx switch
    {
        GameplayContext.Tutorial           => TutorialMusic        ?? gameplayMusic,
        GameplayContext.TaintedGrove       => TaintedGroveMusic    ?? gameplayMusic,
        GameplayContext.VerdantHollow      => VerdantHollowMusic   ?? gameplayMusic,
        GameplayContext.CorruptedThicket1  => CorruptedThicket1Music?? gameplayMusic,
        GameplayContext.CorruptedThicket2  => CorruptedThicket2Music?? gameplayMusic,
        GameplayContext.CelestialGardens   => CelestialGardensMusic?? gameplayMusic,
        GameplayContext.Timberlands        => TimberlandsMusic     ?? gameplayMusic,
        GameplayContext.Factory            => FactoryMusic         ?? gameplayMusic,
        GameplayContext.EcoStation         => EcoStationMusic      ?? gameplayMusic,
        GameplayContext.HunterBossFight    => HunterBossFightMusic ?? fightMusic,
        GameplayContext.TigerBossFight     => TigerBossFightMusic  ?? fightMusic,
        GameplayContext.RobotBossFight     => RobotBossFightMusic  ?? fightMusic,
        _                                   => gameplayMusic
    };

    void PlayWithFade(AudioClip clip, float seconds)
    {
        if (clip == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(clip, seconds));
    }

    IEnumerator FadeRoutine(AudioClip next, float tDur)
    {
        /* fade‑out */
        for (float t = 0, s = _music.volume; t < tDur; t += Time.unscaledDeltaTime)
        {
            _music.volume = Mathf.Lerp(s, 0f, t / tDur); yield return null;
        }
        _music.volume = 0f; _music.Stop();

        /* swap & play */
        PlayMusic(next);

        /* fade‑in */
        for (float t = 0; t < tDur; t += Time.unscaledDeltaTime)
        {
            _music.volume = Mathf.Lerp(0f, _defaultVolume, t / tDur); yield return null;
        }
        _music.volume = _defaultVolume;
    }

    /* ─────────────────────  LISTENER SANITISER  ───────────────────── */

    static void SanitizeListenerTransform()
    {
        var lis = AudioListener.pause ? null : FindObjectOfType<AudioListener>();
        if (lis == null) return;

        Vector3 p = lis.transform.position;
        if (float.IsFinite(p.x) && float.IsFinite(p.y) && float.IsFinite(p.z)) return;

        Debug.LogWarning($"[AudioManager] Non‑finite listener position detected ({p}) – reset to (0,0,0)");
        lis.transform.position = Vector3.zero;
    }
}
