using UnityEngine;

public class DustTrailSpawner : MonoBehaviour
{
    public GameObject dustPrefab; // Prefab da poeira
    public float spawnRate = 0.05f; // Tempo entre cada poeira gerada
    public Vector3 dustOffset = new Vector3(0, 0.1f, 0); // Ajuste a posição no Inspector

    private float nextSpawnTime = 0f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Verifica se há entrada de movimento do jogador
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        // Somente gera poeira se o personagem estiver no chão e se movendo por input
        if (controller.isGrounded && isMoving && Time.time >= nextSpawnTime)
        {
            SpawnDust();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnDust()
    {
        // Define a posição da poeira com base no offset configurável
        Vector3 spawnPos = transform.position + dustOffset + new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));

        Instantiate(dustPrefab, spawnPos, Quaternion.identity);
    }
}
