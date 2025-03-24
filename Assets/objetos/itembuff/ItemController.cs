using UnityEngine;

public class ItemController : MonoBehaviour
{
    public float alturaFlutuacao = 0.5f; // Altura da flutuação acima do chão
    public float velocidadeFlutuacao = 2f; // Velocidade da flutuação
    public float velocidadeRotacao = 50f; // Velocidade de rotação

    private bool ativado = false;
    private Vector3 posicaoInicial;

    void Start()
    {
        // Se o objeto já estiver no chão ao iniciar, ajusta a altura correta
        AjustarAlturaAcimaDoChao();
    }

    void Update()
    {
        if (ativado)
        {
            // Movimento de flutuação (senoidal para suavidade)
            float novaAltura = Mathf.Sin(Time.time * velocidadeFlutuacao) * alturaFlutuacao;
            transform.position = posicaoInicial + new Vector3(0, novaAltura, 0);

            // Rotação contínua
            transform.Rotate(Vector3.up * velocidadeRotacao * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("chao") && !ativado)
        {
            ativado = true;
            AjustarAlturaAcimaDoChao();
        }
    }

    void AjustarAlturaAcimaDoChao()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            // Garante que o objeto fique na altura correta acima do chão
            transform.position = hit.point + new Vector3(0, alturaFlutuacao, 0);
            posicaoInicial = transform.position;
        }
    }
}
