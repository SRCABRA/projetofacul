using UnityEngine;

public class portaDaMansao : MonoBehaviour
{
    [SerializeField] private PizzaDeliveryPoint pizzaDeliveryPoint;
    [SerializeField] private float alturaFinal = 10f;
    [SerializeField] private float velocidade = 2f;

    private Vector3 posicaoInicial;
    private Vector3 posicaoAlvo;
    private bool subindo = false;

    void Start()
    {
        posicaoInicial = transform.position;
        posicaoAlvo = posicaoInicial + new Vector3(0, alturaFinal, 0);
    }

    void Update()
    {
        if (pizzaDeliveryPoint.JaEntregou && !subindo)
        {
            subindo = true;
        }

        if (subindo)
        {
            transform.position = Vector3.Lerp(transform.position, posicaoAlvo, velocidade * Time.deltaTime);

            // Para evitar que a porta nunca chegue exatamente no ponto:
            if (Vector3.Distance(transform.position, posicaoAlvo) < 0.01f)
            {
                transform.position = posicaoAlvo;
                subindo = false;
            }
        }
    }
}
