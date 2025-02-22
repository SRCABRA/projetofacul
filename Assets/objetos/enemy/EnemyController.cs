using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float life = 3f;
    public float speed = 5f;
    public float viewDistance = 40f;	

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestPlayer();
        MoveTowardsPlayer();

        if (life <= 0){
                Destroy(gameObject);
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Busca todos os jogadores na cena
        float closestDistance = Mathf.Infinity; // Define a menor distância como infinito
        Transform closestPlayer = null; // Define o jogador mais próximo como nulo

        foreach (GameObject p in players)  // Para cada jogador na cena
        {
            float distance = Vector3.Distance(transform.position, p.transform.position); // Calcula a distância entre o inimigo e o jogador
            if (distance < closestDistance && distance < viewDistance) // Se a distância for menor que a menor distância atual e menor que a distância de visão
            {
                closestDistance = distance;
                closestPlayer = p.transform;
            }
        }

        player = closestPlayer;
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Ignora a componente y
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet")){
            life -= 1;
        }

    }
}
