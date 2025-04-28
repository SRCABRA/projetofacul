using UnityEngine;
using System.Collections.Generic;

public class CameraOcclusionFadeDebug : MonoBehaviour
{
    public Transform player;
    public LayerMask obstacleLayers;
    public float fadeStartDistance = 3f;
    public float fadeSpeed = 2f;

    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private List<Renderer> fadingRenderers = new List<Renderer>();

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player não atribuído no CameraOcclusionFade!");
            return;
        }

        // Restaurar objetos
        foreach (Renderer rend in fadingRenderers)
        {
            if (rend != null)
                UpdateFade(rend, 1f);
        }
        fadingRenderers.Clear();

        Vector3 directionToCamera = (transform.position - player.position).normalized;
        float distanceToCamera = Vector3.Distance(player.position, transform.position);

        Debug.DrawLine(player.position, transform.position, Color.red);

        RaycastHit[] hits = Physics.RaycastAll(player.position, directionToCamera, distanceToCamera, obstacleLayers);

        if (hits.Length > 0)
        {
            Debug.Log("Raycast bateu em " + hits.Length + " objetos.");
        }
        else
        {
            Debug.Log("Raycast NÃO bateu em nada.");
        }

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                if (!originalMaterials.ContainsKey(rend))
                {
                    originalMaterials[rend] = rend.materials;
                    MakeMaterialFade(rend);
                }
                fadingRenderers.Add(rend);

                float dist = Vector3.Distance(transform.position, hit.point);
                float t = Mathf.InverseLerp(fadeStartDistance, 0f, dist);
                UpdateFade(rend, 1f - t);
            }
        }
    }

void MakeMaterialFade(Renderer rend)
{
    foreach (Material mat in rend.materials)
    {
        if (mat.shader.name.Contains("Universal Render Pipeline/Lit"))
        {
            mat.SetFloat("_Surface", 1f); // 1 = Transparent
            mat.SetFloat("_Blend", 0f); // Alpha blending
            mat.SetFloat("_AlphaClip", 0f); // Desliga alpha clipping
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }
        else if (mat.shader.name.Contains("Standard"))
        {
            mat.SetFloat("_Mode", 2f); // Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
    }
}

    void UpdateFade(Renderer rend, float targetAlpha)
    {
        foreach (Material mat in rend.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color color = mat.color;
                color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
                mat.color = color;
            }
        }
    }
}
