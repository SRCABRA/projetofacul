using UnityEngine;

public class BulletEnemyController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [Tooltip("Velocidade da bala")]
    [SerializeField] private float speed = 10.0f;

    [Tooltip("Tempo até a bala ser destruída automaticamente")]
    [SerializeField] private float timeToDestroy = 20f;

    [Tooltip("Tag do alvo (ex: 'Player')")]
    [SerializeField] private string targetTagPlayer = "Player";

    [Header("Collision Settings")]
    [Tooltip("Ativar destruição por colisão com o alvo")]
    [SerializeField] private bool destroyOnCollision = false;

    [Tooltip("Ativar destruição por trigger com o alvo")]
    [SerializeField] private bool destroyOnTrigger = true;

    [Tooltip("Destruir com qualquer colisão (independente da tag)")]
    [SerializeField] private bool destroyOnAnyCollision = true;

    [Header("Debug (somente leitura)")]
    [Tooltip("Direção inicial da bala (calculada no Start)")]
    [SerializeField] private Vector3 direction;

    private void Start()
    {
        SetInitialDirection();
        Destroy(gameObject, timeToDestroy);
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void SetInitialDirection()
    {
        GameObject target = GameObject.FindGameObjectWithTag(targetTagPlayer);
        if (target != null)
        {
            direction = (target.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            Debug.LogWarning($"Alvo com tag '{targetTagPlayer}' não foi encontrado.");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (destroyOnAnyCollision)
        {
            Debug.Log("Colidiu com qualquer coisa via Collision");
            Destroy(gameObject);
        }
        else if (destroyOnCollision && collision.gameObject.CompareTag(targetTagPlayer))
        {
            Debug.Log("Colidiu com o alvo via Collision");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destroyOnAnyCollision)
        {
            Debug.Log("Colidiu com qualquer coisa via Trigger");
            Destroy(gameObject);
        }
        else if (destroyOnTrigger && other.CompareTag(targetTagPlayer))
        {
            Debug.Log("Colidiu com o alvo via Trigger");
            Destroy(gameObject);
        }
    }
}
