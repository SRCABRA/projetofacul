using UnityEngine;

public class DustBall : MonoBehaviour
{
    private float lifetime = 0.5f; // Tempo até desaparecer
    private Vector3 shrinkRate;

    void Start()
    {
        // Define um tamanho aleatório e velocidade de encolhimento
        float randomSize = Random.Range(0.9f, 1.2f);
        transform.localScale = Vector3.one * randomSize;
        shrinkRate = transform.localScale / lifetime;
    }

    void Update()
    {
        // Diminui de tamanho suavemente
        transform.localScale -= shrinkRate * Time.deltaTime;

        // Destrói quando estiver pequeno
        if (transform.localScale.x <= 0.01f)
        {
            Destroy(gameObject);
        }
    }
}
