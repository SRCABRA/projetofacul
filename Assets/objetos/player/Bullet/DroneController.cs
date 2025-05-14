using UnityEngine;

public class DroneController : MonoBehaviour
{
    public GameObject StayHereDrone;
    public float speed;
    public float maxSpeed;
    public float acceleration = 0.5f;
    public float rotationSpeed = 5f; // Velocidade de rotação do drone

    void Update()
    {
        DroneMovement();
        LookAtNearestEnemy();

        if (speed < maxSpeed)
        {
            speed += acceleration * Time.deltaTime;
            speed = Mathf.Min(speed, maxSpeed);
        }
    }

    void DroneMovement()
    {
        transform.position = Vector3.Lerp(transform.position, StayHereDrone.transform.position, speed * Time.deltaTime);
    }

    void LookAtNearestEnemy()
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

        if (nearestEnemy != null)
        {
            Vector3 directionToEnemy = nearestEnemy.transform.position - transform.position;
            if (directionToEnemy != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime * 100f);
            }
        }
    }
}
