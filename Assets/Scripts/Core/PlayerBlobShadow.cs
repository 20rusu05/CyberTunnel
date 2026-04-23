using UnityEngine;

/// <summary>
/// Simple flat shadow under the player (Minecraft-style), raycast to ground.
/// </summary>
public class PlayerBlobShadow : MonoBehaviour
{
    [SerializeField] private float rayHeight = 2f;
    [SerializeField] private float rayDistance = 30f;
    [SerializeField] private float yOffset = 0.04f;
    [SerializeField] private Vector3 baseScale = new Vector3(1.15f, 1f, 0.55f);

    private Transform _blob;
    private Renderer _blobRenderer;

    private void Awake()
    {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "BlobShadow";
        quad.transform.SetParent(transform, false);
        Object.Destroy(quad.GetComponent<Collider>());

        _blob = quad.transform;
        // Default Unity quad: mesh in XY plane, mesh normal +Z — keep identity so we can align +Z to ground normal.
        _blob.localRotation = Quaternion.identity;
        _blob.localScale = new Vector3(baseScale.x, baseScale.y, 1f);

        _blobRenderer = quad.GetComponent<Renderer>();
        Shader sh = Shader.Find("Universal Render Pipeline/Unlit");
        if (sh == null) sh = Shader.Find("Unlit/Transparent");
        var mat = new Material(sh);
        mat.color = new Color(0f, 0f, 0f, 0.45f);
        if (mat.HasProperty("_Surface"))
            mat.SetFloat("_Surface", 1f);
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        if (mat.HasProperty("_Cull"))
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        _blobRenderer.material = mat;
        _blobRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _blobRenderer.receiveShadows = false;
    }

    private void LateUpdate()
    {
        if (_blob == null) return;

        Vector3 origin = transform.position + Vector3.up * rayHeight;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            Vector3 up = hit.normal;
            if (up.sqrMagnitude < 1e-6f) up = Vector3.up;
            up.Normalize();

            // Align quad mesh normal (+Z) with surface normal — flat decal. (LookRotation(hit.normal) is
            // degenerate when normal ≈ world up and turns the quad into a vertical slab.)
            Quaternion align = Quaternion.FromToRotation(Vector3.forward, up);

            Vector3 planarFwd = Vector3.ProjectOnPlane(transform.forward, up);
            if (planarFwd.sqrMagnitude < 1e-4f)
                planarFwd = Vector3.ProjectOnPlane(transform.right, up);
            if (planarFwd.sqrMagnitude < 1e-4f)
                planarFwd = Vector3.ProjectOnPlane(Vector3.forward, up);
            planarFwd.Normalize();

            Vector3 worldQuadRight = align * Vector3.right;
            float yawDeg = Mathf.Atan2(
                Vector3.Dot(up, Vector3.Cross(worldQuadRight, planarFwd)),
                Vector3.Dot(worldQuadRight, planarFwd)) * Mathf.Rad2Deg;
            Quaternion yaw = Quaternion.AngleAxis(yawDeg, up);

            _blob.SetPositionAndRotation(hit.point + up * yOffset, yaw * align);

            float d = Mathf.Clamp01(hit.distance / 4f);
            float s = 1f + d * 0.35f;
            _blob.localScale = new Vector3(baseScale.x * s, baseScale.y * s, 1f);
        }
        else
        {
            _blob.localPosition = new Vector3(0f, 0.02f, 0f);
            _blob.localRotation = Quaternion.Euler(90f, 0f, 0f);
            _blob.localScale = baseScale;
        }
    }
}
