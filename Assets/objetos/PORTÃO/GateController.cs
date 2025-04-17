using UnityEngine;
using System.Collections.Generic;

public class BarrierController : MonoBehaviour
{
    [Header("Barrier Settings")]
    public int requiredEnemiesDefeated = 5;
    public float descentSpeed = 2f;
    public float descentHeight = 3f;
    public float detectionRadius = 10f;

    [Header("Shake Effect")]
    public float shakeIntensity = 0.2f;
    public float shakeDamping = 0.05f;

    [Header("Optional Dependencies")]
    public BarrierController previousBarrier;

    [Header("Debug Info (Somente Leitura)")]
    [SerializeField] private int detectedEnemies = 0;
    [SerializeField] private int defeatedEnemies = 0;

    private bool isOpening = false;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private HashSet<GameObject> trackedEnemies = new HashSet<GameObject>();

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition - new Vector3(0, descentHeight, 0);
    }

    void Update()
    {
        // Aguarda portão anterior abrir, se houver
        if (previousBarrier != null && !previousBarrier.isOpening)
            return;

        if (!isOpening)
        {
            TrackNearbyEnemies();
            CheckDefeatedEnemies();
        }

        if (defeatedEnemies >= requiredEnemiesDefeated)
        {
            isOpening = true;
        }

        if (isOpening)
        {
            // Move o portão para baixo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * descentSpeed);

            // Efeito de tremor
            if (shakeIntensity > 0)
            {
                float shakeX = Random.Range(-shakeIntensity, shakeIntensity);
                float shakeY = Random.Range(-shakeIntensity, shakeIntensity);
                transform.position += new Vector3(shakeX, shakeY, 0);
                shakeIntensity -= shakeDamping * Time.deltaTime;
            }
        }
    }

    void TrackNearbyEnemies()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allEnemies)
        {
            if (trackedEnemies.Contains(enemy)) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= detectionRadius)
            {
                trackedEnemies.Add(enemy);
                detectedEnemies = trackedEnemies.Count;
                Debug.Log("👀 Inimigo detectado na área! Total rastreados: " + detectedEnemies);
            }
        }
    }

    void CheckDefeatedEnemies()
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject enemy in trackedEnemies)
        {
            if (enemy == null)
            {
                defeatedEnemies++;
                Debug.Log("☠️ Inimigo rastreado foi derrotado! Total: " + defeatedEnemies);
                toRemove.Add(enemy);
            }
        }

        foreach (GameObject deadEnemy in toRemove)
        {
            trackedEnemies.Remove(deadEnemy);
        }

        detectedEnemies = trackedEnemies.Count;
    }

    // Gizmos: mostram área de detecção no editor
    private void OnDrawGizmos()
    {
        Gizmos.color = isOpening ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, $"Detectados: {detectedEnemies} / Mortos: {defeatedEnemies}");
#endif
    }
}
