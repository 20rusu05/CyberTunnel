using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip puzzleSolvedClip;
    [SerializeField] private AudioClip puzzleFailedClip;
    [SerializeField] private AudioClip doorOpenClip;
    [SerializeField] private AudioClip interactClip;
    [SerializeField] private AudioClip typingClip;
    [SerializeField] private AudioClip victoryClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayPuzzleSolved() => PlaySFX(puzzleSolvedClip);
    public void PlayPuzzleFailed() => PlaySFX(puzzleFailedClip);
    public void PlayDoorOpen() => PlaySFX(doorOpenClip);
    public void PlayInteract() => PlaySFX(interactClip);
    public void PlayTyping() => PlaySFX(typingClip);
    public void PlayVictory() => PlaySFX(victoryClip);

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = Mathf.Clamp01(volume);
    }
}
