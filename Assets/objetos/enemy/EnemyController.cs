using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public float life = 10f;
    public float speed = 5f;
    public float viewDistance = 40f;

    public float danobulletbase = 4f;
    public float danoAreaAttack = 6f;
    public float danoBackpack = 3f;

    public GameObject pizzaBoxPrefab;
    public GameObject consumable1Prefab;
    public GameObject consumable2Prefab;
    public float consumable1DropChance = 0.3f;
    public float consumable2DropChance = 0.3f;

    private GameObject carriedPizzaBox;
    private Rigidbody rb;
    private Renderer enemyRenderer;
    private Color originalColor;
    private bool isJumping = false;
    private bool isFlashing = false; // Para evitar chamadas repetidas

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
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
            isJumping = true;
            life -= danoAreaAttack;
            JumpEffect();
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            life -= danobulletbase;
        }
        else if (collision.gameObject.CompareTag("backpack"))
        {
            life -= danoBackpack;
        }

        // Pisca vermelho para qualquer dano
        FlashRed();
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("AreaAttack"))
        {
            isJumping = false;
        }
    }

    void FlashRed()
    {
        if (enemyRenderer != null && !isFlashing) // Evita chamadas múltiplas
        {
            isFlashing = true;
            enemyRenderer.material.color = Color.red;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    void ResetColor()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }
        isFlashing = false;
    }

    void JumpEffect()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);
        }
    }

    void Drop()
    {
        if (carriedPizzaBox != null)
        {
            carriedPizzaBox.transform.SetParent(null);
            Rigidbody pizzaBoxRigidbody = carriedPizzaBox.GetComponent<Rigidbody>();
            if (pizzaBoxRigidbody != null)
            {
                pizzaBoxRigidbody.linearVelocity = Vector3.zero;
                pizzaBoxRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }
            carriedPizzaBox = null;
        }

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
