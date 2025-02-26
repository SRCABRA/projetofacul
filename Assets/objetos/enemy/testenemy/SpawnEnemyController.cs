using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnTime = 3f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTime <= 0)
        {
            Instantiate(enemyPrefab, transform.position, transform.rotation);
            spawnTime = 3f;
        }
        else
        {
            spawnTime -= Time.deltaTime;
        }
    }
}
