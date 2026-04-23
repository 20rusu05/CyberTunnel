using UnityEngine;
using UnityEngine.Events;

public class Room : MonoBehaviour
{
    [Header("Room Configuration")]
    [SerializeField] private int roomIndex;
    [SerializeField] private string roomName;
    [SerializeField] private PuzzleBase puzzle;
    [SerializeField] private Door exitDoor;
    [SerializeField] private PuzzleTerminal terminal;

    [Header("Lighting")]
    [SerializeField] private Light roomLight;
    [SerializeField] private Color activeColor = new Color(0.2f, 0.8f, 0.4f);
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.1f, 0.1f);
    [SerializeField] private Color solvedColor = new Color(0.1f, 0.5f, 1f);

    [Header("Events")]
    public UnityEvent OnRoomActivated;
    public UnityEvent OnRoomCompleted;

    private bool isActive;
    private bool isCompleted;

    public int RoomIndex => roomIndex;
    public string RoomName => roomName;
    public bool IsActive => isActive;
    public bool IsCompleted => isCompleted;
    public PuzzleBase Puzzle => puzzle;

    public void Initialize(bool startActive)
    {
        isCompleted = false;

        if (puzzle != null)
        {
            puzzle.InitializePuzzle();
            puzzle.OnPuzzleSolved.AddListener(OnPuzzleSolved);
        }

        if (startActive)
            Activate();
        else
            Deactivate();
    }

    public void Activate()
    {
        isActive = true;

        if (roomLight != null)
            roomLight.color = activeColor;

        if (terminal != null)
            terminal.SetInteractable(true);

        OnRoomActivated?.Invoke();
    }

    public void Deactivate()
    {
        isActive = false;

        if (roomLight != null)
            roomLight.color = inactiveColor;

        if (terminal != null)
            terminal.SetInteractable(false);
    }

    private void OnPuzzleSolved()
    {
        isCompleted = true;
        isActive = false;

        if (roomLight != null)
            roomLight.color = solvedColor;

        if (terminal != null)
            terminal.SetInteractable(false);

        OnRoomCompleted?.Invoke();
        GameManager.Instance?.OnPuzzleSolved(roomIndex);
    }

    public void OpenExitDoor()
    {
        if (exitDoor != null)
            exitDoor.Open();
    }

    private void OnDestroy()
    {
        if (puzzle != null)
            puzzle.OnPuzzleSolved.RemoveListener(OnPuzzleSolved);
    }
}
