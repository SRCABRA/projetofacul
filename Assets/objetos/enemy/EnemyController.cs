using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player; // Alvo do inimigo
    public Transform target; // Alvo do inimigo
    public float life = 10f; // Vida do inimigo
    public float speed = 5f; // Velocidade do inimigo
    public float viewDistance = 40f; // Distância máxima para identificar um alvo

    public float danobulletbase = 4f; // Dano base do tiro
    public float danoAreaAttack = 6f;  // Dano do ataque de área
    public GameObject pizzaBoxPrefab; // Prefab da caixa de pizza
    public GameObject consumable1Prefab; // Prefab do consumível 1
    public GameObject consumable2Prefab; // Prefab do consumível 2

    public float consumable1DropChance = 0.3f; // Chance de drop do consumível 1
    public float consumable2DropChance = 0.3f; // Chance de drop do consumível 2

    private GameObject carriedPizzaBox; // Referência da caixa de pizza carregada
    private Rigidbody rb; // Referência para o Rigidbody
    private Renderer enemyRenderer; // Referência para o Renderer
    private Color originalColor; // Cor original do inimigo
    private bool isJumping = false; // Para evitar efeitos repetidos

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Obtém o Rigidbody do inimigo
        enemyRenderer = GetComponent<Renderer>(); // Obtém o Renderer para mudar a cor
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color; // Salva a cor original
        }
        ChooseTarget();
    }

    void Update()
    {
        ChooseTarget();
        MoveTowardsTarget();

        if (life <= 0)
        {
            Drop();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AreaAttack") && !isJumping)
        {
            isJumping = true; // Evita múltiplos impulsos

            life -= danoAreaAttack;

            // Se o Rigidbody existir, adiciona um impulso para cima
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Reseta a velocidade vertical
                rb.AddForce(Vector3.up * 10f, ForceMode.VelocityChange); // Ajuste a força conforme necessário
            }

            // Pisca vermelho
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = Color.red;
            }

            // Restaura a cor após 0.2 segundos
            Invoke(nameof(ResetColor), 0.2f);
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            life -= danobulletbase;
        }
        else if (collision.gameObject.CompareTag("PizzaBox") && collision.transform.parent != player)
        {
            Rigidbody pizzaBoxRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (pizzaBoxRigidbody != null && Mathf.Abs(pizzaBoxRigidbody.linearVelocity.x) < 0.01f)
            {
                PickUpPizzaBox(collision.gameObject);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("AreaAttack"))
        {
            isJumping = false; // Permite outro impulso ao sair da colisão
        }
    }

    void ResetColor()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }
    }

    void PickUpPizzaBox(GameObject pizzaBox)
    {
        carriedPizzaBox = pizzaBox;
        carriedPizzaBox.transform.SetParent(transform);
        carriedPizzaBox.transform.localPosition = new Vector3(0, 1, 0); // Ajuste a posição conforme necessário
        carriedPizzaBox.transform.localRotation = Quaternion.identity; // Reseta a rotação
    }

    void Drop()
    {
        if (carriedPizzaBox != null)
        {
            carriedPizzaBox.transform.SetParent(null);
            Rigidbody pizzaBoxRigidbody = carriedPizzaBox.GetComponent<Rigidbody>();
            if (pizzaBoxRigidbody != null)
            {
                pizzaBoxRigidbody.linearVelocity = Vector3.zero; // Zera a velocidade atual
                pizzaBoxRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }
            carriedPizzaBox = null;
        }

        // Adiciona a lógica para dropar os consumíveis
        if (UnityEngine.Random.value <= consumable1DropChance)
        {
            Instantiate(consumable1Prefab, transform.position, Quaternion.identity);
        }
        if (UnityEngine.Random.value <= consumable2DropChance)
        {
            Instantiate(consumable2Prefab, transform.position, Quaternion.identity);
        }
    }

    void ChooseTarget()
    {
        FindClosestObject();
        if (target == null)
        {
            FindClosestPlayer();
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null;

        foreach (GameObject p in players)
        {
            float distance = Vector3.Distance(transform.position, p.transform.position);
            if (distance < closestDistance && distance < viewDistance)
            {
                closestDistance = distance;
                closestPlayer = p.transform;
            }
        }

        target = closestPlayer;
    }

    void FindClosestObject()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PizzaBox");
        float closestDistance = Mathf.Infinity;
        Transform closestObject = null;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance && distance < viewDistance)
            {
                closestDistance = distance;
                closestObject = obj.transform;
            }
        }

        target = closestObject;
    }

    void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
