using UnityEngine;

/// <summary>
/// Rotating mirror ball + spinning light rig for lobby disco vibe.
/// </summary>
public class DiscoBall : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 48f;
    [SerializeField] private Transform lightPivot;

    public void SetLightPivot(Transform pivot) => lightPivot = pivot;

    private void Awake()
    {
        if (lightPivot == null)
        {
            var p = transform.Find("LightPivot");
            if (p != null) lightPivot = p;
        }
    }

    private void Reset()
    {
        if (lightPivot == null)
        {
            var p = transform.Find("LightPivot");
            if (p != null) lightPivot = p;
        }
    }

    private void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
        if (lightPivot != null)
            lightPivot.Rotate(0f, -95f * Time.deltaTime, 0f, Space.Self);
    }
}
