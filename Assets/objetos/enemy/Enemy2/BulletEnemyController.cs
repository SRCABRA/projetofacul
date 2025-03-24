using UnityEngine;

public class BulletEnemyController : MonoBehaviour
{
    public float speed = 10.0f;
    private Vector3 direction; // Direção fixa da bala

    void Start()
    {
        SetInitialDirection();
    }

    void Update()
    {
        // Move a bala na direção inicial (sem recalcular)
        transform.position += direction * speed * Time.deltaTime;
    }

    void SetInitialDirection()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Calcula a direção apenas uma vez
            direction = (player.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            Destroy(gameObject); // Se não tem player, destrói a bala
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroi a bala ao colidir
        }
    }
}
