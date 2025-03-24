using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    private float timer;
    public GameObject bullet;
    public float shootRange = 40f; // Distância mínima para atirar
    public LayerMask obstacleMask; // Camada dos obstáculos

    void Start()
    {
        timer = 0;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            GameObject nearestEnemy = GetNearestEnemy();
            if (nearestEnemy != null)
            {
                float distance = Vector3.Distance(transform.position, nearestEnemy.transform.position);
                
                if (distance <= shootRange && CanSeeEnemy(nearestEnemy)) // Só atira se enxergar o inimigo
                {
                    Shoot();
                    timer = 0;
                }
            }
        }
    }

    void Shoot()
    {
        Instantiate(bullet, transform.position, transform.rotation);
    }

    GameObject GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    bool CanSeeEnemy(GameObject enemy)
    {
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, enemy.transform.position);

        // Raycast para detectar se há algo entre o jogador e o inimigo
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, obstacleMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, 0.2f);
            return false; // O tiro é bloqueado
        }

        Debug.DrawLine(transform.position, enemy.transform.position, Color.green, 0.2f);
        return true; // O jogador pode atirar
    }
}
