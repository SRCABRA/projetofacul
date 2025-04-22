using UnityEngine;

public class WaterController : MonoBehaviour
{
    [Header("Configurações de velocidade")]
    public float normalSpeed = 0.01f;
    public float accelerationCurve = 0.000001f;

    [Header("Configuração da subida suave")]
    public float smoothTime = 0.5f; // Tempo para suavizar subida rápida

    [Header("Oscilação da água")]
    public float bobbingAmplitude = 0.05f;     // Altura da oscilação vertical
    public float bobbingFrequency = 1f;        // Velocidade da oscilação
    public float rotationAmplitude = 0.5f;     // Grau da rotação suave
    public float rotationFrequency = 0.5f;     // Velocidade da rotação

    private float currentSpeed;
    private bool risingFast = false;
    private float fastTargetY;
    private float velocity = 0f; // usado pelo SmoothDamp

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float waterTime = 0f; // tempo customizado que respeita o pause

    void Start()
    {
        currentSpeed = normalSpeed;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Impede movimentação com o jogo pausado ou praticamente pausado
        if (Time.timeScale < 0.01f) return;

        // Garante um deltaTime mínimo para evitar bugs em slow motion
        float dt = Mathf.Max(Time.deltaTime, 0.02f);

        waterTime += dt;

        Vector3 position = transform.position;

        if (risingFast)
        {
            float newY = Mathf.SmoothDamp(position.y, fastTargetY, ref velocity, smoothTime, Mathf.Infinity, dt);

            if (Mathf.Abs(newY - fastTargetY) < 0.01f)
            {
                newY = fastTargetY;
                risingFast = false;
                currentSpeed = normalSpeed;
                velocity = 0f;
            }

            position.y = newY;
            transform.position = position;
        }
        else
        {
            position += new Vector3(0f, currentSpeed * dt, 0f);
            transform.position = position;
            currentSpeed += accelerationCurve * dt;
        }

        ApplyWaterBobbing();
    }

    private void ApplyWaterBobbing()
    {
        // Usa o tempo controlado manualmente
        float bobbingOffset = Mathf.Sin(waterTime * bobbingFrequency) * bobbingAmplitude;
        float rotationOffset = Mathf.Sin(waterTime * rotationFrequency) * rotationAmplitude;

        // Aplica a oscilação na posição Y
        Vector3 pos = transform.position;
        pos.y += bobbingOffset;
        transform.position = pos;

        // Aplica rotação suave no eixo Z (parece com "balanço")
        Vector3 rot = initialRotation.eulerAngles;
        rot.z += rotationOffset;
        transform.rotation = Quaternion.Euler(rot);
    }

    public void StartFastRise(float targetY, float fastSpeed)
    {
        fastTargetY = targetY;
        currentSpeed = fastSpeed;
        risingFast = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
