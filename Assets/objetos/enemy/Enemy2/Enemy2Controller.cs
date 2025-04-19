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

    protected override void Start()
    {
        base.Start();
        StartCoroutine(PauseMovement());
    }

    protected override void Update()
    {
        base.Update();

        if (isPaused || waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

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
            InstantiateTurret();
            yield return new WaitForSeconds(pauseDuration);
            isPaused = false;
        }
    }

    void InstantiateTurret()
    {
        if (turretPrefab != null)
        {
            Instantiate(turretPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Turret Prefab não foi atribuído no Inspector!");
        }
    }
}
