using UnityEngine;

public class DeathEffectSpawner : MonoBehaviour
{
    public GameObject deathEffectPrefab; // Prefab do efeito de morte/explosão

    void OnDestroy()
    {
        // Verifica se há um prefab de efeito configurado
        if (deathEffectPrefab != null)
        {
            // Instancia o efeito na posição do objeto destruído
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
