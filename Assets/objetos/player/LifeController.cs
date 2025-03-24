using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Para reiniciar a cena

public class LifeController : MonoBehaviour
{
    public float invulnerabilityDuration = 2f;
    private bool isInvulnerable = false;
    private Transform[] pizzaBoxes;
    private Vector3[] originalPositions;
    private bool isRestarting = false; // Evita múltiplas chamadas de reinício
    public Transform pizzaGhost; // Referência à pizza fantasma

    void Start()
    {
        pizzaBoxes = GetComponentsInChildren<Transform>();
        originalPositions = new Vector3[pizzaBoxes.Length];

        for (int i = 0; i < pizzaBoxes.Length; i++)
        {
            if (pizzaBoxes[i].CompareTag("PizzaBox"))
            {
                originalPositions[i] = pizzaBoxes[i].localPosition;
            }
        }

        // Garante que a pizza fantasma nunca seja removida
        if (pizzaGhost != null && !pizzaGhost.CompareTag("PizzaBox"))
        {
            pizzaGhost.SetParent(transform);
            pizzaGhost.localPosition = Vector3.zero; // Mantém no lugar
            pizzaGhost.gameObject.SetActive(false); // Invisível
        }
    }

    void Update()
    {
        CheckGameOver(); // Verifica constantemente se todas as caixas foram destruídas
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
            if (child.CompareTag("PizzaBox") && child != pizzaGhost) // Não joga a pizza fantasma
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                child.SetParent(null);
                return;
            }
        }

        Debug.Log("Nenhuma pizza disponível para jogar!");
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
                return;
            }
        }

        // Se todas as pizzas foram perdidas, ativa a pizza fantasma
        if (pizzaGhost != null && transform.childCount == 0)
        {
            pizzaGhost.gameObject.SetActive(true);
        }
    }

    public void ActivateInvulnerability(float duration)
    {
        StartCoroutine(InvulnerabilityCoroutine(duration));
    }

    IEnumerator InvulnerabilityCoroutine(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }

    void CheckGameOver()
    {
        // Verifica se todas as caixas de pizza foram destruídas, ignorando a pizza fantasma
        GameObject[] remainingBoxes = GameObject.FindGameObjectsWithTag("PizzaBox");
        bool allBoxesDestroyed = remainingBoxes.Length == 0 || (remainingBoxes.Length == 1 && remainingBoxes[0] == pizzaGhost.gameObject);

        // Verifica se o jogador ainda existe
        bool playerDestroyed = GameObject.FindGameObjectWithTag("Player") == null;

        if ((allBoxesDestroyed || playerDestroyed) && !isRestarting)
        {
            isRestarting = true;
            StartCoroutine(RestartGame());
        }
    }

    IEnumerator RestartGame()
    {
        Debug.Log("Fim de jogo! Reiniciando em 2 segundos...");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
