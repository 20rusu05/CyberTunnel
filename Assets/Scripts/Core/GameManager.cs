using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Room Configuration")]
    [SerializeField] private List<Room> rooms = new List<Room>();

    [Tooltip("Lobby → tunnel (Door_Entry). Sealed when room 0 is completed.")]
    [SerializeField] private Door lobbyEntryDoor;

    [Tooltip("Exit doors by room index: Door_i opens when room i is solved. When room i+1 completes, Door_i closes behind you.")]
    [SerializeField] private List<Door> roomExitDoors = new List<Door>();

    [Header("Game State")]
    [SerializeField] private int maxGlobalAttempts = 5;
    [SerializeField] private int currentRoomIndex = 0;
    [SerializeField] private int remainingGlobalAttempts;
    [SerializeField] private float gameTimer;
    [SerializeField] private bool isGameActive;

    [Header("Events")]
    public UnityEvent<int> OnRoomCompleted;
    public UnityEvent OnAllRoomsCompleted;
    public UnityEvent<int> OnGlobalAttemptsChanged;
    public UnityEvent OnGameLost;
    public UnityEvent<float> OnTimerUpdated;

    public int CurrentRoomIndex => currentRoomIndex;
    public int TotalRooms => rooms.Count;
    public float GameTimer => gameTimer;
    public bool IsGameActive => isGameActive;
    public int RemainingGlobalAttempts => remainingGlobalAttempts;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return null;
        StartGame();
    }

    private void Update()
    {
        if (!isGameActive) return;

        gameTimer += Time.deltaTime;
        OnTimerUpdated?.Invoke(gameTimer);
    }

    public void StartGame()
    {
        currentRoomIndex = 0;
        gameTimer = 0f;
        remainingGlobalAttempts = Mathf.Max(1, maxGlobalAttempts);
        isGameActive = true;
        OnGlobalAttemptsChanged?.Invoke(remainingGlobalAttempts);

        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].Initialize(i == 0);
        }
    }

    public void OnPuzzleSolved(int roomIndex)
    {
        if (roomIndex != currentRoomIndex) return;

        SealEntranceBehind(roomIndex);

        OnRoomCompleted?.Invoke(roomIndex);
        currentRoomIndex++;

        if (currentRoomIndex >= rooms.Count)
        {
            CompleteGame();
            return;
        }

        // Door.Open() itself handles the door sound.
        rooms[currentRoomIndex].Activate();
    }

    /// <summary>
    /// After a room is cleared, block going back: lobby door after room 0, previous segment door after later rooms.
    /// </summary>
    void SealEntranceBehind(int completedRoomIndex)
    {
        if (completedRoomIndex == 0)
        {
            lobbyEntryDoor?.Close();
            return;
        }

        int prevExitIdx = completedRoomIndex - 1;
        if (prevExitIdx >= 0 && roomExitDoors != null && prevExitIdx < roomExitDoors.Count)
            roomExitDoors[prevExitIdx]?.Close();
    }

    private void CompleteGame()
    {
        isGameActive = false;
        OnAllRoomsCompleted?.Invoke();
        Debug.Log($"Game completed in {gameTimer:F1} seconds!");
    }

    public void ConsumeGlobalAttempt()
    {
        if (!isGameActive) return;

        remainingGlobalAttempts = Mathf.Max(remainingGlobalAttempts - 1, 0);
        OnGlobalAttemptsChanged?.Invoke(remainingGlobalAttempts);

        if (remainingGlobalAttempts <= 0)
            LoseGame();
    }

    private void LoseGame()
    {
        if (!isGameActive) return;

        // Mark inactive immediately so double-calls are impossible.
        isGameActive = false;

        // Play YOU LOST sound first — needs real time before freeze.
        AudioManager.Instance?.PlayGameLost();

        // Disable player movement right away, but delay the timeScale=0 freeze
        // so the audio engine has time to start the clip (same-frame freeze kills it).
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null) player.CanMove = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(FreezeAfterSoundDelay());
    }

    private IEnumerator FreezeAfterSoundDelay()
    {
        // WaitForSecondsRealtime is NOT affected by Time.timeScale — always waits real time.
        yield return new WaitForSecondsRealtime(0.25f);

        Time.timeScale = 0f;
        OnGameLost?.Invoke();
    }

    public void RetryFromLobby()
    {
        Time.timeScale = 1f;
        AudioManager.Instance?.RestartBackgroundMusicFromStart();
        SceneLoader.Instance?.LoadScene("GameScene");
    }

    public void PauseGame()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Mouse look runs even when timeScale is 0 unless we disable the controller.
        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) pc.CanMove = false;
    }

    public void ResumeGame()
    {
        isGameActive = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) pc.CanMove = true;
    }
}
