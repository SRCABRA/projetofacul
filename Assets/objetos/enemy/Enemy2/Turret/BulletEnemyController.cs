using UnityEngine;

public class BulletEnemyController : MonoBehaviour
{
    public float speed = 10.0f;
    private Vector3 direction; // Direção fixa da bala

    void Start()
    {
        SetInitialDirection();
        Destroy(gameObject, 200);
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
            Debug.Log("não foi encontrado");
            Destroy(gameObject); // Se não tem player, destrói a bala
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("colidiu com o player (Collision)");
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("colidiu com o player (Trigger)");
            Destroy(gameObject);
        }
    }

}
