using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnTime = 1f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTime <= 0)
        {
            Instantiate(enemyPrefab, transform.position, transform.rotation);
            spawnTime = 15f;
        }
        else
        {
            spawnTime -= Time.deltaTime;
        }
    }
}
