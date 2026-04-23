using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PuzzleTerminal : MonoBehaviour, IInteractable
{
    [Header("Terminal Settings")]
    [SerializeField] private PuzzleBase linkedPuzzle;
    [SerializeField] private Renderer screenRenderer;
    [SerializeField] private Material activeScreenMaterial;
    [SerializeField] private Material inactiveScreenMaterial;

    [Header("Visual Feedback")]
    [SerializeField] private Light terminalLight;
    [SerializeField] private GameObject highlightEffect;

    private bool isInteractable;
    private bool isBeingLookedAt;
    private bool hasBeenInitialized;

    public PuzzleBase LinkedPuzzle => linkedPuzzle;

    private void Start()
    {
        if (highlightEffect != null)
            highlightEffect.SetActive(false);

        if (!hasBeenInitialized)
            SetInteractable(false);
    }

    public void SetInteractable(bool interactable)
    {
        hasBeenInitialized = true;
        isInteractable = interactable;

        if (screenRenderer != null)
        {
            screenRenderer.material = interactable
                ? activeScreenMaterial
                : inactiveScreenMaterial;
        }

        if (terminalLight != null)
            terminalLight.enabled = interactable;
    }

    public void Interact()
    {
        if (!isInteractable || linkedPuzzle == null || linkedPuzzle.IsSolved) return;
        if (PuzzleUIManager.Instance != null && PuzzleUIManager.Instance.IsPuzzleOpen) return;

        AudioManager.Instance?.PlayInteract();
        PuzzleUIManager.Instance?.ShowPuzzle(linkedPuzzle);

        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.CanMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void OnLookAt()
    {
        if (!isInteractable) return;

        isBeingLookedAt = true;

        if (highlightEffect != null)
            highlightEffect.SetActive(true);
    }

    public void OnLookAway()
    {
        isBeingLookedAt = false;

        if (highlightEffect != null)
            highlightEffect.SetActive(false);
    }
}
