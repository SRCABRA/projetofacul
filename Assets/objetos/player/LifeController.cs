using UnityEngine;
using System.Collections;

public class LifeController : MonoBehaviour
{
    public int pizzabox = 3;
    public GameObject Player;
    public GameObject newObjectPrefab; // Prefab do novo objeto a ser instanciado
    private bool isInvulnerable = false;
    public float invulnerabilityDuration = 1.0f; // Duração da invulnerabilidade em segundos

    void Start()
    {
        // Inicialização, se necessário
    }

    void Update()
    {
        // Atualização por frame, se necessário
    }

    public void TakeDamage()
    {
        if (pizzabox > 0 && !isInvulnerable)
        {
            // Decrementa o número de caixas de pizza
            pizzabox--;

            // Encontra todas as caixas de pizza
            foreach (Transform child in Player.transform)
            {
                if (child.CompareTag("PizzaBox"))
                {
                    // Deleta a caixa de pizza
                    Destroy(child.gameObject);
                    break;
                }
            }

            // Instancia um novo objeto
            Instantiate(newObjectPrefab, Player.transform.position, Quaternion.identity);

            // Ativa a invulnerabilidade
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }
}