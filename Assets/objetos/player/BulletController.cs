using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 10.0f;
    private Transform enemy;  // para pegar a posição, rotação e escala do inimigo

    void Start()
    {
        // Encontra um inimigo automaticamente
        FindClosestEnemy();
    }

    void Update()
    {
        // Se não tem inimigo, destrua a bala
        if (enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move o projétil na direção do inimigo
        Vector3 direction = (enemy.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void FindClosestEnemy() // Encontra o inimigo mais próximo
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");  // Busca todos os inimigos na cena
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject e in enemies)
        {
            float distance = Vector3.Distance(transform.position, e.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = e.transform;
            }
        }

        enemy = closestEnemy;  // Define o inimigo mais próximo
    }
}
