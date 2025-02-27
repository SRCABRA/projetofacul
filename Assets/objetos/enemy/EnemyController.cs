using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player; // Alvo do inimigo
    public Transform target; // Alvo do inimigo
    public float life = 3f; // Vida do inimigo
    public float speed = 5f; // Velocidade do inimigo
    public float viewDistance = 40f; // Distância máxima para identificar um alvo

    public float danobulletbase = 1f; //dano base do tiro
    public float danoAreaAttack = 3f;  //dano do ataque de area
    public GameObject pizzaBoxPrefab; // Prefab da caixa de pizza

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

        if (carriedPizzaBox != null)
        {
            // Manter a caixa de pizza na cabeça do inimigo
            carriedPizzaBox.transform.position = transform.position + Vector3.up * 2;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")){
            life -= danobulletbase;
        }
        if(collision.gameObject.CompareTag("AreaAttack")){
            life -= danoAreaAttack;
        }
        if (collision.gameObject.CompareTag("BoxPizzaPrefab") && carriedPizzaBox == null)
        {
            // Destrua o BoxPizzaPrefab original
            Destroy(collision.gameObject);

            // Instancie uma nova caixa de pizza e anexe à cabeça do inimigo
            carriedPizzaBox = Instantiate(pizzaBoxPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            carriedPizzaBox.transform.SetParent(transform);
        }
    }

    void DropPizzaBox()
    {
        if (carriedPizzaBox != null)
        {
            // Solte a caixa de pizza
            carriedPizzaBox.transform.SetParent(null);
            carriedPizzaBox = null;
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
        GameObject[] objects = GameObject.FindGameObjectsWithTag("BoxPizzaPrefab");
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
