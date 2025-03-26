using UnityEngine;

public class DustBall : MonoBehaviour
{
    [SerializeField] private bool startBig = true; // Alternar entre crescer ou diminuir
    [SerializeField] private float riseSpeed = 0.9f; // Velocidade de subida

    private float lifetime = 0.5f; // Tempo até desaparecer
    private Vector3 sizeChangeRate;

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
