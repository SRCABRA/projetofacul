using UnityEngine;
using System.Collections;

public class LifeController : MonoBehaviour
{
    public float invulnerabilityDuration = 2f; // Tempo padrão de invulnerabilidade
    private bool isInvulnerable = false; // Flag para controlar invulnerabilidade
    private Transform[] pizzaBoxes;
    private Vector3[] originalPositions;

    void Start()
    {
        // Armazena as posições originais das caixas de pizza
        pizzaBoxes = GetComponentsInChildren<Transform>();
        originalPositions = new Vector3[pizzaBoxes.Length];
        for (int i = 0; i < pizzaBoxes.Length; i++)
        {
            if (pizzaBoxes[i].CompareTag("PizzaBox"))
            {
                originalPositions[i] = pizzaBoxes[i].localPosition;
            }
        }
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
        foreach (Transform child in transform)
        {
            if (child.CompareTag("PizzaBox"))
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
                rb.constraints = RigidbodyConstraints.FreezeRotation; // Congela a rotação
                child.SetParent(null);
                break; // Sai do loop após encontrar e lançar uma caixa de pizza
            }
        }
    }

    public void RestorePizzaBox(Transform pizzaBox)
    {
        for (int i = 0; i < pizzaBoxes.Length; i++)
        {
            if (pizzaBoxes[i] == pizzaBox)
            {
                pizzaBox.SetParent(transform);
                pizzaBox.localPosition = originalPositions[i];
                Destroy(pizzaBox.GetComponent<Rigidbody>());
                break;
            }
        }
    }

    // Método para ativar a invulnerabilidade externamente (usado pelo PlayerBuffs)
    public void ActivateInvulnerability(float duration)
    {
        StartCoroutine(InvulnerabilityCoroutine(duration));
    }

    // Coroutine que lida com a invulnerabilidade
    IEnumerator InvulnerabilityCoroutine(float duration)
    {
        isInvulnerable = true;
        Debug.Log("Jogador está invulnerável!");
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
        Debug.Log("Invulnerabilidade acabou!");
    }
}
