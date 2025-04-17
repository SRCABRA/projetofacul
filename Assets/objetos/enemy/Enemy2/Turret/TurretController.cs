using UnityEngine;

public class TurretController : EnemyPai
{
    public GameObject bulletPrefab; // Prefab da bala
    public Transform firePoint; // Ponto de disparo
    public float fireRate = 4f; // Tempo entre os disparos
    private float fireTimer = 0f; // Temporizador interno

    public float detectionRange = 10f; // Distância de detecção do player
    private Transform playerTransform; // Referência ao jogador

    public GameObject explosionEffect; // Prefab da explosão

    protected override void Start()
    {
        base.Start();

        // Encontra o player pela tag (certifique-se de que o jogador tem a tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    protected override void Update()
    {
        base.Update();

        // Se o player não foi encontrado, não faz nada
        if (playerTransform == null) return;

        // Calcula a distância até o player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Se estiver dentro do alcance, atualiza o timer e atira
        if (distanceToPlayer <= detectionRange)
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= fireRate)
            {
                Shoot(); // Dispara
                fireTimer = 0f; // Reseta o temporizador
            }
        }
    }

    // Método que dispara a BulletEnemy
    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // Cria a bala
        }
    }

    // Método de destruição com efeito
    protected override void Drop()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        base.Drop(); // Chama o método Drop da classe EnemyPai para destruir a torreta
    }

    // Gizmo para visualizar o alcance no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
