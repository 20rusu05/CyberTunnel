using UnityEngine;

/// <summary>
/// One dancefloor square: own hue phase so each tile cycles a different colour (disco grid).
/// </summary>
public class DiscoTile : MonoBehaviour
{
    [SerializeField] private float huePhase;
    [SerializeField] private float hueSpeed = 0.42f;

    private Renderer _rend;
    private MaterialPropertyBlock _mpb;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        _rend = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    public void Configure(int gridX, int gridZ)
    {
        huePhase = (gridX * 0.173f + gridZ * 0.271f + gridX * gridZ * 0.019f) % 1f;
    }

    private void Update()
    {
        if (_rend == null) return;

        float h = (Time.time * hueSpeed + huePhase) % 1f;
        Color rgb = Color.HSVToRGB(h, 0.92f, 1f);
        Color baseCol = Color.Lerp(new Color(0.04f, 0.04f, 0.07f), rgb, 0.45f);

        _rend.GetPropertyBlock(_mpb);
        if (_rend.sharedMaterial != null && _rend.sharedMaterial.HasProperty(BaseColorId))
            _mpb.SetColor(BaseColorId, baseCol);
        if (_rend.sharedMaterial != null && _rend.sharedMaterial.HasProperty(EmissionColorId))
            _mpb.SetColor(EmissionColorId, rgb * 1.9f);
        _rend.SetPropertyBlock(_mpb);
    }
}
