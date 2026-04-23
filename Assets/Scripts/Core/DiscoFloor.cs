using UnityEngine;

/// <summary>
/// Dancefloor: shifts floor tint / emission over time (lobby + exit).
/// </summary>
public class DiscoFloor : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private float hueSpeed = 0.22f;
    [SerializeField] private float emissionMul = 1.6f;

    private Material _mat;
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();
        if (targetRenderer == null) return;

        _mat = new Material(targetRenderer.sharedMaterial);
        targetRenderer.material = _mat;
        if (_mat.HasProperty(EmissionColorId))
            _mat.EnableKeyword("_EMISSION");
    }

    private void Update()
    {
        if (_mat == null) return;

        float h = (Time.time * hueSpeed) % 1f;
        Color rgb = Color.HSVToRGB(h, 0.88f, 0.95f);
        if (_mat.HasProperty(BaseColorId))
            _mat.SetColor(BaseColorId, Color.Lerp(new Color(0.03f, 0.03f, 0.06f), rgb, 0.55f));
        if (_mat.HasProperty(EmissionColorId))
            _mat.SetColor(EmissionColorId, rgb * emissionMul);
    }
}
