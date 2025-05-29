using UnityEngine;
using System.Collections;

public class LifeController : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public bool enableInvulnerability = true;
    public bool enablePizzaThrowing = true;
    public bool enableGameOverCheck = true;

    [Header("Configurações de Invulnerabilidade")]
    [SerializeField] private float invulnerabilityDuration = 2f;
    private bool isInvulnerable = false;

    [Header("Referências")]
    public Transform pizzaGhost;
    [SerializeField] private GameObject deathScreen;

    private Transform[] pizzaBoxes;
    private Vector3[] originalPositions;
    private bool isGameOver = false;
    private bool isPlayerDead = false;
    private bool pizzasEntregues = false;


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

        if (pizzaGhost != null && !pizzaGhost.CompareTag("PizzaBox"))
        {
            pizzaGhost.SetParent(transform);
            pizzaGhost.localPosition = Vector3.zero;
            pizzaGhost.gameObject.SetActive(false);
        }

        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    void Update()
    {
        if (enableGameOverCheck && !isGameOver)
        {
            CheckGameOver();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!enableInvulnerability || isInvulnerable) return;

        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("BulletEnemy"))
        {
            if (enablePizzaThrowing)
            {
                ThrowPizzaBox();
            }

            StartCoroutine(InvulnerabilityCoroutine(invulnerabilityDuration));
        }
    }

    void ThrowPizzaBox()
    {
        if (!enablePizzaThrowing) return;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("PizzaBox") && child != pizzaGhost)
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                child.SetParent(null);

                if (enableGameOverCheck)
                {
                    CheckGameOver(); // Verifica imediatamente após perder pizza
                }

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

        if (pizzaGhost != null && transform.childCount == 0)
        {
            pizzaGhost.gameObject.SetActive(true);
        }
    }

    public void ActivateInvulnerability(float duration)
    {
        if (!enableInvulnerability) return;
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
        if (isGameOver || pizzasEntregues) return;

        GameObject[] remainingBoxes = GameObject.FindGameObjectsWithTag("PizzaBox");
        bool allBoxesDestroyed = true;

        foreach (GameObject box in remainingBoxes)
        {
            if (box != null && box != pizzaGhost?.gameObject)
            {
                allBoxesDestroyed = false;
                break;
            }
        }


        if (allBoxesDestroyed || isPlayerDead)
        {
            isGameOver = true;
            StartCoroutine(GameOverRoutine());
        }
    }

    public void MarkPizzaDelivered()
    {
        pizzasEntregues = true;
        Debug.Log("Pizzas entregues! Game Over desativado.");
    }


    public void Die()
    {
        if (isPlayerDead) return;

        isPlayerDead = true;

        // Desativa controles, mas não destrói imediatamente o jogador
        GetComponent<PlayerController1>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        CheckGameOver(); // Chama game over imediatamente
    }

    IEnumerator GameOverRoutine()
    {
        Debug.Log("Game Over! Exibindo tela de morte em 2 segundos...");
        yield return new WaitForSeconds(2f);
        ShowDeathScreen();
    }

    void ShowDeathScreen()
    {
        Debug.Log("Iniciando transição para tela de Game Over.");

        ScreenTransition transition = FindFirstObjectByType<ScreenTransition>();
        if (transition != null)
        {
            transition.StartTransition(() =>
            {
                if (deathScreen != null)
                    deathScreen.SetActive(true);
            });
        }
        else
        {
            Debug.LogWarning("Nenhum ScreenTransition encontrado na cena!");
            if (deathScreen != null)
                deathScreen.SetActive(true);
        }
    }

    public int GetPizzaCount()
    {
        int count = 0;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("PizzaBox") && child != pizzaGhost)
            {
                count++;
            }
        }
        return count;
    }

}

