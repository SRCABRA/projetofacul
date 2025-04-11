using UnityEngine;

public class ActivateWaterTrigger : MonoBehaviour
{
    [Header("Referência da Água")]
    public WaterController waterController;

    [Header("Altura-Alvo da Água ao Ativar")]
    public float waterTargetHeight = 10f;

    [Header("Velocidade rápida da água")]
    public float fastRiseSpeed = 0.5f;

    private bool alreadyActivated = false;

    void OnTriggerEnter(Collider other)
    {
        if (alreadyActivated) return;

        if (other.CompareTag("Player"))
        {
            alreadyActivated = true;

            if (waterController != null)
            {
                float finalTargetY = transform.position.y - waterTargetHeight;
                waterController.StartFastRise(finalTargetY, fastRiseSpeed);
            }
        }
    }
}
