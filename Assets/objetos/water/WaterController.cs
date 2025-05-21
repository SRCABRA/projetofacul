using UnityEngine;

public class WaterController : MonoBehaviour
{
    [Header("Configurações de velocidade")]
    public float normalSpeed = 0.01f;
    public float accelerationCurve = 0.000001f;

    [Header("Configuração da subida suave")]
    public float smoothTime = 0.5f;

    [Header("Oscilação da água")]
    public float bobbingAmplitude = 0.05f;
    public float bobbingFrequency = 1f;
    public float rotationAmplitude = 0.5f;
    public float rotationFrequency = 0.5f;

    [Header("Delay antes da subida")]
    public float startDelay = 5f; // Tempo (em segundos) que a água espera antes de começar a subir

    private float currentSpeed;
    private bool risingFast = false;
    private float fastTargetY;
    private float velocity = 0f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float waterTime = 0f;

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
        if (Time.timeScale < 0.01f) return;

        float dt = Mathf.Max(Time.deltaTime, 0.02f);
        waterTime += dt;

        Vector3 position = transform.position;

        // Espera antes de começar a subir
        if (waterTime < startDelay)
        {
            ApplyWaterBobbing(); // Pode manter a oscilação mesmo parada
            return;
        }

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
            float heightOffset = currentSpeed * dt + 0.5f * accelerationCurve * dt * dt;
            position.y += heightOffset;
            transform.position = position;

            currentSpeed += accelerationCurve * dt;
        }

        ApplyWaterBobbing();
    }

    private void ApplyWaterBobbing()
    {
        float bobbingOffset = Mathf.Sin(waterTime * bobbingFrequency) * bobbingAmplitude;
        float rotationOffset = Mathf.Sin(waterTime * rotationFrequency) * rotationAmplitude;

        Vector3 pos = transform.position;
        pos.y += bobbingOffset;
        transform.position = pos;

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
