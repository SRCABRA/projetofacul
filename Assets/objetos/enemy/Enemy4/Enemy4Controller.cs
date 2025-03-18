using UnityEngine;

public class Enemy4Controller : EnemyController
{
    public GameObject enemyMinionPrefab; // Prefab dos inimigos menores
    public int minionsToSpawn = 3; // Quantidade de inimigos menores gerados
    public float spawnOffset = 1.5f; 

    protected override void Drop()
    {
        base.Drop(); // Mantém a lógica de drop de itens do EnemyController
        SpawnMinions();
    }

    private void SpawnMinions()
    {
        if (enemyMinionPrefab != null)
        {
            for (int i = 0; i < minionsToSpawn; i++)
            {
                Vector3 spawnPosition = transform.position + (Random.insideUnitSphere * spawnOffset);
                spawnPosition.y = transform.position.y;
                Instantiate(enemyMinionPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
