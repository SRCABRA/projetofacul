using System.Threading;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab da bala
    public Transform firePoint; // Ponto de disparo
    public float fireRate = 4f; // Tempo entre os disparos
    private float fireTimer = 0f; // Temporizador interno

    void Update()
    {
        // Atualiza o tempo
        fireTimer += Time.deltaTime;

        // Se passou 4 segundos, dispara
        if (fireTimer >= fireRate)
        {
            Shoot(); // Chama o método de disparo
            fireTimer = 0f; // Reseta o temporizador
        }
    }

    // Método que dispara a BulletEnemy
    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // Cria a bala
        }
        else
        {
            Debug.LogWarning("BulletPrefab ou FirePoint não foram atribuídos!");
        }
    }
}
