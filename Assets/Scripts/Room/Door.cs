using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorTransform;
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private bool useRotation;
    [SerializeField] private Vector3 openRotation;
    [SerializeField] private Vector3 closedRotation;

    [Header("Visual Feedback")]
    [SerializeField] private Renderer doorRenderer;
    [SerializeField] private Material lockedMaterial;
    [SerializeField] private Material unlockedMaterial;

    [Header("Effects")]
    [SerializeField] private ParticleSystem openEffect;
    [SerializeField] private Light doorLight;
    [SerializeField] private Color lockedLightColor = Color.red;
    [SerializeField] private Color unlockedLightColor = Color.green;

    [SerializeField] private bool startOpen;

    private bool isOpen;
    private Coroutine animationCoroutine;

    public bool IsOpen => isOpen;

    private void Start()
    {
        if (doorTransform == null)
            doorTransform = transform;

        if (startOpen)
        {
            isOpen = true;
            if (doorTransform != null)
                doorTransform.localPosition = openPosition;
            SetVisualState(true);
        }
        else
        {
            SetVisualState(false);
        }
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        SetVisualState(true);
        AudioManager.Instance?.PlayDoorOpen();

        if (openEffect != null)
            openEffect.Play();

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimateDoor(true));
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        SetVisualState(false);
        AudioManager.Instance?.PlayDoorClose();

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(AnimateDoor(false));
    }

    private IEnumerator AnimateDoor(bool opening)
    {
        Vector3 targetPos = opening ? openPosition : closedPosition;
        Quaternion targetRot = Quaternion.Euler(opening ? openRotation : closedRotation);

        while (true)
        {
            doorTransform.localPosition = Vector3.Lerp(
                doorTransform.localPosition, targetPos, Time.deltaTime * openSpeed);

            if (useRotation)
            {
                doorTransform.localRotation = Quaternion.Lerp(
                    doorTransform.localRotation, targetRot, Time.deltaTime * openSpeed);
            }

            if (Vector3.Distance(doorTransform.localPosition, targetPos) < 0.01f)
            {
                doorTransform.localPosition = targetPos;
                if (useRotation) doorTransform.localRotation = targetRot;
                break;
            }

            yield return null;
        }
    }

    private void SetVisualState(bool unlocked)
    {
        if (doorRenderer != null)
        {
            doorRenderer.material = unlocked ? unlockedMaterial : lockedMaterial;
        }

        if (doorLight != null)
        {
            doorLight.color = unlocked ? unlockedLightColor : lockedLightColor;
        }
    }
}
