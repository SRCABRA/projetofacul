using UnityEngine;

public class ColetavelFlutuante : MonoBehaviour
{
    public float alturaFlutuacao = 0.5f; // Altura da flutuação
    public float velocidadeFlutuacao = 2f; // Velocidade do efeito de flutuação
    public float velocidadeRotacao = 50f; // Velocidade de rotação

    private bool ativado = false;
    private Vector3 posicaoInicial;

    void Start()
    {
        posicaoInicial = transform.position;
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
            posicaoInicial = transform.position;
        }
    }

}
