using UnityEngine;

public class DustBall : MonoBehaviour
{
    [SerializeField] private bool startBig = true; // Alternar entre crescer ou diminuir
    [SerializeField] private float riseSpeed = 0.5f; // Velocidade de subida

    private float lifetime = 0.5f; // Tempo até desaparecer
    private Vector3 sizeChangeRate;

    void Start()
    {
        float minSize = 0.2f;
        float maxSize = 0.9f;

        if (startBig)
        {
            // Começa grande e diminui
            float randomSize = Random.Range(0.7f, maxSize);
            transform.localScale = Vector3.one * randomSize;
            sizeChangeRate = transform.localScale / lifetime;
        }
        else
        {
            // Começa pequeno e cresce
            float randomSize = Random.Range(minSize, 0.7f);
            transform.localScale = Vector3.one * randomSize;
            sizeChangeRate = (Vector3.one * maxSize - transform.localScale) / lifetime;
        }
    }

    void Update()
    {
        // Faz a poeira subir aos poucos
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        if (startBig)
        {
            // Diminui de tamanho
            transform.localScale -= sizeChangeRate * Time.deltaTime;

            if (transform.localScale.x <= 0.01f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Aumenta de tamanho
            transform.localScale += sizeChangeRate * Time.deltaTime;

            if (transform.localScale.x >= 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
