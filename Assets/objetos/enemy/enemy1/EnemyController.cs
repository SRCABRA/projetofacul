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

    [Header("Animação")]
    [SerializeField] private Animator animator;

    [Header("Configuração de Patrulha")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderInterval = 5f;
    private float wanderTimer;
    private Vector3 wanderTarget;

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
        wanderTimer = wanderInterval;
    }

    protected override void Update()
    {
        base.Update();

        Debug.Log("Update rodando no inimigo: " + gameObject.name);

        if (!enableMovement)
        {
            SetRunning(false);
            return;
        }

        if (PlayerInSight())
        {
            FindClosestPlayer();
            MoveTowardsTarget();
        }
        else
        {
            PatrolBehavior();
        }
    }

    private bool PlayerInSight()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            float distance = Vector3.Distance(transform.position, p.transform.position);
            if (distance <= viewDistance)
            {
                player = p.transform;
                return true;
            }
        }

        return false;
    }

    private void PatrolBehavior()
    {
        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval || Vector3.Distance(transform.position, wanderTarget) < 1f)
        {
            Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
            wanderTarget = new Vector3(
                transform.position.x + randomCircle.x,
                transform.position.y,
                transform.position.z + randomCircle.y
            );
            wanderTimer = 0f;
        }

        Vector3 direction = (wanderTarget - transform.position).normalized;
        direction.y = 0;

        transform.position += direction * (speed * 0.5f) * Time.deltaTime;
        RotateTowards(direction);

        SetRunning(false); // patrulhando = Idle
    }

    private void MoveTowardsTarget()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            transform.position += direction * speed * Time.deltaTime;
            RotateTowards(direction);

            SetRunning(true); // perseguindo = Run
        }
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
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

        player = closestPlayer;
    }

private void SetRunning(bool isRunning)
{
    if (animator != null)
    {
        animator.SetBool("IsRunning", isRunning);
        Debug.Log($"{gameObject.name} - SetRunning({isRunning})");
    }
    else
    {
        Debug.LogWarning($"{gameObject.name} - Animator está NULL!");
    }
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
}
