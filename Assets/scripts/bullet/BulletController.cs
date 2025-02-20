using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject enemy;
    public float speed = 10.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemy != null){
            Vector3 direction = (enemy.transform.position - transform.position).normalized;

            transform.position += direction * speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void FindClosestEnemy()
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

        enemy = closestEnemy.gameObject;
    }
}
