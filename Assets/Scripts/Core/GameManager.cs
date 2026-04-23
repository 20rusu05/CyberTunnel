using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Room Configuration")]
    [SerializeField] private List<Room> rooms = new List<Room>();

    [Header("Game State")]
    [SerializeField] private int currentRoomIndex = 0;
    [SerializeField] private float gameTimer;
    [SerializeField] private bool isGameActive;

    [Header("Events")]
    public UnityEvent<int> OnRoomCompleted;
    public UnityEvent OnAllRoomsCompleted;
    public UnityEvent<float> OnTimerUpdated;

    public int CurrentRoomIndex => currentRoomIndex;
    public int TotalRooms => rooms.Count;
    public float GameTimer => gameTimer;
    public bool IsGameActive => isGameActive;

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
        isGameActive = true;

        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].Initialize(i == 0);
        }
    }

    public void OnPuzzleSolved(int roomIndex)
    {
        if (roomIndex != currentRoomIndex) return;

        OnRoomCompleted?.Invoke(roomIndex);
        currentRoomIndex++;

        if (currentRoomIndex >= rooms.Count)
        {
            CompleteGame();
            return;
        }

        rooms[currentRoomIndex].Activate();
    }

    private void CompleteGame()
    {
        isGameActive = false;
        OnAllRoomsCompleted?.Invoke();
        Debug.Log($"Game completed in {gameTimer:F1} seconds!");
    }

    public void PauseGame()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isGameActive = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
