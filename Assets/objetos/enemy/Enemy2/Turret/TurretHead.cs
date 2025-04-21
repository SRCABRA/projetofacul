using UnityEngine;

public class TurretHead : MonoBehaviour
{
    [Header("Alvo")]
    public Transform target; // Normalmente o Player

    [Header("Detecção")]
    public float detectionRange = 50f;
    public LayerMask obstacleMask;

    [Header("Rotação")]
    public float rotationSpeed = 5f;


    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }


    private void Update()
    {
        if (target == null) return;

        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        // Verifica se o alvo está dentro do alcance
        if (distanceToTarget <= detectionRange)
        {
            // Faz raycast para ver se há visão direta
            Ray ray = new Ray(transform.position, directionToTarget.normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, detectionRange, ~obstacleMask))
            {
                if (hit.transform == target)
                {
                    RotateTowards(target.position);
                }
            }
        }
    }

    void RotateTowards(Vector3 lookPosition)
    {
        Vector3 direction = lookPosition - transform.position;
        direction.y = 0; // Opcional: trava a rotação apenas no eixo Y (sem olhar pra cima/baixo)

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
