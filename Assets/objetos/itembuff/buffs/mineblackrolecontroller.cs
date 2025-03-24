using UnityEngine;
using System.Collections;

public class MineBlackHoleController : MonoBehaviour
{
    public float radius = 20f;        // Raio de atração
    public float pullForce = 100f;     // Força de atração
    public float effectDuration = 2f; // Tempo de duração do efeito

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(PullEnemies());
        }
    }

    IEnumerator PullEnemies()
    {
        float elapsedTime = 0f;

        while (elapsedTime < effectDuration)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    Rigidbody enemyRb = col.GetComponent<Rigidbody>();
                    Debug.Log("Colisão com a mina");
                    if (enemyRb != null)
                    {
                        // Puxa o inimigo rapidamente para o centro da mina
                        Vector3 direction = (transform.position - enemyRb.position).normalized;
                        enemyRb.linearVelocity = direction * pullForce;
                    }
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Remove a mina após o efeito
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
