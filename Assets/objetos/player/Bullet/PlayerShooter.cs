using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    private float timer;
    public GameObject bullet;
    public float shootRange = 40f; // Distância máxima para atirar
    public LayerMask obstacleMask; // Camadas dos obstáculos (defina no Inspector)

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

                // Só atira se o inimigo estiver dentro do alcance e visível
                if (distance <= shootRange && CanSeeEnemy(nearestEnemy))
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

        // Raycast que só colide com camadas marcadas em obstacleMask
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, obstacleMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, 0.5f);
            Debug.Log("Visão bloqueada por: " + hit.collider.name + " | Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));
            return false; // Obstáculo entre jogador e inimigo
        }

        Debug.DrawLine(transform.position, enemy.transform.position, Color.green, 0.5f);
        return true; // Nada bloqueando a visão
    }
}
