using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI roomText;
    [SerializeField] private TextMeshProUGUI interactPromptText;
    [SerializeField] private GameObject crosshair;

    [Header("Room Progress")]
    [SerializeField] private GameObject[] roomIndicators;
    [SerializeField] private Color completedColor = Color.green;
    [SerializeField] private Color currentColor = Color.yellow;
    [SerializeField] private Color lockedColor = Color.red;

    [Header("Camera label (lobby = 0, puzzle zones = 1–5)")]
    [SerializeField] private float lobbyMaxWorldZ = -3.5f;

    [Header("Completion Screen")]
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private TextMeshProUGUI completionTimeText;
    [SerializeField] private TextMeshProUGUI completionMessageText;

    private void Start()
    {
        if (completionPanel != null)
            completionPanel.SetActive(false);

        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.OnTimerUpdated.AddListener(UpdateTimer);
            gm.OnRoomCompleted.AddListener(UpdateRoomProgress);
            gm.OnAllRoomsCompleted.AddListener(ShowCompletionScreen);
        }
    }

    private void Update()
    {
        UpdateCameraLabel();
    }

    private void UpdateTimer(float time)
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void UpdateRoomProgress(int completedRoom)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        UpdateCameraLabel();

        for (int i = 0; i < roomIndicators.Length; i++)
        {
            var img = roomIndicators[i].GetComponent<UnityEngine.UI.Image>();
            if (img == null) continue;

            if (i <= completedRoom)
                img.color = completedColor;
            else if (i == completedRoom + 1)
                img.color = currentColor;
            else
                img.color = lockedColor;
        }
    }

    private void UpdateCameraLabel()
    {
        if (roomText == null) return;

        var p = FindFirstObjectByType<PlayerController>();
        if (p == null)
        {
            roomText.text = "Camera 0 / 5";
            return;
        }

        float z = p.transform.position.z;
        int cam;
        if (z < lobbyMaxWorldZ)
            cam = 0;
        else
        {
            int seg = Mathf.FloorToInt((z + 5f) / 15f);
            cam = Mathf.Clamp(seg + 1, 1, 5);
        }

        roomText.text = $"Camera {cam} / 5";
    }

    private void ShowCompletionScreen()
    {
        if (completionPanel == null) return;

        completionPanel.SetActive(true);

        float time = GameManager.Instance.GameTimer;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        completionTimeText.text = $"Completion Time: {minutes:00}:{seconds:00}";

        if (time < 180)
            completionMessageText.text = "ELITE HACKER - Exceptional performance!";
        else if (time < 300)
            completionMessageText.text = "SKILLED ANALYST - Great job!";
        else if (time < 600)
            completionMessageText.text = "JUNIOR OPERATIVE - Room for improvement.";
        else
            completionMessageText.text = "TRAINEE - Keep practicing your skills.";

        var player = FindFirstObjectByType<PlayerController>();
        if (player != null) player.CanMove = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(HideCompletionAfterDelay(10f));
    }

    private System.Collections.IEnumerator HideCompletionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (completionPanel != null)
            completionPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        var player = FindFirstObjectByType<PlayerController>();
        if (player != null) player.CanMove = true;
    }

    public void SetInteractPrompt(bool visible, string text = "Press [E] to interact")
    {
        if (interactPromptText == null) return;

        interactPromptText.gameObject.SetActive(visible);
        interactPromptText.text = text;
    }

    private void OnDestroy()
    {
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.OnTimerUpdated.RemoveListener(UpdateTimer);
            gm.OnRoomCompleted.RemoveListener(UpdateRoomProgress);
            gm.OnAllRoomsCompleted.RemoveListener(ShowCompletionScreen);
        }
    }
}
