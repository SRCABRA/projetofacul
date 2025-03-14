using UnityEngine;
using System.Collections; // Necessário para corrotinas

public class Enemy2Controller : EnemyPai
{
    public Transform[] waypoints; // Array de pontos do caminho
    public float speed = 2f; // Velocidade do inimigo
    private int currentWaypointIndex = 0; // Índice do ponto atual
    private bool movingForward = true; // Controla se o inimigo está indo para frente ou voltando

    private bool isPaused = false; // Indica se o inimigo está parado

    public GameObject turretPrefab; // Prefab da torreta

    protected override void Start()
    {
        base.Start();
        StartCoroutine(PauseMovement()); // Inicia a rotina de pausas
    }

    protected override void Update()
    {
        base.Update();
        if (isPaused || waypoints.Length == 0) return; // Se estiver pausado, não se move

        // Pega o waypoint atual
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Move o inimigo na direção do waypoint atual
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Verifica se chegou ao waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 1f)
        {
            // Se estiver indo para frente, avança no array
            if (movingForward)
            {
                currentWaypointIndex++;

                // Se chegou no último ponto, começa a voltar
                if (currentWaypointIndex >= waypoints.Length - 1)
                {
                    movingForward = false;
                }
            }
            else // Se estiver voltando, decrementa no array
            {
                currentWaypointIndex--;

                // Se chegou no primeiro ponto, começa a ir para frente novamente
                if (currentWaypointIndex <= 0)
                {
                    movingForward = true;
                }
            }
        }
    }

    // Corrotina para pausar o movimento a cada 15 segundos e instanciar a torreta
    IEnumerator PauseMovement()
    {
        while (true) // Loop infinito para repetir a pausa constantemente
        {
            yield return new WaitForSeconds(10f); // Espera 15 segundos
            isPaused = true; // Pausa o inimigo
            InstantiateTurret(); // Cria a torreta
            yield return new WaitForSeconds(1f); // Espera 1 segundo parado
            isPaused = false; // Volta ao movimento
        }
    }

    // Método para instanciar a torreta
    void InstantiateTurret()
    {
        if (turretPrefab != null)
        {
            Instantiate(turretPrefab, transform.position, Quaternion.identity); // Instancia na posição do inimigo
        }
        else
        {
            Debug.LogWarning("Turret Prefab não foi atribuído no Inspector!");
        }
    }
}
