using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    [Header("Referência ao controlador da água")]
    [SerializeField] private WaterController waterController;

    [Header("Configurações de subida rápida (opcional)")]
    public bool usarSubidaRapida = false;
    public float alturaAlvo = 20f;
    public float velocidadeRapida = 0.1f;

    private bool ativado = false;

    void OnTriggerEnter(Collider other)
    {
        if (ativado) return;

        if (other.CompareTag("Player"))
        {
            ativado = true;

            if (waterController != null)
            {
                if (usarSubidaRapida)
                {
                    waterController.StartFastRise(alturaAlvo, velocidadeRapida);
                }
                else
                {
                    // Apenas inicia o tempo interno do WaterController para ele subir normalmente
                    waterController.enabled = true;
                }
            }

            // Opcional: desativa o trigger para evitar reuso
            gameObject.SetActive(false);
        }
    }
}
