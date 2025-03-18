using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LifeController : MonoBehaviour
{
    public float invulnerabilityDuration = 2f; // Tempo de invulnerabilidade
    private bool isInvulnerable = false; // Controle de invulnerabilidade

    private List<Transform> pizzaBoxes = new List<Transform>(); // Todas as pizzas coletáveis
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>(); // Posições originais das pizzas

    void Start()
    {
        FindAllPizzaBoxes();
    }

    void FindAllPizzaBoxes()
    {
        // Procura todas as caixas de pizza no jogo
        GameObject[] allPizzas = GameObject.FindGameObjectsWithTag("PizzaBox");

        foreach (GameObject pizza in allPizzas)
        {
            Transform pizzaTransform = pizza.transform;

            if (!pizzaBoxes.Contains(pizzaTransform))
            {
                pizzaBoxes.Add(pizzaTransform);
                if (!originalPositions.ContainsKey(pizzaTransform))
                {
                    originalPositions[pizzaTransform] = pizzaTransform.localPosition;
                }
            }
        }

        Debug.Log("Total de pizzas registradas: " + pizzaBoxes.Count);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvulnerable)
        {
            ThrowPizzaBox();
            StartCoroutine(InvulnerabilityCoroutine(invulnerabilityDuration));
        }
    }

    void ThrowPizzaBox()
    {
        if (pizzaBoxes.Count > 0)
        {
            Transform pizza = pizzaBoxes[0]; // Pega a primeira pizza da lista
            pizzaBoxes.RemoveAt(0); // Remove da lista de pizzas nas costas

            Rigidbody rb = pizza.gameObject.AddComponent<Rigidbody>();
            rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            pizza.SetParent(null); // Solta a pizza no mundo

            Debug.Log("Pizza perdida! Restam: " + pizzaBoxes.Count);
        }
        else
        {
            Debug.Log("Não há mais pizzas para perder!");
        }
    }

public void RestorePizzaBox(Transform pizzaBox)
{
    // Garante que a pizza seja adicionada de volta à lista
    if (!pizzaBoxes.Contains(pizzaBox))
    {
        pizzaBoxes.Add(pizzaBox);
    }

    // Se a pizza não estava registrada, adicionamos ela com uma posição padrão
    if (!originalPositions.ContainsKey(pizzaBox))
    {
        originalPositions[pizzaBox] = new Vector3(0, 1, 0); // Posição padrão para evitar erro
    }

    // Faz a pizza voltar para as costas do jogador
    pizzaBox.SetParent(transform);

    // Ajusta a posição relativa correta
    if (originalPositions.ContainsKey(pizzaBox))
    {
        pizzaBox.localPosition = originalPositions[pizzaBox];
    }
    else
    {
        pizzaBox.localPosition = new Vector3(0, 1, 0); // Caso não tenha posição salva
    }

    // Remove a física para impedir comportamento inesperado
    Rigidbody rb = pizzaBox.GetComponent<Rigidbody>();
    if (rb != null)
    {
        Destroy(rb);
    }
}

    public void ActivateInvulnerability(float duration)
    {
        StartCoroutine(InvulnerabilityCoroutine(duration));
    }

    IEnumerator InvulnerabilityCoroutine(float duration)
    {
        isInvulnerable = true;
        Debug.Log("Jogador está invulnerável!");
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
        Debug.Log("Invulnerabilidade acabou!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PizzaBox"))
        {
            RestorePizzaBox(other.transform);
        }
    }
}
