using UnityEngine;

public class EnemyController : EnemyPai
{
    [Header("Configurações do Inimigo")]
    public bool enableMovement = true;
    public bool enableTargeting = true;
    public bool enableItemDrop = true;

    [Header("Referências")]
    public Transform player;
    public Transform target;

    [Header("Configuração de Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float viewDistance = 40f;

    [Header("Itens para Drop")]
    [SerializeField] private GameObject pizzaBoxPrefab;
    [SerializeField] private GameObject consumable1Prefab;
    [SerializeField] private GameObject consumable2Prefab;
    [SerializeField] private GameObject consumable4Prefab;

    [Header("Chance de Drop")]
    [Range(0f, 1f)] [SerializeField] private float consumable1DropChance = 0.3f;
    [Range(0f, 1f)] [SerializeField] private float consumable2DropChance = 0.3f;
    [Range(0f, 1f)] [SerializeField] private float consumable4DropChance = 0.1f;

    private GameObject carriedPizzaBox;

    protected override void Start()
    {
        base.Start();
        if (enableTargeting) ChooseTarget();
    }

    protected override void Update()
    {
        base.Update();
        if (enableTargeting) ChooseTarget();
        if (enableMovement) MoveTowardsTarget();
    }

    protected override void Drop()
    {
        if (!enableItemDrop) return;

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

        TryDropConsumable(consumable1Prefab, consumable1DropChance);
        TryDropConsumable(consumable2Prefab, consumable2DropChance);
        TryDropConsumable(consumable4Prefab, consumable4DropChance);
    }

    private void TryDropConsumable(GameObject consumablePrefab, float dropChance)
    {
        if (consumablePrefab != null && Random.value <= dropChance)
        {
            Instantiate(consumablePrefab, transform.position, Quaternion.identity);
        }
    }

    private void ChooseTarget()
    {
        FindClosestObject();
        if (target == null) FindClosestPlayer();
    }

    private void FindClosestPlayer()
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

    private void FindClosestObject()
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

    private void MoveTowardsTarget()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
