using UnityEngine;

public class DestroyAfterEffect : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        if (ps == null)
        {
            Debug.LogError("Nenhum ParticleSystem encontrado!", this);
            return;
        }

        // Garante que não fique em loop
        var mainModule = ps.main;
        mainModule.loop = false;

        // Inicia o efeito se necessário
        if (!ps.isPlaying)
            ps.Play();
    }

    void Update()
    {
        // Espera até que o efeito esteja realmente morto antes de destruir
        if (ps != null && !ps.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }
}
