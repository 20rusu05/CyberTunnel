using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("UI Reference")]
    [SerializeField] private GameObject interactionPrompt;

    private Camera mainCamera;
    private IInteractable currentTarget;

    private void Start()
    {
        FindCamera();
    }

    private void FindCamera()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
            mainCamera = GetComponentInChildren<Camera>();

        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
    }

    private void Update()
    {
        if (mainCamera == null)
        {
            FindCamera();
            if (mainCamera == null) return;
        }

        if (PuzzleUIManager.Instance != null && PuzzleUIManager.Instance.IsPuzzleOpen)
        {
            if (currentTarget != null)
            {
                currentTarget.OnLookAway();
                currentTarget = null;
            }

            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);

            return;
        }

        CheckForInteractable();

        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentTarget != interactable)
                {
                    currentTarget?.OnLookAway();
                    currentTarget = interactable;
                    currentTarget.OnLookAt();
                }

                if (interactionPrompt != null)
                    interactionPrompt.SetActive(true);

                return;
            }
        }

        if (currentTarget != null)
        {
            currentTarget.OnLookAway();
            currentTarget = null;
        }

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}

public interface IInteractable
{
    void Interact();
    void OnLookAt();
    void OnLookAway();
}
