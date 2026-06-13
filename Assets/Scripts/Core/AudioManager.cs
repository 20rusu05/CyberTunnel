using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources (set by builder)")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips (set by builder)")]
    [SerializeField] private AudioClip doorOpenClip;
    [SerializeField] private AudioClip doorCloseClip;
    [SerializeField] private AudioClip levelUnlockedClip;
    [SerializeField] private AudioClip gameLostClip;
    [SerializeField] private AudioClip lavaSizzleClip;
    [SerializeField] private AudioClip lavaScreamClip;
    [SerializeField] private AudioClip backgroundMusicClip;

    private const string MusicVolPref = "musicVolume";
    private const string SfxVolPref   = "sfxVolume";

    // Resources paths (fallback if serialized clips are missing)
    private const string ResDoor      = "Audio/door-open-close-sfx";
    private const string ResYouLost   = "Audio/you-lost-negative-beeps";
    private const string ResUnlocked  = "Audio/level-unlocked-sfx";
    private const string ResSizzle    = "Audio/lava-sizzle-sfx";
    private const string ResScream    = "Audio/lava-scream-sfx";

    // Dedicated 2D source for critical sounds (YOU LOST, lava).
    // Kept on this DontDestroyOnLoad object — safe across scene changes.
    private AudioSource _critSrc;

    private bool _gameLostPlayed;
    private GameObject _youLostGO;

    // ------------------------------------------------------------------ lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        EnsureSources();
        LoadMissingClips();
        ApplyVolumes();
        PlayMusic();
        Debug.Log("[AudioManager] Started. door=" + (doorOpenClip != null ? doorOpenClip.name : "NULL") +
                  " youLost=" + (gameLostClip != null ? gameLostClip.name : "NULL"));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        if (scene.name == "GameScene") ResetRun();
    }

    // ------------------------------------------------------------------ public API

    public void ResetRun()
    {
        _gameLostPlayed = false;
        if (_youLostGO != null) { Destroy(_youLostGO); _youLostGO = null; }
        LoadMissingClips();
        PlayMusic(forceRestart: true);
    }

    // Called from Door.Open()
    public void PlayDoorOpen()
    {
        AudioClip c = doorOpenClip ?? Resources.Load<AudioClip>(ResDoor);
        PlaySfxDirect(c, "DoorOpen");
    }

    // Called from Door.Close()
    public void PlayDoorClose()
    {
        AudioClip c = doorCloseClip ?? Resources.Load<AudioClip>(ResDoor);
        PlaySfxDirect(c, "DoorClose");
    }

    public void PlayLevelUnlocked()
    {
        PlaySfxDirect(levelUnlockedClip, "LevelUnlocked");
    }

    // Called from GameManager.LoseGame() BEFORE timeScale=0
    public void PlayGameLost()
    {
        if (_gameLostPlayed) return;
        _gameLostPlayed = true;

        if (musicSource != null) musicSource.Stop();

        // Always reload fresh from Resources to bypass any stale reference.
        AudioClip c = Resources.Load<AudioClip>(ResYouLost);
        if (c == null) c = gameLostClip;

        if (c == null)
        {
            Debug.LogError("[AudioManager] YOU LOST clip is NULL. File missing: Assets/Resources/Audio/you-lost-negative-beeps.mp3");
            return;
        }

        Debug.Log("[AudioManager] PlayGameLost -> " + c.name + " len=" + c.length + "s");

        // Spawn a fully independent GO — DontDestroyOnLoad so scene reload can't kill it.
        _youLostGO = new GameObject("_YouLostSFX");
        DontDestroyOnLoad(_youLostGO);
        AudioSource src = _youLostGO.AddComponent<AudioSource>();
        src.clip             = c;
        src.volume           = 1f;
        src.spatialBlend     = 0f;   // 2D
        src.ignoreListenerPause = true;
        src.Play();
        Destroy(_youLostGO, c.length + 2f);
    }

    public void PlayPuzzleSolved()  => PlaySfxDirect(levelUnlockedClip, "PuzzleSolved");
    public void PlayPuzzleFailed()  => PlaySfxDirect(null, "PuzzleFailed"); // no clip needed, silent OK
    public void PlayInteract()      => PlaySfxDirect(levelUnlockedClip, "Interact");
    public void PlayTyping()        { /* silent – no typing clip */ }
    public void PlayVictory()       => PlaySfxDirect(levelUnlockedClip, "Victory");

    public void PlayLavaDeath()
    {
        AudioClip s = lavaSizzleClip ?? Resources.Load<AudioClip>(ResSizzle);
        AudioClip sc = lavaScreamClip ?? Resources.Load<AudioClip>(ResScream);
        if (s  != null) PlaySfxDirect(s,  "LavaSizzle");
        if (sc != null) PlaySfxDirect(sc, "LavaScream");
    }

    public void SetMusicVolume(float v)
    {
        v = Mathf.Clamp01(v);
        if (musicSource != null) musicSource.volume = v;
        PlayerPrefs.SetFloat(MusicVolPref, v);
    }

    public void SetSFXVolume(float v)
    {
        v = Mathf.Clamp01(v);
        if (sfxSource != null) sfxSource.volume = v;
        PlayerPrefs.SetFloat(SfxVolPref, v);
    }

    public void RestartBackgroundMusicFromStart() => ResetRun();

    // ------------------------------------------------------------------ internals

    private void EnsureSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.spatialBlend = 0f;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;
        }

        if (_critSrc == null)
        {
            _critSrc = gameObject.AddComponent<AudioSource>();
            _critSrc.playOnAwake = false;
            _critSrc.spatialBlend = 0f;
            _critSrc.ignoreListenerPause = true;
        }
    }

    private void LoadMissingClips()
    {
        if (doorOpenClip  == null) doorOpenClip  = Resources.Load<AudioClip>(ResDoor);
        if (doorCloseClip == null) doorCloseClip = Resources.Load<AudioClip>(ResDoor);
        if (gameLostClip  == null) gameLostClip  = Resources.Load<AudioClip>(ResYouLost);
        if (levelUnlockedClip == null) levelUnlockedClip = Resources.Load<AudioClip>(ResUnlocked);
        if (lavaSizzleClip == null) lavaSizzleClip = Resources.Load<AudioClip>(ResSizzle);
        if (lavaScreamClip == null) lavaScreamClip = Resources.Load<AudioClip>(ResScream);
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = PlayerPrefs.GetFloat(MusicVolPref, 0.3f);
        if (sfxSource != null)
            sfxSource.volume = PlayerPrefs.GetFloat(SfxVolPref, 1f);
    }

    private void PlayMusic(bool forceRestart = false)
    {
        if (musicSource == null || backgroundMusicClip == null) return;
        musicSource.clip = backgroundMusicClip;
        musicSource.loop = true;
        if (forceRestart || !musicSource.isPlaying)
        {
            musicSource.Stop();
            musicSource.time = 0f;
            musicSource.Play();
        }
    }

    /// <summary>Direct 2D one-shot on sfxSource — used for doors and level sounds.</summary>
    private void PlaySfxDirect(AudioClip clip, string label)
    {
        if (clip == null) { Debug.LogWarning("[AudioManager] " + label + " clip is null"); return; }
        if (sfxSource == null) { Debug.LogWarning("[AudioManager] sfxSource is null"); return; }

        // Force volume to 1 — bypass player-prefs slider so it's always audible.
        float prevVol = sfxSource.volume;
        sfxSource.volume = 1f;
        sfxSource.PlayOneShot(clip);
        sfxSource.volume = prevVol;

        Debug.Log("[AudioManager] PlaySfxDirect: " + label + " -> " + clip.name);
    }
}
