using UnityEngine;

public class ShadowProjector : MonoBehaviour
{
    public Transform player;
    public LayerMask groundLayer;
    public float shadowHeightOffset = 0.05f;

    void Update()
    {
        Ray ray = new Ray(player.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            transform.position = hit.point + Vector3.up * shadowHeightOffset;
            transform.rotation = Quaternion.LookRotation(hit.normal); // Acompanha a inclinação
        }
    }
}
