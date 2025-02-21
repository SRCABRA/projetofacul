using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestPlayer();
        MoveTowardsPlayer();
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); // Busca todos os jogadores na cena
        float closestDistance = Mathf.Infinity; // Define a menor distância como infinito
        Transform closestPlayer = null; // Define o jogador mais próximo como nulo

        foreach (GameObject p in players)  // Para cada jogador na cena
        {
            float distance = Vector3.Distance(transform.position, p.transform.position); // Calcula a distância entre o inimigo e o jogador
            if (distance < closestDistance)
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
}
