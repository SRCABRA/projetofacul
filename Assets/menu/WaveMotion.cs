using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WaveMotion : MonoBehaviour
{
    public float amplitude = 10f;
    public float frequency = 2f;
    public float speed = 5f;

    private TextMeshProUGUI tmpText;
    private Mesh mesh;
    private Vector3[] vertices;

    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        tmpText.ForceMeshUpdate(); // Garante que a malha está atualizada
        mesh = tmpText.mesh;
        vertices = mesh.vertices;

        int characterCount = tmpText.textInfo.characterCount;

        for (int i = 0; i < characterCount; i++)
        {
            var charInfo = tmpText.textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int vertexIndex = charInfo.vertexIndex;
            Vector3 offset = new Vector3(
                0f,
                Mathf.Sin(Time.time * frequency + i * speed) * amplitude,
                0f
            );

            for (int j = 0; j < 4; j++)
            {
                vertices[vertexIndex + j] += offset;
            }
        }

        mesh.vertices = vertices;
        tmpText.canvasRenderer.SetMesh(mesh);
    }
}
