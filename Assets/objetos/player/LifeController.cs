using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Necessário para reiniciar a cena

public class LifeController : MonoBehaviour
{
    public float invulnerabilityDuration = 2f;
    private bool isInvulnerable = false;
    private Transform[] pizzaBoxes;
    private Vector3[] originalPositions;
    private bool isRestarting = false; // Evita múltiplas chamadas de reinício

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
    }

    void Update()
    {
        CheckGameOver(); // Verifica constantemente se todas as caixas foram destruídas ou o jogador foi eliminado
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
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                child.SetParent(null);
                break;
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
        // Verifica se todas as caixas de pizza foram destruídas
        GameObject[] remainingBoxes = GameObject.FindGameObjectsWithTag("PizzaBox");
        bool allBoxesDestroyed = remainingBoxes.Length == 0;

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
