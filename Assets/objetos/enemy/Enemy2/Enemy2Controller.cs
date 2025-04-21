using UnityEngine;
using System.Collections;

public class Enemy2Controller : EnemyPai
{
    [Header("Movimentação")]
    public Transform[] waypoints; 
    public float speed = 2f;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private bool isPaused = false;

    [Header("Torreta")]
    public GameObject turretPrefab;

    [Tooltip("Tempo entre cada pausa para spawnar a torreta")]
    public float turretSpawnInterval = 10f;

    [Tooltip("Tempo em que o inimigo fica parado após spawnar a torreta")]
    public float pauseDuration = 1f;

    // ⬇️ Novo: Referência ao Animator e posição anterior
    private Animator animator;
    private Vector3 lastPosition;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>(); // Garante que o Animator está no mesmo GameObject
        lastPosition = transform.position;
        StartCoroutine(PauseMovement());
    }

    protected override void Update()
    {
        base.Update();

        if (isPaused || waypoints.Length == 0)
        {
            UpdateAnimatorSpeed(0f); // Se estiver pausado, zera a velocidade
            return;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);
        
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
        }


        float currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        UpdateAnimatorSpeed(currentSpeed);
        Debug.Log("Enemy Speed: " + currentSpeed);
        lastPosition = transform.position;

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 1f)
        {
            if (movingForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length - 1)
                    movingForward = false;
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex <= 0)
                    movingForward = true;
            }
        }
    }

    IEnumerator PauseMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(turretSpawnInterval);
            isPaused = true;
            UpdateAnimatorSpeed(0f); // Zera animação enquanto está parado
            InstantiateTurret();
            yield return new WaitForSeconds(pauseDuration);
            isPaused = false;
        }
    }

    void InstantiateTurret()
    {
        if (turretPrefab != null)
        {
            Vector3 spawnOffset = transform.right * 1.5f; // Ajusta 1.5f conforme a distância lateral desejada
            Vector3 spawnPosition = transform.position + spawnOffset;

            Instantiate(turretPrefab, spawnPosition, Quaternion.identity);

        }
        else
        {
            Debug.LogWarning("Turret Prefab não foi atribuído no Inspector!");
        }
    }

    void UpdateAnimatorSpeed(float currentSpeed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }
    }
}
