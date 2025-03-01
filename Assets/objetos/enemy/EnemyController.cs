using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player; // Alvo do inimigo
    public Transform target; // Alvo do inimigo
    public float life = 3f; // Vida do inimigo
    public float speed = 5f; // Velocidade do inimigo
    public float viewDistance = 40f; // Distância máxima para identificar um alvo

    public float danobulletbase = 4f; //dano base do tiro
    public float danoAreaAttack = 10f;  //dano do ataque de area
    public GameObject pizzaBoxPrefab; // Prefab da caixa de pizza
    public GameObject consumable1Prefab; // Prefab do consumable1
    public float consumable1DropChance = 0.5f; // Chance de drop do consumable1 (50%)

    private GameObject carriedPizzaBox; // Referência da caixa de pizza carregada

    void Start()
    {
        ChooseTarget(); // Escolhe um alvo para o inimigo
    }

    // Update is called once per frame
    void Update()
    {
        ChooseTarget();
        MoveTowardsTarget();

        if (life <= 0)
        {
            DropPizzaBox();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AreaAttack"))
        {
            life -= danoAreaAttack;
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            life -= danobulletbase;
        }
        else if (collision.gameObject.CompareTag("PizzaBox") && collision.transform.parent != player)
        {
            Rigidbody pizzaBoxRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (pizzaBoxRigidbody != null && Mathf.Abs(pizzaBoxRigidbody.velocity.x) < 0.01f)  //mudar para linearVelocity na versão mais recente
            {
                PickUpPizzaBox(collision.gameObject);
            }
        }
    }

    void PickUpPizzaBox(GameObject pizzaBox)
    {
        carriedPizzaBox = pizzaBox;
        carriedPizzaBox.transform.SetParent(transform);
        carriedPizzaBox.transform.localPosition = new Vector3(0, 1, 0); // Ajuste a posição conforme necessário
        carriedPizzaBox.transform.localRotation = Quaternion.identity; // Reseta a rotação
    }

    void DropPizzaBox()
    {
        if (carriedPizzaBox != null)
        {
            carriedPizzaBox.transform.SetParent(null);
            Rigidbody pizzaBoxRigidbody = carriedPizzaBox.GetComponent<Rigidbody>();
            if (pizzaBoxRigidbody != null)
            {
                pizzaBoxRigidbody.velocity = Vector3.zero; // Zera a velocidade atual
                pizzaBoxRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse); // Ajuste a força conforme necessário
            }
            carriedPizzaBox = null;
        }

        // Adiciona a lógica para dropar o consumable1
        if (UnityEngine.Random.value <= consumable1DropChance)
        {
            Instantiate(consumable1Prefab, transform.position, Quaternion.identity);
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

