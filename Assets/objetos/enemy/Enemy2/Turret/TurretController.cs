using UnityEngine;

public class TurretController : EnemyPai
{
    [Header("Turret Settings")]
    public GameObject bulletPrefab; // Prefab da bala
    public Transform firePoint; // Ponto de disparo
    public float fireRate = 4f; // Tempo entre os disparos
    private float fireTimer = 0f; // Temporizador interno

    [Header("Detection")]
    public float detectionRange = 10f; // Distância de detecção do player
    private Transform playerTransform; // Referência ao jogador
    public LayerMask obstacleMask; // Layer dos obstáculos (paredes, etc)

    [Header("FX")]
    public GameObject explosionEffect; // Prefab da explosão

    protected override void Start()
    {
        base.Start();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange && CanSeePlayer())
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= fireRate)
            {
                Shoot(); // Dispara
                fireTimer = 0f; // Reseta o temporizador
            }
        }
    }

    private bool CanSeePlayer()
    {
        if (playerTransform == null) return false;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, obstacleMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red, 0.5f);
            return false; // Obstáculo bloqueando
        }

        Debug.DrawLine(transform.position, playerTransform.position, Color.green, 0.5f);
        return true; // Sem obstáculos
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }

    protected override void Drop()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        base.Drop();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
