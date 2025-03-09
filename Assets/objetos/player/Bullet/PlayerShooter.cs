using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    private float timer;
    public GameObject bullet;
    public float shootRange = 40f; // Distância mínima para atirar

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
                if (distance <= shootRange) // Só atira se o inimigo estiver dentro da distância permitida
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
        Debug.Log("Atirou!");
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
}
