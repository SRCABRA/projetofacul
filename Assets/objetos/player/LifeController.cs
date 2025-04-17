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
    [SerializeField] private Transform pizzaGhost;
    [SerializeField] private GameObject deathScreen; // Painel da tela de morte

    private Transform[] pizzaBoxes;
    private Vector3[] originalPositions;
    private bool isGameOver = false; // Evita múltiplas execuções do Game Over

    

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

        // Desativa a tela de morte no início do jogo
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }
    }

    void Update()
    {
        if (enableGameOverCheck) CheckGameOver();
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (enableInvulnerability && collision.gameObject.CompareTag("Enemy") && !isInvulnerable)
        {
            if (enablePizzaThrowing) ThrowPizzaBox();
            StartCoroutine(InvulnerabilityCoroutine(invulnerabilityDuration));
        }
        if (enableInvulnerability && collision.gameObject.CompareTag("BulletEnemy") && !isInvulnerable)
        {
            if (enablePizzaThrowing) ThrowPizzaBox();
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
        if (isGameOver) return;

        GameObject[] remainingBoxes = GameObject.FindGameObjectsWithTag("PizzaBox");
        bool allBoxesDestroyed = remainingBoxes.Length == 0 || (remainingBoxes.Length == 1 && remainingBoxes[0] == pizzaGhost.gameObject);
        bool playerDestroyed = GameObject.FindGameObjectWithTag("Player") == null;

        if (allBoxesDestroyed || playerDestroyed)
        {
            isGameOver = true;
            StartCoroutine(GameOverRoutine()); // Espera 2 segundos antes de ativar a tela
        }
    }

    IEnumerator GameOverRoutine()
    {
        Debug.Log("Game Over! Exibindo tela de morte em 2 segundos...");
        yield return new WaitForSeconds(2f); // Aguarda 2 segundos

        ShowDeathScreen(); // Agora a tela aparece depois da espera
    }

    void ShowDeathScreen()
    {
        Debug.Log("Exibindo tela de Game Over.");
        if (deathScreen != null)
        {
            deathScreen.SetActive(true); // Ativa a tela de morte
        }
    }
}
